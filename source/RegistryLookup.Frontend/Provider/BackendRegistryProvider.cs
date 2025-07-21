using dev.lkcode.RegistryLookup.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RegistryLookup.Client;
using RegistryLookup.Client.Models;
using RegistryResponse = RegistryLookup.Client.Models.RegistryResponse;

namespace dev.lkcode.RegistryLookup.Frontend.Provider;

public class BackendRegistryProvider(RestClient RestClient) : IBackendRegistryProvider
{
    public async Task<IRegistryResult> SendAsync(string httpMethod,
        Uri uri,
        CancellationToken cancellationToken)
    {
        return await SendAsync(httpMethod,
            uri,
            null,
            cancellationToken);
    }

    public async Task<IRegistryResult> SendAsync(string httpMethod,
        Uri uri,
        string? endpoint,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Uri requestUri = new(uri.ToString().TrimEnd('/') + endpoint);
            var response = await RestClient.Registry.Proxy.GetAsync(x =>
            {
                x.QueryParameters.RegistryHost = requestUri.ToString();
                x.QueryParameters.HttpMethod = "GET";
            }, cancellationToken);

            return new RegistryResult
            {
                HttpStatusCode = response.HttpStatusCode ?? 0,
                Content = response.Content
            };
        }
        catch (Exception err)
        {
            return new RegistryResult
            {
                HttpStatusCode = 999,
                Content = err.Message
            };
        }
    }
}