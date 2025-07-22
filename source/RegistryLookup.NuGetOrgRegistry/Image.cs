using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.NuGetOrgRegistry;

public record Image(string image) : IRegistryEntry
{
    public string Name { get; } = image;
}