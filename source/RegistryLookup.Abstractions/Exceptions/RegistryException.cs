namespace dev.lkcode.RegistryLookup.Abstractions.Exceptions;

public class RegistryException(string? message, Exception? inner = null) : Exception(message, inner);