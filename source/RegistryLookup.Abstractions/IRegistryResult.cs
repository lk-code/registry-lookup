namespace dev.lkcode.RegistryLookup.Abstractions;

public interface IRegistryResult
{
    int HttpStatusCode { get; init; }
    string Content { get; init; }
}