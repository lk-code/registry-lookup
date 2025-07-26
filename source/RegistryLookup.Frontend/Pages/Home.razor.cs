using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Frontend.Components;
using dev.lkcode.RegistryLookup.Frontend.Factories;
using dev.lkcode.RegistryLookup.Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace dev.lkcode.RegistryLookup.Frontend.Pages;

public partial class Home : ComponentBase, IDisposable
{
    public string HostAddressInputValue { get; set; } = string.Empty;
    public IRegistryHost? RegistryHost = null;
    public AppRegistryIndex? AppRegistryIndex = null;

    private readonly CancellationTokenSource _ctsSource = new();
    private string? _errorMessage = null;
    private string? _errorAdditionalMessage = null;
    private bool _checkingBackend = false;
    private bool _isBackendAvailable = false;
    private bool _checkingRegistryAvailability = false;
    private bool _registryAvailable = false;
    private RegistryHostModel? _selectedHost;
    private List<RegistryHostModel> _hosts = [];
    private IRegistryItem? _selectedRegistryItem = null;

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
            await InvokeAsync(() =>
            {
                _checkingBackend = true;

                StateHasChanged();
            });

            // create and set registries for selection
            List<RegistryHostModel> registries = await GetRegistryTypesAsync(_ctsSource.Token);
            await InvokeAsync(() =>
            {
                _hosts.Clear();
                _hosts = registries;

                StateHasChanged();
            });

            bool isBackendAvailable = await BackendProvider.IsBackendAvailable(_ctsSource.Token);

            await InvokeAsync(() =>
            {
                _isBackendAvailable = isBackendAvailable;
                _checkingBackend = false;

                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
                if (isBackendAvailable)
                {
                    Snackbar.Add("Backend is available", Severity.Success);
                }
                else
                {
                    _errorMessage = "Registry L00kUp Backend isn't available. Make sure the backend is running correctly.";
                    Snackbar.Add("Backend isn't available :( Please make sure, that the backend is running correctly", Severity.Error);
                }

                StateHasChanged();
            });
        }
    }

    private async Task<List<RegistryHostModel>> GetRegistryTypesAsync(CancellationToken ct)
    {
        string iconPathTemplate = "img/icons/{0}.svg";
        string[] availableRegistries = Registries.GetRegistryHostTypes();

        List<RegistryHostModel> registries = [];
        foreach (string availableRegistry in availableRegistries)
        {
            string iconName = availableRegistry.ToLowerInvariant();
            string iconPath = string.Format(iconPathTemplate, iconName);
            string icon = await ContentProvider.GetContentAsync(iconPath);

            registries.Add(
                new RegistryHostModel(availableRegistry,
                    icon,
                    availableRegistry));
        }

        return registries;
    }

    private void RegistryHostTypeSelectChanged()
    {
        HandleRegistryValueChange();
    }

    private void HostAddressInputChanged(FocusEventArgs args)
    {
        HandleRegistryValueChange();
    }

    private void HandleRegistryValueChange()
    {
        if (_selectedHost is null
            || string.IsNullOrEmpty(HostAddressInputValue))
        {
            return;
        }

        _errorMessage = null;
        _errorAdditionalMessage = null;

        if (!Uri.TryCreate(HostAddressInputValue.TrimEnd('/'), UriKind.Absolute, out Uri? hostUri))
        {
            _errorMessage = $"Invalid host address: {HostAddressInputValue}";
            return;
        }

        _ = LoadRegistryAsync(_selectedHost, hostUri);
    }

    private async Task LoadRegistryAsync(RegistryHostModel registryHostModel,
        Uri hostUri,
        CancellationToken cancellationToken = default)
    {
        await InvokeAsync(() =>
        {
            _checkingRegistryAvailability = true;

            StateHasChanged();
        });

        try
        {
            // load as docker registry:v2
            RegistryHost = await RegistryHostFactory.CreateAsync(registryHostModel.RegistryType,
                hostUri,
                cancellationToken);

            bool isAvailable = await RegistryHost.IsAvailableAsync(cancellationToken);
            await InvokeAsync(() =>
            {
                _registryAvailable = isAvailable;
                _checkingRegistryAvailability = false;

                StateHasChanged();
            });

            if (!isAvailable)
            {
                await InvokeAsync(() =>
                {
                    _errorMessage = $"Registry host not available: {hostUri}";

                    StateHasChanged();
                });
                return;
            }

            if (isAvailable)
            {
                await AppRegistryIndex!.ReloadAsync(cancellationToken);
            }
        }
        catch (Exception err)
        {
            await InvokeAsync(() =>
            {
                _errorMessage = $"Registry host not available: {hostUri} - " + err.Message;
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
                _checkingRegistryAvailability = false;

                StateHasChanged();
            });
        }
    }

    private void RegistryItemSelected(IRegistryItem item)
    {
        _selectedRegistryItem = item;
    }
}