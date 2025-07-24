namespace dev.lkcode.RegistryLookup.Abstractions;

public class RegistryResult : IRegistryResult
{
    public int HttpStatusCode { get; init; }
    public string Content { get; init; }
}