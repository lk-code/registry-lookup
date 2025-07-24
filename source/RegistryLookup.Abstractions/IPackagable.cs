namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IPackagable
{
    Task<IReadOnlyCollection<string>> GetPackagesAsync(CancellationToken cancellationToken = default, params string[] args);
}