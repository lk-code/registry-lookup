using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public record Image(string image) : IRegistryEntry
{
    public string Name { get; } = image;
}