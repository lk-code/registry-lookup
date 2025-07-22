using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Frontend.Models;
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
    private List<IRegistryEntry> _registryIndex = [];

    private RegistryHostModel _selectedHost;
    private List<RegistryHostModel> _hosts =
    [
        new("Docker Private Registry", typeof(RegistryLookup.DockerRegistryV2.RegistryHost), "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSaKKoEfs7PzMWKDgD5ms2JzUhvRDpAafzA4w&s"),
        new("Docker Hub", typeof(RegistryLookup.DockerHubRegistry.RegistryHost), "https://www.opc-router.de/wp-content/uploads/2023/07/Docker_150x150px-01-01-01.png"),
        new("NuGet.org", typeof(RegistryLookup.NuGetOrgRegistry.RegistryHost), "https://plpsoft.vn/ckfinder/connector?command=Proxy&lang=vi&type=Files&currentFolder=%2F&hash=c245c263ce0eced480effe66bbede6b4d46c15ae&fileName=logo-og-600x600.png")
    ];

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
            RegistryHost = await RegistryHostFactory.CreateAsync(hostUri, cancellationToken);

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