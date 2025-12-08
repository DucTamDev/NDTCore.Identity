using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Authorization.Policies;
using NDTCore.Identity.Contracts.Configuration.Authorization;

namespace NDTCore.Identity.API.Configuration
{
    public class PermissionPolicyConfigurator : IConfigureOptions<AuthorizationOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        public PermissionPolicyConfigurator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(AuthorizationOptions options)
        {
            using var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            var registry = sp.GetRequiredService<IPermissionRegistry>();
            var permissionOptions = sp.GetService<IOptions<PermissionOptions>>()?.Value;

            // Register default permissions
            RegisterUserPermissions(registry);
            RegisterRolePermissions(registry);
            RegisterRoleClaimPermissions(registry);
            RegisterAuthenticationPermissions(registry);
            RegisterSystemAdministrationPermissions(registry);

            // Register modules from appsettings.json
            if (permissionOptions?.Modules != null)
            {
                foreach (var moduleConfig in permissionOptions.Modules)
                {
                    RegisterModuleFromConfiguration(registry, moduleConfig);
                }
            }

            // Configure policies
            var policyBuilder = sp.GetRequiredService<IPolicyBuilder>();
            policyBuilder.ConfigurePolicies(options);
        }

        private static void RegisterModuleFromConfiguration(IPermissionRegistry registry, PermissionModuleConfig moduleConfig)
        {
            var module = PermissionModule.Create(moduleConfig.Name)
                .WithDisplayName(moduleConfig.DisplayName ?? moduleConfig.Name)
                .WithDescription(moduleConfig.Description ?? string.Empty)
                .WithSortOrder(moduleConfig.SortOrder);

            if (moduleConfig.Permissions != null && moduleConfig.Permissions.Any())
            {
                var permissions = new List<PermissionDefinition>();
                foreach (var permConfig in moduleConfig.Permissions)
                {
                    var permBuilder = PermissionDefinition.Create(permConfig.Name)
                        .WithDisplayName(permConfig.DisplayName ?? permConfig.Name)
                        .InModule(moduleConfig.Name)
                        .WithDescription(permConfig.Description ?? string.Empty)
                        .WithSortOrder(permConfig.SortOrder);

                    if (!string.IsNullOrEmpty(permConfig.Group))
                    {
                        permBuilder.InGroup(permConfig.Group);
                    }

                    permissions.Add(permBuilder.Build());
                }

                module.WithPermissions(permissions.ToArray());
            }

            registry.RegisterModule(module.Build());
        }

        private static void RegisterUserPermissions(IPermissionRegistry registry)
        {
            var module = PermissionModule.Create("Users")
                .WithDisplayName("User Management")
                .WithDescription("Permissions for managing users")
                .WithSortOrder(1)
                .WithPermissions(
                    PermissionDefinition.Create("Permissions.Users.View")
                        .WithDisplayName("View Users")
                        .InModule("Users")
                        .WithDescription("View user list and details")
                        .WithSortOrder(1)
                        .InGroup("Basic")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.Create")
                        .WithDisplayName("Create Users")
                        .InModule("Users")
                        .WithDescription("Create new users")
                        .WithSortOrder(2)
                        .InGroup("Basic")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.Edit")
                        .WithDisplayName("Edit Users")
                        .InModule("Users")
                        .WithDescription("Modify existing users")
                        .WithSortOrder(3)
                        .InGroup("Basic")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.Delete")
                        .WithDisplayName("Delete Users")
                        .InModule("Users")
                        .WithDescription("Delete users from the system")
                        .WithSortOrder(4)
                        .InGroup("Basic")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.Lock")
                        .WithDisplayName("Lock Users")
                        .InModule("Users")
                        .WithDescription("Lock user accounts")
                        .WithSortOrder(5)
                        .InGroup("Advanced")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.Unlock")
                        .WithDisplayName("Unlock Users")
                        .InModule("Users")
                        .WithDescription("Unlock user accounts")
                        .WithSortOrder(6)
                        .InGroup("Advanced")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.ResetPassword")
                        .WithDisplayName("Reset User Password")
                        .InModule("Users")
                        .WithDescription("Reset passwords for user accounts")
                        .WithSortOrder(7)
                        .InGroup("Advanced")
                        .Build(),
                    PermissionDefinition.Create("Permissions.Users.ViewSensitiveData")
                        .WithDisplayName("View Sensitive User Data")
                        .InModule("Users")
                        .WithDescription("View sensitive user information (email, phone, etc.)")
                        .WithSortOrder(8)
                        .InGroup("Advanced")
                        .Build()
                )
                .Build();

            registry.RegisterModule(module);
        }

