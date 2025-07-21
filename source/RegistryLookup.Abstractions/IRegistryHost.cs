namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHost
{
    Uri Host { get; init; }
    
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<IRegistryEntry>> GetEntriesAsync(CancellationToken cancellationToken);
}