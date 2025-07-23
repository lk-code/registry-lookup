using System.Text.Json;
using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;

namespace dev.lkcode.RegistryLookup.NuGetOrgRegistry;

public class RegistryHost(
    ILogger<RegistryHost> Logger,
    IBackendRegistryProvider RegistryProvider)
    : RegistryBase, IRegistryHost
{
    private const string CATALOG_PATH = "/_catalog";

    public DisplayConfiguration GetDisplayConfiguration() => new("Package", "Packages");

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            IRegistryResult result = await RegistryProvider.SendAsync("GET",
                Host,
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
                Host,
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