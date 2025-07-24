using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerHubRegistry;

public record Image(string image) : IRegistryItem
{
    public string Name { get; } = image;
}