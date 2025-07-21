namespace dev.lkcode.RegistryLookup.Backend.Endpoints;

public static class VersionEndpoints
{
    public static WebApplication MapVersionEndpoints(this WebApplication app)
    {
        app.MapGet("/version", (IConfiguration configuration) => configuration.GetSection("Version").Value)
            .WithName("GetBackendVersion");

        return app;
    }
}