using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.DockerRegistryV2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace dev.lkcode.RegistryLookup.Frontend.Pages;

public partial class Home : ComponentBase, IDisposable
{
    public string HostAddressInputValue { get; set; } = string.Empty;

    private string? _errorMessage = null;
    private bool _isBusy = false;
    private readonly CancellationTokenSource _ctsSource = new();
    private IRegistryHost? RegistryHost { get; set; } = null;

    public void Dispose()
    {
        _ctsSource?.Cancel();
        _ctsSource?.Dispose();

        GC.SuppressFinalize(this);
    }

    private void HandleBlur(FocusEventArgs args)
    {
        _errorMessage = null;

        if (string.IsNullOrEmpty(HostAddressInputValue))
        {
            return;
        }

        if (!Uri.TryCreate(HostAddressInputValue, UriKind.Absolute, out Uri? hostUri))
        {
            _errorMessage = $"Invalid host address: {HostAddressInputValue}";
            return;
        }

        _ = LoadRegistryAsync(hostUri);
    }

    private async Task LoadRegistryAsync(Uri hostUri, CancellationToken cancellationToken = default)
    {
        await InvokeAsync(() =>
        {
            _isBusy = true;

            StateHasChanged();
        });

        try
        {
            // load as docker registry:v2
            RegistryHost = new RegistryHost(hostUri);

            bool isAvailable = await RegistryHost.IsAvailableAsync(CancellationToken.None);
            if (!isAvailable)
            {
                await InvokeAsync(() =>
                {
                    _errorMessage = $"Registry host not available: {hostUri}";

                    StateHasChanged();
                });
                return;
            }

            await LoadEntriesAsync(cancellationToken);
        }
        catch (Exception err)
        {
            await InvokeAsync(() =>
            {
                _errorMessage = $"Registry host not available: {hostUri}";

                StateHasChanged();
            });
        }
        finally
        {
            await InvokeAsync(() =>
            {
                _isBusy = false;

                StateHasChanged();
            });
        }
    }

    private async Task LoadEntriesAsync(CancellationToken cancellationToken = default)
    {
        if (RegistryHost is null)
        {
            return;
        }

        IReadOnlyCollection<IRegistryEntry> entries = await RegistryHost.GetEntriesAsync(CancellationToken.None);
    }
}