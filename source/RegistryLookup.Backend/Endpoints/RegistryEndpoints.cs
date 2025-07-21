using System.Diagnostics;
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
            Uri requestUri = new(registryHost);
            HttpClient client = new();

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                HttpResponseMessage response = httpMethod.ToLowerInvariant() switch
                {
                    "get" => await client.GetAsync(requestUri),
                    "post" => await client.PostAsync(requestUri, null),
                    "put" => await client.PutAsync(requestUri, null),
                    "delete" => await client.DeleteAsync(requestUri),
                    "head" => await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, requestUri)),
                    "options" => await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, requestUri)),
                    "patch" => await client.PatchAsync(requestUri, null),
                    _ => throw new NotSupportedException($"HTTP method '{httpMethod}' is not supported.")
                };
                stopwatch.Stop();

                string content = await response.Content.ReadAsStringAsync();

                return new RegistryResponse((int)response.StatusCode!, content, stopwatch.ElapsedMilliseconds);
            }
            catch (HttpRequestException err)
            {
                stopwatch.Stop();
                return new RegistryResponse((int)err.StatusCode!, err.Message, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception err)
            {
                stopwatch.Stop();
                return new RegistryResponse(999, err.Message, stopwatch.ElapsedMilliseconds);
            }
        });

        return app;
    }
}