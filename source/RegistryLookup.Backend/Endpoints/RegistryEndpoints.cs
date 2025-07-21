using dev.lkcode.RegistryLookup.Backend.Endpoints.Responses;
using Microsoft.AspNetCore.Mvc;

namespace dev.lkcode.RegistryLookup.Backend.Endpoints;

public static class RegistryEndpoints
{
    public static WebApplication MapRegistryEndpoints(this WebApplication app)
    {
        app.MapGet("/registry/proxy", async ([FromQuery] string httpMethod,
            [FromQuery] string registryHost) =>
        {
            HttpClient client = new();

            try
            {
                HttpResponseMessage response = httpMethod.ToLowerInvariant() switch
                {
                    "get" => await client.GetAsync(registryHost),
                    "post" => await client.PostAsync(registryHost, null),
                    "put" => await client.PutAsync(registryHost, null),
                    "delete" => await client.DeleteAsync(registryHost),
                    "head" => await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, registryHost)),
                    "options" => await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, registryHost)),
                    "patch" => await client.PatchAsync(registryHost, null),
                    _ => throw new NotSupportedException($"HTTP method '{httpMethod}' is not supported.")
                };
                string content = await response.Content.ReadAsStringAsync();
                
                return new RegistryResponse((int)response.StatusCode!, content);
            }
            catch (HttpRequestException err)
            {
                return new RegistryResponse((int)err.StatusCode!, err.Message);
            }
            catch (Exception err)
            {
                return new RegistryResponse(999, err.Message);
            }
        });
        
        return app;
    }
}