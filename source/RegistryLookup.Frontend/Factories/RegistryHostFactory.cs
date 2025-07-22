using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.Frontend.Factories;

public class RegistryHostFactory(
    ILoggerFactory LoggerFactory,
    IBackendRegistryProvider BackendRegistryProvider)
    : IRegistryHostFactory
{
    public static readonly Type[] REGISTRY_HOST_TYPES =
    [
        typeof(RegistryLookup.DockerRegistryV2.RegistryHost),
        typeof(RegistryLookup.DockerHubRegistry.RegistryHost),
        typeof(RegistryLookup.NuGetOrgRegistry.RegistryHost),
    ];

    public async Task<IRegistryHost> CreateAsync(Uri hostUrl,
        CancellationToken cancellationToken)
    {
        // var hostChecks = await Task.WhenAll(
        //     REGISTRY_HOST_TYPES.Select(async type =>
        //     {
        //         var loggerType = typeof(ILogger<>).MakeGenericType(type);
        //         var createLoggerMethod = typeof(LoggerFactoryExtensions)
        //             .GetMethod("CreateLogger", new[] { typeof(ILoggerFactory) })!
        //             .MakeGenericMethod(type);
        //
        //         var logger = createLoggerMethod.Invoke(null, new object[] { LoggerFactory });
        //         var instance = (IRegistryHost)Activator.CreateInstance(type, logger, BackendRegistryProvider, hostUrl)!;
        //         var result = await instance.CanHandleHost(cancellationToken);
        //         return (result, instance);
        //     })
        // );
        //
        // var sortedResults = hostChecks.OrderBy(x => x.result).ToList();
        // var responsibleRegistry = sortedResults.FirstOrDefault(x => x.result == HandleLevel.SUPPORTED);
        //
        // if (responsibleRegistry is null)
        // {
        //     responsibleRegistry = sortedResults.FirstOrDefault(x => x.result == HandleLevel.UNKNOWN);
        // }

        // You can return or process `sortedResults` as needed
        return null;
    }
}