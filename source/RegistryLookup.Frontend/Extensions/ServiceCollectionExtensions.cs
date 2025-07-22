using dev.lkcode.RegistryLookup.Abstractions;
using dev.lkcode.RegistryLookup.Frontend.Factories;
using dev.lkcode.RegistryLookup.Frontend.Provider;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using RegistryLookup.Client;

namespace dev.lkcode.RegistryLookup.Frontend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrontendServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IRegistryHostFactory, RegistryHostFactory>();
        services.AddTransient<IBackendProvider, BackendProvider>();
        services.AddTransient<IBackendRegistryProvider, BackendRegistryProvider>();
        
        // add registry hosts
        
        // add kiota client
        services.AddSingleton<IAuthenticationProvider, AnonymousAuthenticationProvider>();
        services.AddSingleton<IRequestAdapter>(sp =>
        {
            string? backendHost = configuration["Backend:Host"];
            if (string.IsNullOrEmpty(backendHost))
            {
                throw new ArgumentNullException("Backend:Host is not configured in appsettings.");
            }
            IAuthenticationProvider authProvider = sp.GetRequiredService<IAuthenticationProvider>();
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(backendHost)
            };
            return new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
        });
        services.AddScoped<RestClient>(sp =>
        {
            IRequestAdapter adapter = sp.GetRequiredService<IRequestAdapter>();
            return new RestClient(adapter);
        });
        
        return services;
    }
}