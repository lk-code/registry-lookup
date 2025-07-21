namespace dev.lkcode.RegistryLookup.Backend.Endpoints.Responses;

public record RegistryResponse(int HttpStatusCode, string Content, long RequestElapsedMilliseconds);