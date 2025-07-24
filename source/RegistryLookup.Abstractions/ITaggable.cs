namespace dev.lkcode.RegistryLookup.Abstractions;

public interface ITaggable
{
    Task<IReadOnlyCollection<string>> GetTagsAsync(CancellationToken cancellationToken = default, params string[] args);
}