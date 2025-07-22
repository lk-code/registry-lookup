using System.Text.Json;
using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Abstractions.Exceptions;
using Microsoft.Extensions.Logging;

namespace dev.lkcode.RegistryLookup.DockerHubRegistry;

public class RegistryHost : IRegistryHost
{
    private readonly ILogger<RegistryHost> _logger;
    private readonly IBackendRegistryProvider _registryProvider;
    private static readonly string[] ALLOWED_HOSTS = ["hub.docker.com", "docker.io"];
    private static readonly string CATALOG_PATH = "/";

    public Uri Host { get; init; }

    public RegistryHost(
        ILogger<RegistryHost> logger,
        IBackendRegistryProvider registryProvider,
        Uri hostUri)
    {
        _logger = logger;
        _registryProvider = registryProvider;
        Host = hostUri;
    }

    public async Task<HandleLevel> CanHandleHost(CancellationToken cancellationToken)
    {
        if (ALLOWED_HOSTS.Any(x => Host.Host.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
        {
            return HandleLevel.SUPPORTED;
        }

        return HandleLevel.UNSUPPORTED;
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