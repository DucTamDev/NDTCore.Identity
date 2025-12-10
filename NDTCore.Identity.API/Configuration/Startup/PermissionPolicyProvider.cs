using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Authorization.Requirements;
using NDTCore.Identity.Contracts.Interfaces.Authorization;
using NDTCore.Identity.Contracts.Settings;
using NDTCore.Identity.Domain.Constants.Authorization.Policies;

namespace NDTCore.Identity.API.Configuration.Startup
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        private readonly PermissionSettings _permissionSettings;
        private readonly IPermissionModuleRegistrar _permissionRegistry;

        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options,
            IOptions<PermissionSettings> permissionSettings,
            IPermissionModuleRegistrar permissionRegistry) : base(options)
        {
            _options = options.Value;
            _permissionSettings = permissionSettings.Value;
            _permissionRegistry = permissionRegistry;

            ConfigurePolicies();
        }

        public void ConfigurePolicies()
        {
            RegisterUserPermissions(_permissionRegistry);
            RegisterRolePermissions(_permissionRegistry);
            RegisterRoleClaimPermissions(_permissionRegistry);
            RegisterAuthenticationPermissions(_permissionRegistry);
            RegisterSystemAdministrationPermissions(_permissionRegistry);

            // Register modules from appsettings.json
            if (_permissionSettings?.Modules != null)
            {
                foreach (var moduleConfig in _permissionSettings.Modules)
                {
                    RegisterModuleFromConfiguration(_permissionRegistry, moduleConfig);
                }
            }

            ConfigurePolicies(_options);
        }

        public void ConfigurePolicies(AuthorizationOptions options)
        {
            // Register individual permission policies
            RegisterPermissionPolicies(options);

            // Register composite/named policies
            RegisterCompositePolicies(options);
        }

        private void RegisterPermissionPolicies(AuthorizationOptions options)
        {
            var allPermissions = _permissionRegistry.GetAllPermissions();

            foreach (var permission in allPermissions.Select(p => p.Name))
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }
        }

        private void RegisterCompositePolicies(AuthorizationOptions options)
        {
            // AdminOnly: Has any admin-level permission
            options.AddPolicy(PolicyNames.AdminOnly, policy =>
            {
                var adminPermissions = new List<string>();

                // Get all permissions from Users module
                var usersPermissions = _permissionRegistry.GetModulePermissions("Users");
                adminPermissions.AddRange(usersPermissions.Select(p => p.Name));

                // Get all permissions from Roles module
                var rolesPermissions = _permissionRegistry.GetModulePermissions("Roles");
                adminPermissions.AddRange(rolesPermissions.Select(p => p.Name));

                // Get system administration permissions
                var sysAdminPermissions = _permissionRegistry.GetModulePermissions("SystemAdministration");
                adminPermissions.AddRange(sysAdminPermissions.Select(p => p.Name));

                if (adminPermissions.Any())
                {
                    policy.Requirements.Add(new HasAnyPermissionRequirement(adminPermissions));
                }
            });

            // UserManagement: Has any user management permission
            options.AddPolicy(PolicyNames.UserManagement, policy =>
            {
                var permissions = _permissionRegistry.GetModulePermissions("Users");
                if (permissions.Any())
                {
                    policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
                }
            });

            // RoleManagement: Has any role management permission
            options.AddPolicy(PolicyNames.RoleManagement, policy =>
            {
                var permissions = _permissionRegistry.GetModulePermissions("Roles");
                if (permissions.Any())
                {
                    policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
                }
            });

            // SystemAdministration: Has any system admin permission
            options.AddPolicy(PolicyNames.SystemAdministration, policy =>
            {
                var permissions = _permissionRegistry.GetModulePermissions("SystemAdministration");
                if (permissions.Any())
                {
                    policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
                }
            });

            // AuthenticationManagement: Has any authentication permission
            options.AddPolicy(PolicyNames.AuthenticationManagement, policy =>
            {
                var permissions = _permissionRegistry.GetModulePermissions("Authentication");
                if (permissions.Any())
                {
                    policy.Requirements.Add(new HasAnyPermissionRequirement(permissions.Select(p => p.Name)));
                }
            });
        }

        private static void RegisterModuleFromConfiguration(IPermissionModuleRegistrar registry, PermissionModuleConfig moduleConfig)
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


        private static void RegisterUserPermissions(IPermissionModuleRegistrar registry)
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

        private static void RegisterRolePermissions(IPermissionModuleRegistrar registry)
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

        private static void RegisterRoleClaimPermissions(IPermissionModuleRegistrar registry)
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

        private static void RegisterAuthenticationPermissions(IPermissionModuleRegistrar registry)
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

        private static void RegisterSystemAdministrationPermissions(IPermissionModuleRegistrar registry)
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