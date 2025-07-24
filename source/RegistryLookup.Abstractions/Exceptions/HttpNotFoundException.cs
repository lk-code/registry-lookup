namespace dev.lkcode.RegistryLookup.Abstractions.Exceptions;

public class HttpNotFoundException(string url, string message, Exception internalException)
    : Exception(message, internalException)
{
    public string Url { get; } = url;
}
