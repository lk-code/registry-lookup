namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IContentProvider
{
    Task<string> GetContentAsync(string file);
}
