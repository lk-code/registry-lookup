using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public record Image(string image) : IRegistryItem
{
    public string Name { get; } = image;
}