namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHostFactory
{
    Task<IRegistryHost?> CreateAsync(string registry, Uri hostUrl, CancellationToken cancellationToken);
}