        private static void RegisterRolePermissions(IPermissionRegistry registry)
        {
            var module = PermissionModule.Create("Roles")
                .WithDisplayName("Role Management")
                .WithDescription("Permissions for managing roles")
                .WithSortOrder(2)
                .WithPermissions(
                    PermissionDefinition.Create("Permissions.Roles.View")
                        .WithDisplayName("View Roles")
                        .InModule("Roles")
                        .WithDescription("View role list and details")
                        .WithSortOrder(1)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Roles.Create")
                        .WithDisplayName("Create Roles")
                        .InModule("Roles")
                        .WithDescription("Create new roles")
                        .WithSortOrder(2)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Roles.Edit")
                        .WithDisplayName("Edit Roles")
                        .InModule("Roles")
                        .WithDescription("Modify existing roles")
                        .WithSortOrder(3)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Roles.Delete")
                        .WithDisplayName("Delete Roles")
                        .InModule("Roles")
                        .WithDescription("Delete roles from the system")
                        .WithSortOrder(4)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Roles.AssignToUser")
                        .WithDisplayName("Assign Roles to Users")
                        .InModule("Roles")
                        .WithDescription("Assign roles to user accounts")
                        .WithSortOrder(5)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Roles.RemoveFromUser")
                        .WithDisplayName("Remove Roles from Users")
                        .InModule("Roles")
                        .WithDescription("Remove roles from user accounts")
                        .WithSortOrder(6)
                        .Build()
                )
                .Build();

            registry.RegisterModule(module);
        }

        private static void RegisterRoleClaimPermissions(IPermissionRegistry registry)
        {
            var module = PermissionModule.Create("RoleClaims")
                .WithDisplayName("Role Claims Management")
                .WithDescription("Permissions for managing role claims")
                .WithSortOrder(3)
                .WithPermissions(
                    PermissionDefinition.Create("Permissions.RoleClaims.View")
                        .WithDisplayName("View Role Claims")
                        .InModule("RoleClaims")
                        .WithDescription("View role claims")
                        .WithSortOrder(1)
                        .Build(),
                    PermissionDefinition.Create("Permissions.RoleClaims.Create")
                        .WithDisplayName("Create Role Claims")
                        .InModule("RoleClaims")
                        .WithDescription("Create new role claims")
                        .WithSortOrder(2)
                        .Build(),
                    PermissionDefinition.Create("Permissions.RoleClaims.Delete")
                        .WithDisplayName("Delete Role Claims")
                        .InModule("RoleClaims")
                        .WithDescription("Delete role claims")
                        .WithSortOrder(3)
                        .Build()
                )
                .Build();

            registry.RegisterModule(module);
        }

        private static void RegisterAuthenticationPermissions(IPermissionRegistry registry)
        {
            var module = PermissionModule.Create("Authentication")
                .WithDisplayName("Authentication Management")
                .WithDescription("Permissions for authentication and token management")
                .WithSortOrder(4)
                .WithPermissions(
                    PermissionDefinition.Create("Permissions.Authentication.Login")
                        .WithDisplayName("User Login")
                        .InModule("Authentication")
                        .WithDescription("Authenticate and login to the system")
                        .WithSortOrder(1)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Authentication.RefreshToken")
                        .WithDisplayName("Refresh Token")
                        .InModule("Authentication")
                        .WithDescription("Refresh authentication tokens")
                        .WithSortOrder(2)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Authentication.RevokeToken")
                        .WithDisplayName("Revoke Token")
                        .InModule("Authentication")
                        .WithDescription("Revoke authentication tokens")
                        .WithSortOrder(3)
                        .Build(),
                    PermissionDefinition.Create("Permissions.Authentication.ViewTokens")
                        .WithDisplayName("View Tokens")
                        .InModule("Authentication")
                        .WithDescription("View authentication tokens")
                        .WithSortOrder(4)
                        .Build()
                )
                .Build();

            registry.RegisterModule(module);
        }

        private static void RegisterSystemAdministrationPermissions(IPermissionRegistry registry)
        {
            var module = PermissionModule.Create("SystemAdministration")
                .WithDisplayName("System Administration")
                .WithDescription("Permissions for system-level administration")
                .WithSortOrder(5)
                .WithPermissions(
                    PermissionDefinition.Create("Permissions.SystemAdministration.ViewAuditLogs")
                        .WithDisplayName("View Audit Logs")
                        .InModule("SystemAdministration")
                        .WithDescription("View system audit logs")
                        .WithSortOrder(1)
                        .Build(),
                    PermissionDefinition.Create("Permissions.SystemAdministration.ManageSystemSettings")
                        .WithDisplayName("Manage System Settings")
                        .InModule("SystemAdministration")
                        .WithDescription("Modify system configuration and settings")
                        .WithSortOrder(2)
                        .Build(),
                    PermissionDefinition.Create("Permissions.SystemAdministration.ViewHealthChecks")
                        .WithDisplayName("View Health Checks")
                        .InModule("SystemAdministration")
                        .WithDescription("View system health status and checks")
                        .WithSortOrder(3)
                        .Build()
                )
                .Build();

            registry.RegisterModule(module);
        }
    }
}
