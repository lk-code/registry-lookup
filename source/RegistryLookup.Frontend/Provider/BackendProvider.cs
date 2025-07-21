using dev.lkcode.RegistryLookup.Abstractions;
using RegistryLookup.Client;

namespace dev.lkcode.RegistryLookup.Frontend.Provider;

public class BackendProvider(RestClient RestClient) : IBackendProvider
{
    public async Task<bool> IsBackendAvailable(CancellationToken cancellationToken = default)
    {
        string? backendVersion = await RestClient.Version.GetAsync(x => { },
            cancellationToken);
        
        return !string.IsNullOrEmpty(backendVersion);
    }
}