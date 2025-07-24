namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IBackendRegistryProvider
{
    Task<IRegistryResult> SendAsync(string httpMethod,
        Uri uri,
        CancellationToken cancellationToken = default);

    Task<IRegistryResult> SendAsync(string httpMethod,
        Uri uri,
        string? endpoint,
        CancellationToken cancellationToken = default);
}