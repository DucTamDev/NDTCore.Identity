namespace NDTCore.Identity.API.Configuration.Extensions;

/// <summary>
/// IServiceCollection extension methods for configuration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all API services
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add configuration sections
        services.AddMemoryCache();

        // Additional service registrations can be added here

        return services;
    }
}

