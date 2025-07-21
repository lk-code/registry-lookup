namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IBackendProvider
{
    Task<bool> IsBackendAvailable(CancellationToken cancellationToken = default);
}