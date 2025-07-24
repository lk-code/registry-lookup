using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.NuGetOrgRegistry;

public class Image(IRegistryHost parentHost, string image) : IRegistryItem
{
    public IRegistryHost ParentHost { get; } = parentHost;

    public string Name { get; } = image;
}