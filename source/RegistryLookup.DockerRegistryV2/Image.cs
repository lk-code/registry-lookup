using System.Text.Json;
using dev.lkcode.RegistryLookup.Abstractions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public class Image(RegistryHost parentHost, string image) : IRegistryItem, ITaggable, IManifestable, IPackagable
{
    public IRegistryHost ParentHost { get; } = parentHost;
    
    public string Name { get; } = image;

    public async Task<IReadOnlyCollection<string>> GetTagsAsync(CancellationToken cancellationToken = default, params string[] args)
    {
        string json = await ParentHost.GetJsonAsync($"/{Name}/tags/list", CancellationToken.None);
        
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("tags", out var tagsElement))
        {
            return [];
        }

        if (tagsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        string[] tags = tagsElement
            .EnumerateArray()
            .Where(e => e.ValueKind == JsonValueKind.String)
            .Select(e => e.GetString()!)
            .ToArray();
        
        return tags;
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