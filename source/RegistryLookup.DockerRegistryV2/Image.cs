using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public class Image(RegistryHost parentHost, string image) : IRegistryItem, ITaggable, IManifestable, IPackagable
{
    public IRegistryHost ParentHost { get; } = parentHost;
    
    public string Name { get; } = image;

    public Task<IReadOnlyCollection<string>> GetTagsAsync(CancellationToken cancellationToken = default, params string[] args)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<string>> GetManifestsAsync(CancellationToken cancellationToken = default, params string[] args)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<string>> GetPackagesAsync(CancellationToken cancellationToken = default, params string[] args)
    {
        throw new NotImplementedException();
    }
}