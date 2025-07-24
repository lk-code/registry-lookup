namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IManifestable
{
    Task<IReadOnlyCollection<string>> GetManifestsAsync(CancellationToken cancellationToken = default, params string[] args);
}