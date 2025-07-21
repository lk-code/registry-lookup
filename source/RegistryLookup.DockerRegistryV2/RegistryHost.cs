using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace dev.lkcode.RegistryLookup.DockerRegistryV2;

public class RegistryHost(
    ILogger<RegistryHost> Logger,
    IBackendRegistryProvider RegistryProvider,
    Uri HostUrl) : IRegistryHost
{
    public Uri Host { get; init; } = HostUrl;

    private const string CATALOG_PATH = "/_catalog";

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            IRegistryResult result = await RegistryProvider.SendAsync("GET",
                HostUrl,
                cancellationToken);

            if (result.HttpStatusCode == 999)
            {
                throw new RegistryException(result.Content);
            }

            return result.HttpStatusCode == 200;
        }
        catch (Exception err)
        {
            throw new RegistryException("", err);
        }
    }

    public async Task<IReadOnlyCollection<IRegistryEntry>> GetEntriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            IRegistryResult result = await RegistryProvider.SendAsync("GET",
                HostUrl,
                CATALOG_PATH,
                cancellationToken);

            if (result.HttpStatusCode == 999)
            {
                throw new RegistryException(result.Content);
            }

            try
            {
                using var doc = JsonDocument.Parse(result.Content);
                if (!doc.RootElement.TryGetProperty("repositories", out var repositoriesElement))
                {
                    return [];
                }

                if (repositoriesElement.ValueKind != JsonValueKind.Array)
                {
                    return [];
                }

                string[] repositories = repositoriesElement
                    .EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString()!)
                    .ToArray();

                return repositories.Select(x => new Image(x)).ToList();
            }
            catch (JsonException jsonEx)
            {
                throw new RegistryException("Invalid JSON content.", jsonEx);
            }

            return [];
        }
        catch (Exception err)
        {
            throw new RegistryException("", err);
        }
    }
}