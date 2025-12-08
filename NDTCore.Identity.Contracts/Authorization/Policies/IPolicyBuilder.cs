using Microsoft.AspNetCore.Authorization;

namespace NDTCore.Identity.Contracts.Authorization.Policies;

/// <summary>
/// Interface for building authorization policies
/// </summary>
public interface IPolicyBuilder
{
    /// <summary>
    /// Configures authorization options with all policies
    /// </summary>
    void ConfigurePolicies(AuthorizationOptions options);
}

