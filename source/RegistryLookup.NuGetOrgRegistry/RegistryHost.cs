using System.Text.Json;
using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;

namespace dev.lkcode.RegistryLookup.NuGetOrgRegistry;

public class RegistryHost : IRegistryHost
{
    private readonly ILogger<RegistryHost> _logger;
    private readonly IBackendRegistryProvider _registryProvider;

    public Uri Host { get; init; }

    private const string CATALOG_PATH = "/_catalog";

    public RegistryHost(
        ILogger<RegistryHost> logger,
        IBackendRegistryProvider registryProvider,
        Uri hostUrl)
    {
        _logger = logger;
        _registryProvider = registryProvider;
        Host = hostUrl;
    }

    public async Task<HandleLevel> CanHandleHost(CancellationToken cancellationToken)
    {
        return HandleLevel.SUPPORTED;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            IRegistryResult result = await _registryProvider.SendAsync("GET",
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
            IRegistryResult result = await _registryProvider.SendAsync("GET",
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