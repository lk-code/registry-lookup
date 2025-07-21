using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.DockerRegistryV2;

namespace dev.lkcode.RegistryLookup.Frontend.Factories;

public class RegistryHostFactory(ILoggerFactory LoggerFactory,
    IBackendRegistryProvider BackendRegistryProvider)
    : IRegistryHostFactory
{
    public IRegistryHost Create(Uri hostUrl)
    {
        var logger = LoggerFactory.CreateLogger<RegistryHost>();

        return new RegistryHost(logger,
            BackendRegistryProvider,
            hostUrl);
    }
}