using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public class Image : IRegistryEntry
{
    public string Name { get; }
}