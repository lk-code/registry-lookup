using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Core.Extensions;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public class RegistryHost(Uri HostUrl, HttpClient? httpClient = null) : IRegistryHost
{
    public Uri Host { get; } = HostUrl;

    public HttpClient HttpClient { get; } = httpClient ?? new HttpClient
    {
        BaseAddress = HostUrl.TrimEnd('/')
    };

    public RegistryHost(Uri HostUrl) : this(HostUrl, null)
    {
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        // call Uri (https://artifacts.lk-code.dev/v2/) and response should "HTTP 200 OK"

        try
        {
            var response = await HttpClient.GetAsync("/", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception err)
        {
            // Failed to Fetch, etc. (disabled CORS)
            return false;
        } 
    }

    public async Task<IReadOnlyCollection<IRegistryEntry>> GetEntriesAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(10000, cancellationToken);

        return new List<IRegistryEntry>();
    }
}