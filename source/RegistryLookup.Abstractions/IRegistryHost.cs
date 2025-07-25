namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryHost
{
    DisplayConfiguration GetDisplayConfiguration();

    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IRegistryItem>> GetIndexAsync(CancellationToken cancellationToken);
    
    Task<string> GetJsonAsync(string url, CancellationToken cancellationToken);
}