using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Mappings;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;
using NDTCore.Identity.Infrastructure.Repositories;

namespace NDTCore.Identity.Infrastructure;

/// <summary>
/// Extension methods for configuring infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddRepositories()
            .AddDatabase(configuration)
            .AddIdentityConfiguration()
            .AddMappings()
            .AddMediatRConfiguration();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Generic repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Specialized repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserClaimRepository, UserClaimRepository>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(ConfigureIdentityOptions);

        // Add HTTP context accessor for CurrentUserService
        services.AddHttpContextAccessor();

        // Add infrastructure services
        services.AddScoped<Contracts.Interfaces.Infrastructure.IAuditService, Services.AuditService>();
        services.AddScoped<Contracts.Interfaces.Infrastructure.ICurrentUserService, Services.CurrentUserService>();

        return services;
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(MappingProfile).Assembly
        };

        services.AddAutoMapper(assemblies);
        return services;
    }

    private static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}

