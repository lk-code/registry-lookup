using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.Frontend.Factories;

public class RegistryHostFactory(IServiceProvider ServiceProvider) : IRegistryHostFactory
{
    public async Task<IRegistryHost?> CreateAsync(string registry,
        Uri hostUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(registry))
            return null;

        IRegistryHost? host = ServiceProvider.GetKeyedService<IRegistryHost>(registry);
        if (host == null)
            return null;

        if (host is IInitializableRegistryHost registryHostInit)
        {
            registryHostInit.Initialize(hostUrl);
        }

        return host;
    }
}