namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHost
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    
    Task<IReadOnlyCollection<IRegistryItem>> GetEntriesAsync(CancellationToken cancellationToken);
    
    DisplayConfiguration GetDisplayConfiguration();
}