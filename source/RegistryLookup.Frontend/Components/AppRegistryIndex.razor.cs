using dev.lkcode.RegistryLookup.Abstractions;
using Microsoft.AspNetCore.Components;

namespace dev.lkcode.RegistryLookup.Frontend.Components;

public partial class AppRegistryIndex : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _ctsSource = new();
    private string _searchValue = string.Empty;
    private List<IRegistryItem> _registryIndex = [];
    private DisplayConfiguration? _registryDisplayConfiguration = null;
    private bool _loadingRegistryIndex = false;
    private string? _errorMessage = null;
    private string? _errorAdditionalMessage = null;

    [Parameter]
    public required IRegistryHost RegistryHost { get; set; }

    public void Dispose()
    {
        _ctsSource?.Cancel();
        _ctsSource?.Dispose();

        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
        }
    }

    public async Task ReloadAsync(CancellationToken cancellationToken)
    {
        if (RegistryHost is null)
        {
            return;
        }

        try
        {
            await InvokeAsync(() =>
            {
                _registryIndex.Clear();
                _loadingRegistryIndex = true;

                StateHasChanged();
            });

            IReadOnlyCollection<IRegistryItem> entries = await RegistryHost.GetEntriesAsync(CancellationToken.None);
            DisplayConfiguration itemTypeTitle = RegistryHost.GetDisplayConfiguration();
            await InvokeAsync(() =>
            {
                _registryIndex = entries.ToList();
                _registryDisplayConfiguration = itemTypeTitle;
                _loadingRegistryIndex = false;

                StateHasChanged();
            });
        }
        catch (Exception err)
        {
            await InvokeAsync(() =>
            {
                _errorMessage = "Registry Index could not be loaded";

                if (err.InnerException is not null
                    && !string.IsNullOrEmpty(err.InnerException.Message))
                {
                    _errorAdditionalMessage = err.InnerException.Message;
                }

                StateHasChanged();
            });
        }
        finally
        {
            await InvokeAsync(() =>
            {
                _loadingRegistryIndex = false;

                StateHasChanged();
            });
        }
    }

    private Func<IRegistryItem, bool> IndexFilterFunc => x =>
    {
        if (string.IsNullOrWhiteSpace(_searchValue))
            return true;

        if (x.Name.Contains(_searchValue, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
}