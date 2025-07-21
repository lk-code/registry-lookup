using dev.lkcode.RegistryLookup.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace dev.lkcode.RegistryLookup.Frontend.Pages;

public partial class Home : ComponentBase, IDisposable
{
    public string HostAddressInputValue { get; set; } = string.Empty;

    private string? _errorMessage = null;
    private string? _errorAdditionalMessage = null;
    private bool _checkingBackend = false;
    private bool _isBackendAvailable = false;
    private bool _checkingRegistryAvailability = false;
    private bool _registryAvailable = false;
    private bool _loadingRegistryIndex = false;
    private readonly CancellationTokenSource _ctsSource = new();
    private IRegistryHost? RegistryHost { get; set; } = null;
    private List<IRegistryEntry> _registryIndex = new List<IRegistryEntry>();

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

            CancellationToken cancellationToken = CancellationToken.None;
            bool isBackendAvailable = await BackendProvider.IsBackendAvailable(cancellationToken);

            await InvokeAsync(() =>
            {
                _isBackendAvailable = isBackendAvailable;
                _checkingBackend = false;

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

    public void Dispose()
    {
        _ctsSource?.Cancel();
        _ctsSource?.Dispose();

        GC.SuppressFinalize(this);
    }

    private void HandleBlur(FocusEventArgs args)
    {
        _errorMessage = null;
        _errorAdditionalMessage = null;

        if (string.IsNullOrEmpty(HostAddressInputValue))
        {
            return;
        }

        if (!Uri.TryCreate(HostAddressInputValue.TrimEnd('/'), UriKind.Absolute, out Uri? hostUri))
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
            _checkingRegistryAvailability = true;

            StateHasChanged();
        });

        try
        {
            // load as docker registry:v2
            RegistryHost = RegistryHostFactory.Create(hostUri);

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

            await LoadEntriesAsync(cancellationToken);
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

    private async Task LoadEntriesAsync(CancellationToken cancellationToken = default)
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
            
            IReadOnlyCollection<IRegistryEntry> entries = await RegistryHost.GetEntriesAsync(CancellationToken.None);
            await InvokeAsync(() =>
            {
                _registryIndex = entries.ToList();
                _loadingRegistryIndex = true;

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
}