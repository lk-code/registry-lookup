using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.Frontend.Factories;

public static class Registries
{
    private static readonly string[] REGISTRY_HOST_TYPES = new[]
    {
        "DOCKER_REGISTRY_V2",
        // "DOCKER_HUB",
        // "NUGET_ORG"
    };

    public static IServiceCollection AddRegistries(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKeyedTransient<IRegistryHost, RegistryLookup.DockerRegistryV2.RegistryHost>("DOCKER_REGISTRY_V2");
        // services.AddKeyedTransient<IRegistryHost, RegistryLookup.DockerHubRegistry.RegistryHost>("DOCKER_HUB");
        // services.AddKeyedTransient<IRegistryHost, RegistryLookup.NuGetOrgRegistry.RegistryHost>("NUGET_ORG");

        return services;
    }

    public static string[] GetRegistryHostTypes() => REGISTRY_HOST_TYPES;
}