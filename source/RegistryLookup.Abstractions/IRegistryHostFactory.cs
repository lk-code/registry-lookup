namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHostFactory
{
    Task<IRegistryHost> CreateAsync(Uri hostUrl, CancellationToken cancellationToken);
}