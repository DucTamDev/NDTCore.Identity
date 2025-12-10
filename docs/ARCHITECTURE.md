# 🏗️ NDTCore.Identity - Clean Architecture v2

**Version**: 2.0  
**Date**: December 10, 2025  
**Framework**: .NET 8+  
**Pattern**: Clean Architecture + CQRS + Vertical Slices

---

## 📂 Complete Architecture Structure

```
NDTCore.Identity/
│
├── 📁 docs/                                    # Project documentation
│   ├── ARCHITECTURE.md                         # Architecture overview and principles
│
├── 🎯 NDTCore.Identity.API/                    # Presentation Layer
│   │
│   ├── Controllers/
│   │   │   ├── AuthenticationController.cs     # Handles authentication endpoints (login, register, refresh token)
│   │   │   ├── UsersController.cs              # Handles user management endpoints (CRUD operations)
│   │   │   ├── RolesController.cs              # Handles role management endpoints (CRUD, assignments)
│   │   │   ├── PermissionsController.cs        # Handles permission queries and assignments
│   │   │   ├── UserRolesController.cs          # Handles user-role relationship management
│   │   │   ├── RoleClaimsController.cs         # Handles role claims management
│   │   │   ├── UserClaimsController.cs         # Handles user claims management
│   │   │   └── AuditLogsController.cs          # Handles audit log queries
│   │   │
│   │   │
│   │   └── Base/
│   │       └── ApiControllerBase.cs            # Base controller with common helpers (GetCurrentUserId, etc.)
│   │
│   ├── Middleware/
│   │   ├── ExceptionHandlingMiddleware.cs      # Global exception handling and error response formatting
│   │   ├── RequestLoggingMiddleware.cs         # Request/response logging for auditing
│   │   └── PerformanceMonitoringMiddleware.cs  # Request performance tracking and metrics
│   │
│   ├── Filters/
│   │   ├── ValidationFilter.cs                 # Model validation before controller action
│   │   └── ApiExceptionFilter.cs               # Additional exception filtering layer
│   │
│   ├── Configuration/
│   │   ├── Startup/
│   │   │   ├── AuthenticationConfiguration.cs  # JWT authentication setup
│   │   │   ├── AuthorizationConfiguration.cs   # Policy-based authorization configuration
│   │   │   ├── SwaggerConfiguration.cs         # Swagger/OpenAPI documentation setup
│   │   │   ├── CorsConfiguration.cs            # CORS policy configuration
│   │   │   └── DependencyInjectionConfiguration.cs  # Service registration and DI setup
│   │   │
│   │   └── Extensions/
│   │       ├── ServiceCollectionExtensions.cs  # IServiceCollection extension methods
│   │       └── ApplicationBuilderExtensions.cs # IApplicationBuilder extension methods
│   │
│   ├── HealthChecks/
│   │   └── ApiHealthCheck.cs                   # API health status check
│   │
│   ├── Program.cs                              # Application entry point and host configuration
│   ├── appsettings.json                        # Base application settings
│   ├── appsettings.Development.json            # Development environment settings
│   └── appsettings.Production.json             # Production environment settings
│
├── 🧠 NDTCore.Identity.Application/            # Application Layer - Business Logic
│   │
│   ├── Common/                                 # Shared application components
│   │   │
│   │   ├── Behaviors/                          # MediatR pipeline behaviors
│   │   │   ├── ValidationBehavior.cs           # Automatic request validation using FluentValidation
│   │   │   ├── LoggingBehavior.cs              # Automatic request/response logging
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── ICommand.cs                     # Marker interface for commands (write operations)
│   │   │   ├── ICommandHandler.cs              # Interface for command handlers
│   │   │   ├── IQuery.cs                       # Marker interface for queries (read operations)
│   │   │   └── IQueryHandler.cs                # Interface for query handlers
│   │   │
│   │   ├── Exceptions/
│   │   │   ├── ApplicationException.cs         # Base application exception
│   │   │   ├── ValidationException.cs          # Validation failure exception
│   │   │   └── BusinessRuleViolationException.cs  # Business rule violation exception
│   │   │
│   │   └── Models/
│   │       ├── PaginationParams.cs             # Pagination parameters (page, size)
│   │       └── SortingParams.cs                # Sorting parameters (field, direction)
│   │
│   ├── Features/                               # Feature-based organization (Vertical Slices)
│   │   │
│   │   ├── Authentication/                     # Authentication feature
│   │   │   │
│   │   │   ├── Commands/                       # Write operations
│   │   │   │   ├── Login/
│   │   │   │   │   ├── LoginCommand.cs         # Login request command
│   │   │   │   │   ├── LoginCommandHandler.cs  # Handles user login logic
│   │   │   │   │   └── LoginCommandValidator.cs # Validates login credentials
│   │   │   │   │
│   │   │   │   ├── Register/
│   │   │   │   │   ├── RegisterCommand.cs      # User registration command
│   │   │   │   │   ├── RegisterCommandHandler.cs # Handles user registration
│   │   │   │   │   └── RegisterCommandValidator.cs # Validates registration data
│   │   │   │   │
│   │   │   │   ├── RefreshToken/
│   │   │   │   │   ├── RefreshTokenCommand.cs  # Token refresh command
│   │   │   │   │   ├── RefreshTokenCommandHandler.cs # Handles token refresh
│   │   │   │   │   └── RefreshTokenCommandValidator.cs # Validates refresh token
│   │   │   │   │
│   │   │   │   ├── RevokeToken/
│   │   │   │   │   ├── RevokeTokenCommand.cs   # Token revocation command
│   │   │   │   │   └── RevokeTokenCommandHandler.cs # Handles token revocation
│   │   │   │   │
│   │   │   │   ├── ChangePassword/
│   │   │   │   │   ├── ChangePasswordCommand.cs # Password change command
│   │   │   │   │   ├── ChangePasswordCommandHandler.cs # Handles password change
│   │   │   │   │   └── ChangePasswordCommandValidator.cs # Validates password requirements
│   │   │   │   │
│   │   │   │   ├── ForgotPassword/
│   │   │   │   │   ├── ForgotPasswordCommand.cs # Password reset request command
│   │   │   │   │   ├── ForgotPasswordCommandHandler.cs # Generates password reset token
│   │   │   │   │   └── ForgotPasswordCommandValidator.cs # Validates email
│   │   │   │   │
│   │   │   │   └── ResetPassword/
│   │   │   │       ├── ResetPasswordCommand.cs # Password reset with token command
│   │   │   │       ├── ResetPasswordCommandHandler.cs # Handles password reset
│   │   │   │       └── ResetPasswordCommandValidator.cs # Validates reset token
│   │   │   │
│   │   │   ├── Queries/                        # Read operations
│   │   │   │   ├── GetCurrentUser/
│   │   │   │   │   ├── GetCurrentUserQuery.cs  # Get authenticated user query
│   │   │   │   │   └── GetCurrentUserQueryHandler.cs # Retrieves current user info
│   │   │   │   │
│   │   │   │   └── ValidateToken/
│   │   │   │       ├── ValidateTokenQuery.cs   # Token validation query
│   │   │   │       └── ValidateTokenQueryHandler.cs # Validates JWT token
│   │   │   │
│   │   │   └── Services/
│   │   │       ├── JwtTokenService.cs          # JWT token generation and validation
│   │   │       └── PasswordHashingService.cs   # Password hashing and verification
│   │   │
│   │   ├── Authorization/                      # Authorization and permissions
│   │   │   │
│   │   │   ├── Configuration/
│   │   │   │   ├── IPermissionModule.cs        # Interface for permission module definition
│   │   │   │   ├── PermissionModuleRegistrar.cs # Registers all permission modules
│   │   │   │   │
│   │   │   │   └── Modules/
│   │   │   │       ├── PermissionModuleBase.cs # Base class for permission modules
│   │   │   │       ├── UserPermissionsModule.cs # User management permissions
│   │   │   │       ├── RolePermissionsModule.cs # Role management permissions
│   │   │   │       ├── RoleClaimPermissionsModule.cs # Role claim permissions
│   │   │   │       ├── AuthenticationPermissionsModule.cs # Authentication permissions
│   │   │   │       └── SystemAdministrationPermissionsModule.cs # System admin permissions
│   │   │   │
│   │   │   ├── Handlers/
│   │   │   │   ├── PermissionAuthorizationHandler.cs # Evaluates permission requirements
│   │   │   │   └── RoleAuthorizationHandler.cs # Evaluates role requirements
│   │   │   │
│   │   │   ├── Requirements/
│   │   │   │   ├── PermissionRequirement.cs    # Permission-based authorization requirement
│   │   │   │   └── RoleRequirement.cs          # Role-based authorization requirement
│   │   │   │
│   │   │   ├── Services/
│   │   │   │   ├── PermissionRegistry.cs       # Central permission registry
│   │   │   │   ├── PolicyBuilder.cs            # Dynamic policy builder
│   │   │   │   └── PermissionEvaluationService.cs # Permission evaluation logic
│   │   │   │
│   │   │   └── Queries/
│   │   │       ├── GetUserPermissions/
│   │   │       │   ├── GetUserPermissionsQuery.cs # Get user permissions query
│   │   │       │   └── GetUserPermissionsQueryHandler.cs # Retrieves effective user permissions
│   │   │       │
│   │   │       └── CheckPermission/
│   │   │           ├── CheckPermissionQuery.cs # Check permission query
│   │   │           └── CheckPermissionQueryHandler.cs # Validates user permission
│   │   │
│   │   ├── Users/                              # User management feature
│   │   │   │
│   │   │   ├── Commands/
│   │   │   │   ├── CreateUser/
│   │   │   │   │   ├── CreateUserCommand.cs    # Create user command
│   │   │   │   │   ├── CreateUserCommandHandler.cs # Handles user creation
│   │   │   │   │   └── CreateUserCommandValidator.cs # Validates user data
│   │   │   │   │
│   │   │   │   ├── UpdateUser/
│   │   │   │   │   ├── UpdateUserCommand.cs    # Update user command
│   │   │   │   │   ├── UpdateUserCommandHandler.cs # Handles user update
│   │   │   │   │   └── UpdateUserCommandValidator.cs # Validates update data
│   │   │   │   │
│   │   │   │   ├── DeleteUser/
│   │   │   │   │   ├── DeleteUserCommand.cs    # Soft delete user command
│   │   │   │   │   └── DeleteUserCommandHandler.cs # Handles user deletion
│   │   │   │   │
│   │   │   │   ├── LockUser/
│   │   │   │   │   ├── LockUserCommand.cs      # Lock user account command
│   │   │   │   │   └── LockUserCommandHandler.cs # Handles user lock
│   │   │   │   │
│   │   │   │   ├── UnlockUser/
│   │   │   │   │   ├── UnlockUserCommand.cs    # Unlock user account command
│   │   │   │   │   └── UnlockUserCommandHandler.cs # Handles user unlock
│   │   │   │   │
│   │   │   │   └── ResetPassword/
│   │   │   │       ├── ResetPasswordCommand.cs # Admin password reset command
│   │   │   │       ├── ResetPasswordCommandHandler.cs # Handles admin password reset
│   │   │   │       └── ResetPasswordCommandValidator.cs # Validates reset request
│   │   │   │
│   │   │   ├── Queries/
│   │   │   │   ├── GetUserById/
│   │   │   │   │   ├── GetUserByIdQuery.cs     # Get user by ID query
│   │   │   │   │   └── GetUserByIdQueryHandler.cs # Retrieves user details
│   │   │   │   │
│   │   │   │   ├── GetUsersList/
│   │   │   │   │   ├── GetUsersListQuery.cs    # Get paginated users query
│   │   │   │   │   └── GetUsersListQueryHandler.cs # Retrieves user list with pagination
│   │   │   │   │
│   │   │   │   └── SearchUsers/
│   │   │   │       ├── SearchUsersQuery.cs     # Search users query
│   │   │   │       └── SearchUsersQueryHandler.cs # Searches users by criteria
│   │   │   │
│   │   │   └── Services/
│   │   │       └── UserValidationService.cs    # User-specific business validation
│   │   │
│   │   ├── Roles/                              # Role management feature
│   │   │   │
│   │   │   ├── Commands/
│   │   │   │   ├── CreateRole/
│   │   │   │   │   ├── CreateRoleCommand.cs    # Create role command
│   │   │   │   │   ├── CreateRoleCommandHandler.cs # Handles role creation
│   │   │   │   │   └── CreateRoleCommandValidator.cs # Validates role data
│   │   │   │   │
│   │   │   │   ├── UpdateRole/
│   │   │   │   │   ├── UpdateRoleCommand.cs    # Update role command
│   │   │   │   │   ├── UpdateRoleCommandHandler.cs # Handles role update
│   │   │   │   │   └── UpdateRoleCommandValidator.cs # Validates update data
│   │   │   │   │
│   │   │   │   ├── DeleteRole/
│   │   │   │   │   ├── DeleteRoleCommand.cs    # Delete role command
│   │   │   │   │   └── DeleteRoleCommandHandler.cs # Handles role deletion
│   │   │   │   │
│   │   │   │   ├── AssignRoleToUser/
│   │   │   │   │   ├── AssignRoleToUserCommand.cs # Assign role command
│   │   │   │   │   ├── AssignRoleToUserCommandHandler.cs # Handles role assignment
│   │   │   │   │   └── AssignRoleToUserCommandValidator.cs # Validates assignment
│   │   │   │   │
│   │   │   │   └── RemoveRoleFromUser/
│   │   │   │       ├── RemoveRoleFromUserCommand.cs # Remove role command
│   │   │   │       └── RemoveRoleFromUserCommandHandler.cs # Handles role removal
│   │   │   │
│   │   │   ├── Queries/
│   │   │   │   ├── GetRoleById/
│   │   │   │   │   ├── GetRoleByIdQuery.cs     # Get role by ID query
│   │   │   │   │   └── GetRoleByIdQueryHandler.cs # Retrieves role details
│   │   │   │   │
│   │   │   │   ├── GetRolesList/
│   │   │   │   │   ├── GetRolesListQuery.cs    # Get all roles query
│   │   │   │   │   └── GetRolesListQueryHandler.cs # Retrieves role list
│   │   │   │   │
│   │   │   │   └── GetUserRoles/
│   │   │   │       ├── GetUserRolesQuery.cs    # Get user's roles query
│   │   │   │       └── GetUserRolesQueryHandler.cs # Retrieves roles for user
│   │   │   │
│   │   │   └── Services/
│   │   │       └── RoleValidationService.cs    # Role-specific business validation
│   │   │
│   │   ├── Permissions/                        # Permission queries (read-only)
│   │   │   │
│   │   │   ├── Queries/
│   │   │   │   ├── GetAllPermissions/
│   │   │   │   │   ├── GetAllPermissionsQuery.cs # Get all permissions query
│   │   │   │   │   └── GetAllPermissionsQueryHandler.cs # Retrieves all system permissions
│   │   │   │   │
│   │   │   │   ├── GetPermissionsByModule/
│   │   │   │   │   ├── GetPermissionsByModuleQuery.cs # Get permissions by module query
│   │   │   │   │   └── GetPermissionsByModuleQueryHandler.cs # Retrieves permissions grouped by module
│   │   │   │   │
│   │   │   │   └── GetRolePermissions/
│   │   │   │       ├── GetRolePermissionsQuery.cs # Get role permissions query
│   │   │   │       └── GetRolePermissionsQueryHandler.cs # Retrieves permissions for role
│   │   │   │
│   │   │   └── Services/
│   │   │       └── PermissionCacheService.cs   # Permission caching service
│   │   │
│   │   ├── RoleClaims/                         # Role claims management
│   │   │   │
│   │   │   ├── Commands/
│   │   │   │   ├── AddRoleClaim/
│   │   │   │   │   ├── AddRoleClaimCommand.cs  # Add role claim command
│   │   │   │   │   ├── AddRoleClaimCommandHandler.cs # Handles role claim addition
│   │   │   │   │   └── AddRoleClaimCommandValidator.cs # Validates claim data
│   │   │   │   │
│   │   │   │   └── RemoveRoleClaim/
│   │   │   │       ├── RemoveRoleClaimCommand.cs # Remove role claim command
│   │   │   │       └── RemoveRoleClaimCommandHandler.cs # Handles role claim removal
│   │   │   │
│   │   │   └── Queries/
│   │   │       └── GetRoleClaims/
│   │   │           ├── GetRoleClaimsQuery.cs   # Get role claims query
│   │   │           └── GetRoleClaimsQueryHandler.cs # Retrieves claims for role
│   │   │
│   │   ├── UserClaims/                         # User claims management
│   │   │   │
│   │   │   ├── Commands/
│   │   │   │   ├── AddUserClaim/
│   │   │   │   │   ├── AddUserClaimCommand.cs  # Add user claim command
│   │   │   │   │   ├── AddUserClaimCommandHandler.cs # Handles user claim addition
│   │   │   │   │   └── AddUserClaimCommandValidator.cs # Validates claim data
│   │   │   │   │
│   │   │   │   └── RemoveUserClaim/
│   │   │   │       ├── RemoveUserClaimCommand.cs # Remove user claim command
│   │   │   │       └── RemoveUserClaimCommandHandler.cs # Handles user claim removal
│   │   │   │
│   │   │   └── Queries/
│   │   │       └── GetUserClaims/
│   │   │           ├── GetUserClaimsQuery.cs   # Get user claims query
│   │   │           └── GetUserClaimsQueryHandler.cs # Retrieves claims for user
│   │   │
│   │   └── AuditLogs/                          # Audit log queries (read-only)
│   │       │
│   │       └── Queries/
│   │           ├── GetAuditLogs/
│   │           │   ├── GetAuditLogsQuery.cs    # Get audit logs query
│   │           │   └── GetAuditLogsQueryHandler.cs # Retrieves audit log list
│   │           │
│   │           └── GetUserAuditHistory/
│   │               ├── GetUserAuditHistoryQuery.cs # Get user audit history query
│   │               └── GetUserAuditHistoryQueryHandler.cs # Retrieves user's audit trail
│   │
│   ├── Mappings/
│   │   └── ApplicationMappingProfile.cs        # AutoMapper profile for DTOs to domain mapping
│   │
│   └── DependencyInjection.cs                  # Application layer dependency registration
│
├── 📋 NDTCore.Identity.Contracts/              # Contracts Layer - Shared Definitions
│   │
│   ├── Common/
│   │   │
│   │   ├── Pagination/
│   │   │   ├── PaginatedResult.cs              # Generic paginated result wrapper
│   │   │   ├── PaginationMetadata.cs           # Pagination metadata (total, page info)
│   │   │   └── PaginationHelper.cs             # Pagination calculation utilities
│   │   │
│   │   ├── Responses/
│   │   │   ├── ApiResponse.cs                  # Standard API response wrapper
│   │   │   ├── ApiResponse{T}.cs               # Generic API response with data
│   │   │   ├── ApiErrorResponse.cs             # Error response format
│   │   │   ├── ApiPagedResponse{T}.cs          # Paginated response wrapper
│   │   │   └── ValidationErrorResponse.cs      # Validation error details
│   │   │
│   │   └── Results/
│   │       ├── Result.cs                       # Operation result (success/failure)
│   │       ├── Result{T}.cs                    # Operation result with data
│   │       └── ResultExtensions.cs             # Result helper extensions
│   │
│   ├── Settings/
│   │   ├── JwtSettings.cs                      # JWT configuration settings
│   │   ├── EmailSettings.cs                    # Email service configuration
│   │   ├── CorsSettings.cs                     # CORS policy settings
│   │   ├── RateLimitSettings.cs                # Rate limiting configuration
│   │   ├── PermissionModuleSettings.cs         # Permission module configuration
│   │   ├── TokenValidationSettings.cs          # Token validation rules
│   │   └── SeedSettings.cs                     # Database seeding configuration
│   │
│   ├── Features/
│   │   │
│   │   ├── Authentication/
│   │   │   ├── DTOs/
│   │   │   │   ├── AuthTokenDto.cs             # JWT token data transfer object
│   │   │   │   ├── RefreshTokenDto.cs          # Refresh token DTO
│   │   │   │   └── AuthenticatedUserDto.cs     # Authenticated user information
│   │   │   │
│   │   │   └── Responses/
│   │   │       ├── LoginResponse.cs            # Login operation response
│   │   │       ├── RegisterResponse.cs         # Registration operation response
│   │   │       └── TokenResponse.cs            # Token refresh response
│   │   │
│   │   ├── Authorization/
│   │   │   ├── DTOs/
│   │   │   │   ├── PermissionDto.cs            # Permission data transfer object
│   │   │   │   ├── PermissionModuleDto.cs      # Permission module DTO
│   │   │   │   └── PolicyDto.cs                # Authorization policy DTO
│   │   │   │
│   │   │   └── Responses/
│   │   │       ├── PermissionCheckResponse.cs  # Permission check result
│   │   │       └── UserPermissionsResponse.cs  # User's effective permissions
│   │   │
│   │   ├── Users/
│   │   │   ├── DTOs/
│   │   │   │   ├── UserDto.cs                  # Basic user information
│   │   │   │   ├── UserDetailDto.cs            # Detailed user information
│   │   │   │   └── UserSummaryDto.cs           # User summary for lists
│   │   │   │
│   │   │   └── Responses/
│   │   │       ├── UserCreatedResponse.cs      # User creation result
│   │   │       └── UserUpdatedResponse.cs      # User update result
│   │   │
│   │   ├── Roles/
│   │   │   ├── DTOs/
│   │   │   │   ├── RoleDto.cs                  # Basic role information
│   │   │   │   ├── RoleDetailDto.cs            # Detailed role information
│   │   │   │   └── RoleSummaryDto.cs           # Role summary for lists
│   │   │   │
│   │   │   └── Responses/
│   │   │       ├── RoleCreatedResponse.cs      # Role creation result
│   │   │       └── RoleAssignedResponse.cs     # Role assignment result
│   │   │
│   │   ├── Permissions/
│   │   │   ├── DTOs/
│   │   │   │   └── PermissionListDto.cs        # Permission list item
│   │   │   │
│   │   │   └── Responses/
│   │   │       └── PermissionListResponse.cs   # Permission list result
│   │   │
│   │   ├── RoleClaims/
│   │   │   └── DTOs/
│   │   │       └── RoleClaimDto.cs             # Role claim data transfer object
│   │   │
│   │   ├── UserClaims/
│   │   │   └── DTOs/
│   │   │       └── UserClaimDto.cs             # User claim data transfer object
│   │   │
│   │   └── AuditLogs/
│   │       ├── DTOs/
│   │       │   └── AuditLogDto.cs              # Audit log data transfer object
│   │       │
│   │       └── Responses/
│   │           └── AuditLogListResponse.cs     # Audit log list result
│   │
│   ├── Interfaces/
│   │   │
│   │   ├── Services/
│   │   │   ├── IAuthenticationService.cs       # Authentication service contract
│   │   │   ├── IUserService.cs                 # User service contract
│   │   │   ├── IRoleService.cs                 # Role service contract
│   │   │   ├── IPermissionService.cs           # Permission service contract
│   │   │   ├── IRoleClaimService.cs            # Role claim service contract
│   │   │   ├── IUserClaimService.cs            # User claim service contract
│   │   │   └── ITokenService.cs                # Token service contract
│   │   │
│   │   ├── Repositories/
│   │   │   ├── IUserRepository.cs              # User data access contract
│   │   │   ├── IRoleRepository.cs              # Role data access contract
│   │   │   ├── IPermissionRepository.cs        # Permission data access contract
│   │   │   ├── IRefreshTokenRepository.cs      # Refresh token data access contract
│   │   │   ├── IUserClaimRepository.cs         # User claim data access contract
│   │   │   ├── IRoleClaimRepository.cs         # Role claim data access contract
│   │   │   ├── IUserRoleRepository.cs          # User-role relationship data access
│   │   │   ├── IAuditLogRepository.cs          # Audit log data access contract
│   │   │   └── IUnitOfWork.cs                  # Unit of work pattern contract
│   │   │
│   │   ├── Infrastructure/
│   │   │   ├── IAuditService.cs                # Audit logging service contract
│   │   │   ├── ICurrentUserService.cs          # Current user context service
│   │   │   ├── IEmailService.cs                # Email sending service contract
│   │   │   ├── ICacheService.cs                # Caching service contract
│   │   │   └── IDateTimeProvider.cs            # Date/time provider abstraction
│   │   │
│   │   └── Authorization/
│   │       ├── IPermissionRegistry.cs          # Permission registry contract
│   │       ├── IPolicyBuilder.cs               # Policy builder contract
│   │       └── IPermissionEvaluator.cs         # Permission evaluation contract
│   │
│   └── Helpers/
│       └── ErrorCodeToHttpStatusMapper.cs      # Maps error codes to HTTP status codes
│
├── 🏛️ NDTCore.Identity.Domain/                # Domain Layer - Business Rules
│   │
│   ├── Entities/
│   │   │
│   │   ├── Identity/
│   │   │   ├── AppUser.cs                      # User entity with business logic
│   │   │   ├── AppRole.cs                      # Role entity with business logic
│   │   │   ├── AppUserRole.cs                  # User-role relationship entity
│   │   │   ├── AppUserClaim.cs                 # User claim entity
│   │   │   ├── AppRoleClaim.cs                 # Role claim entity
│   │   │   ├── AppUserLogin.cs                 # External login provider entity
│   │   │   └── AppUserToken.cs                 # User token entity
│   │   │
│   │   ├── Security/
│   │   │   ├── RefreshToken.cs                 # Refresh token entity
│   │   │   ├── Permission.cs                   # Permission entity
│   │   │   └── RolePermission.cs               # Role-permission relationship
│   │   │
│   │   └── Auditing/
│   │       └── AuditLog.cs                     # Audit log entity for tracking changes
│   │
│   ├── Common/
│   │   ├── BaseEntity.cs                       # Base entity with Id and timestamps
│   │   ├── IAggregateRoot.cs                   # Marker interface for aggregate roots
│   │   ├── IAuditableEntity.cs                 # Interface for auditable entities
│   │   ├── ISoftDeletable.cs                   # Interface for soft delete support
│   │   └── ITimestampedEntity.cs               # Interface for timestamp tracking
│   │
│   ├── Constants/
│   │   ├── ClaimTypes.cs                       # Custom claim type constants
│   │   ├── ErrorCodes.cs                       # Domain error code constants
│   │   ├── ValidationMessages.cs               # Validation message constants
│   │   └── SystemDefaults.cs                   # System default values
│   │
│   ├── Authorization/
│   │   ├── PermissionNames.cs                  # Static permission name constants
│   │   ├── PolicyNames.cs                      # Static policy name constants
│   │   └── Modules/
│   │       └── ModuleNames.cs                  # Permission module name constants
│   │
│   ├── Exceptions/
│   │   ├── DomainException.cs                  # Base domain exception
│   │   ├── EntityNotFoundException.cs          # Entity not found exception
│   │   ├── DuplicateEntityException.cs         # Duplicate entity exception
│   │   ├── DomainValidationException.cs        # Domain validation exception
│   │   ├── UnauthorizedAccessException.cs      # Unauthorized access exception
│   │   ├── ForbiddenAccessException.cs         # Forbidden access exception
│   │   ├── RateLimitExceededException.cs       # Rate limit exceeded exception
│   │   ├── InvalidOperationException.cs        # Invalid operation exception
│   │   └── BusinessRuleViolationException.cs   # Business rule violation exception
│   │
│   ├── ValueObjects/
│   │   ├── Email.cs                            # Email value object with validation
│   │   ├── PhoneNumber.cs                      # Phone number value object
│   │   └── FullName.cs                         # Full name value object
│   │
│   ├── Enums/
│   │   ├── UserStatus.cs                       # User status enum (Active, Inactive, Locked)
│   │   ├── RoleType.cs                         # Role type enum (System, Custom)
│   │   ├── PermissionType.cs                   # Permission type enum
│   │   ├── AuditActionType.cs                  # Audit action type enum
│   │   └── TokenStatus.cs                      # Token status enum
│   │
│   └── Events/
│       ├── UserCreatedEvent.cs                 # Domain event when user is created
│       ├── UserLockedEvent.cs                  # Domain event when user is locked
│       ├── RoleAssignedEvent.cs                # Domain event when role is assigned
│       └── PasswordChangedEvent.cs             # Domain event when password changes
│
├── 🔧 NDTCore.Identity.Infrastructure/         # Infrastructure Layer - External Concerns
│   │
│   ├── Persistence/
│   │   │
│   │   ├── Context/
│   │   │   ├── ApplicationDbContext.cs         # Main EF Core database context
│   │   │   └── ApplicationDbContextFactory.cs  # Design-time factory for migrations
│   │   │
│   │   ├── Configurations/
│   │   │   │
│   │   │   ├── Identity/
│   │   │   │   ├── AppUserConfiguration.cs     # User entity configuration
│   │   │   │   ├── AppRoleConfiguration.cs     # Role entity configuration
│   │   │   │   ├── AppUserRoleConfiguration.cs # User-role relationship configuration
│   │   │   │   ├── AppUserClaimConfiguration.cs # User claim configuration
│   │   │   │   └── AppRoleClaimConfiguration.cs # Role claim configuration
│   │   │   │
│   │   │   ├── Security/
│   │   │   │   ├── PermissionConfiguration.cs  # Permission entity configuration
│   │   │   │   ├── RolePermissionConfiguration.cs # Role-permission configuration
│   │   │   │   └── RefreshTokenConfiguration.cs # Refresh token configuration
│   │   │   │
│   │   │   └── Auditing/
│   │   │       └── AuditLogConfiguration.cs    # Audit log entity configuration
│   │   │
│   │   ├── Migrations/
│   │   │   └── [Generated EF Core migrations] # Database migration files
│   │   │
│   │   │
│   │   └── Seeders/
│   │       ├── DatabaseSeeder.cs               # Main database seeder orchestrator
│   │       ├── DefaultRolesSeeder.cs           # Seeds default roles (Admin, User)
│   │       ├── DefaultPermissionsSeeder.cs     # Seeds default permissions
│   │       └── DefaultUsersSeeder.cs           # Seeds default users (admin)
│   │
│   ├── Repositories/
│   │   │
│   │   ├── Base/
│   │   │   ├── RepositoryBase.cs               # Generic repository base class
│   │   │   └── ReadOnlyRepositoryBase.cs       # Read-only repository base
│   │   │
│   │   ├── UserRepository.cs                   # User repository implementation
│   │   ├── RoleRepository.cs                   # Role repository implementation
│   │   ├── PermissionRepository.cs             # Permission repository implementation
│   │   ├── RefreshTokenRepository.cs           # Refresh token repository
│   │   ├── UserClaimRepository.cs              # User claim repository
│   │   ├── RoleClaimRepository.cs              # Role claim repository
│   │   ├── UserRoleRepository.cs               # User-role repository
│   │   ├── AuditLogRepository.cs               # Audit log repository
│   │   └── UnitOfWork.cs                       # Unit of work implementation
│   │
│   ├── Services/
│   │   │
│   │   ├── Infrastructure/
│   │   │   ├── AuditService.cs                 # Audit logging service implementation
│   │   │   ├── CurrentUserService.cs           # Current user context service
│   │   │   ├── DateTimeProvider.cs             # DateTime abstraction implementation
│   │   │   └── MachineTimeProvider.cs          # Machine time provider
│   │   │
│   │   ├── Caching/
│   │   │   ├── MemoryCacheService.cs           # In-memory cache implementation
│   │   │
│   │   ├── Email/
│   │   │   ├── EmailService.cs                 # Email service implementation
│   │
│   ├── Identity/
│   │   │
│   │   ├── Managers/
│   │   │   ├── ApplicationUserManager.cs       # Custom UserManager implementation
│   │   │   ├── ApplicationRoleManager.cs       # Custom RoleManager implementation
│   │   │   └── ApplicationSignInManager.cs     # Custom SignInManager implementation
│   │   │
│   │   ├── Stores/
│   │   │   ├── ApplicationUserStore.cs         # Custom user store
│   │   │   └── ApplicationRoleStore.cs         # Custom role store
│   │   │
│   │   └── Validators/
│   │       ├── CustomPasswordValidator.cs      # Custom password validation rules
│   │       └── CustomUserValidator.cs          # Custom user validation rules
│   │
│   ├── HealthChecks/
│   │   ├── DatabaseHealthCheck.cs              # Database connectivity health check
│   │
│   └── DependencyInjection.cs                  # Infrastructure layer dependency registration
│
├── 🧪 tests/                                   # Test Projects
│   │
│   ├── NDTCore.Identity.ArchitectureTests/     # Architecture compliance tests
│   │   ├── ArchitectureTests.cs                # Tests architecture rules and patterns
│   │   ├── DependencyTests.cs                  # Tests dependency flow
│   │   ├── NamingConventionTests.cs            # Tests naming conventions
│   │   └── LayerTests.cs                       # Tests layer isolation
│   │
│   ├── NDTCore.Identity.UnitTests/             # Unit tests
│   │   │
│   │   ├── Application/
│   │   │   ├── Features/
│   │   │   │   ├── Authentication/
│   │   │   │   │   ├── LoginCommandHandlerTests.cs # Tests login handler
│   │   │   │   │   └── RegisterCommandHandlerTests.cs # Tests register handler
│   │   │   │   │
│   │   │   │   ├── Users/
│   │   │   │   │   ├── CreateUserCommandHandlerTests.cs # Tests user creation
│   │   │   │   │   └── GetUserByIdQueryHandlerTests.cs # Tests user query
│   │   │   │   │
│   │   │   │   └── Roles/
│   │   │   │       └── CreateRoleCommandHandlerTests.cs # Tests role creation
│   │   │   │
│   │   │   └── Behaviors/
│   │   │       ├── ValidationBehaviorTests.cs  # Tests validation pipeline
│   │   │       ├── CachingBehaviorTests.cs     # Tests caching pipeline
│   │   │       └── TransactionBehaviorTests.cs # Tests transaction pipeline
│   │   │
│   │   ├── Domain/
│   │   │   ├── Entities/
│   │   │   │   ├── UserTests.cs                # Tests user entity business logic
│   │   │   │   └── RoleTests.cs                # Tests role entity business logic
│   │   │   │
│   │   │   └── ValueObjects/
│   │   │       ├── EmailTests.cs               # Tests email value object
│   │   │       ├── PhoneNumberTests.cs         # Tests phone number value object
│   │   │       └── FullNameTests.cs            # Tests full name value object
│   │   │
│   │   └── Helpers/
│   │       ├── TestFixture.cs                  # Shared test setup
│   │       └── MockFactory.cs                  # Mock object factory
│   │
│   ├── NDTCore.Identity.IntegrationTests/      # Integration tests
│   │   │
│   │   ├── Features/
│   │   │   ├── Authentication/
│   │   │   │   ├── LoginIntegrationTests.cs    # Tests login flow end-to-end
│   │   │   │   └── RegisterIntegrationTests.cs # Tests registration flow
│   │   │   │
│   │   │   └── Users/
│   │   │       ├── CreateUserIntegrationTests.cs # Tests user creation with DB
│   │   │       └── GetUserIntegrationTests.cs  # Tests user retrieval with DB
│   │   │
│   │   ├── Infrastructure/
│   │   │   ├── Repositories/
│   │   │   │   └── UserRepositoryTests.cs      # Tests user repository
│   │   │   │
│   │   │   └── Services/
│   │   │       └── CacheServiceTests.cs        # Tests cache service
│   │   │
│   │   └── Setup/
│   │       ├── IntegrationTestBase.cs          # Base class for integration tests
│   │       ├── TestDatabaseFactory.cs          # Test database setup
│   │       └── TestWebApplicationFactory.cs    # Test web host setup
│   │
│   ├── NDTCore.Identity.E2ETests/              # End-to-end tests
│   │   │
│   │   ├── Scenarios/
│   │   │   ├── UserRegistrationE2ETests.cs     # Tests complete registration flow
│   │   │   ├── LoginFlowE2ETests.cs            # Tests complete login flow
│   │   │   └── RoleManagementE2ETests.cs       # Tests role management scenarios
│   │   │
│   │   └── Setup/
│   │       └── E2ETestBase.cs                  # Base class for E2E tests
│   │
│   └── NDTCore.Identity.PerformanceTests/      # Load and performance tests
│       │
│       ├── LoadTests/
│       │   ├── LoginLoadTest.cs                # Load test for login endpoint
│       │   ├── GetUsersLoadTest.cs             # Load test for user listing
│       │   └── CreateUserLoadTest.cs           # Load test for user creation
│       │
│       └── Setup/
│           └── PerformanceTestBase.cs          # Base class for performance tests
│
├── 📁 scripts/                                 # Utility scripts
│   ├── build.ps1                               # Build script
│   ├── deploy.ps1                              # Deployment script
│   ├── migrate.ps1                             # Database migration script
│   └── seed-data.ps1                           # Data seeding script
│
├── .gitignore                                  # Git ignore rules
├── .editorconfig                               # Editor configuration
├── Directory.Build.props                       # Shared MSBuild properties
├── Directory.Packages.props                    # Central package management (NuGet)
└── NDTCore.Identity.sln                        # Solution file
```

---

## 📊 Layer Responsibilities

### 🎯 API Layer (Presentation)
**Purpose**: HTTP request handling and response formatting
- Receives HTTP requests
- Routes to appropriate handlers via MediatR
- Formats responses
- Handles authentication/authorization
- Swagger documentation
- Health checks

### 🧠 Application Layer (Business Logic)
**Purpose**: Use cases and application workflows
- Contains all business logic
- CQRS implementation (Commands & Queries)
- Validation using FluentValidation
- Authorization logic
- Pipeline behaviors (logging, caching, validation)
- Application services

### 📋 Contracts Layer (Shared Definitions)
**Purpose**: Shared contracts across layers
- DTOs (Data Transfer Objects)
- Interface definitions
- Response models
- Settings/Configuration
- Enums and constants
- Does NOT contain Commands or Queries (those are in Application)

### 🏛️ Domain Layer (Core Business Rules)
**Purpose**: Core domain model and business rules
- Entities with business logic
- Value Objects
- Domain Events
- Domain Exceptions
- Business rules and invariants
- No dependencies on other layers

### 🔧 Infrastructure Layer (External Concerns)
**Purpose**: External services and data access
- Database context and configurations
- Repository implementations
- External service integrations (Email, Storage)
- Caching implementations
- Identity framework customization
- Background job implementations

---

## 🎯 Key Architecture Principles

1. **Clean Architecture**: Dependency rule - inner layers don't depend on outer layers
2. **CQRS**: Separate read (Queries) and write (Commands) operations
3. **Vertical Slices**: Features organized by business capability
4. **Domain-Driven Design**: Rich domain model with business logic
5. **Single Responsibility**: Each class has one reason to change
6. **Dependency Inversion**: Depend on abstractions, not concretions

---

## 📝 Notes

- Settings are in **Contracts** layer (shared configuration)
- Commands/Queries are in **Application** layer (use cases)
- Each file has clear responsibility
- No code examples - focus on structure and purpose
- Architecture supports testing, scaling, and maintenance

---

**Version**: 2.0 (Final Clean Architecture)  
**Status**: Ready for Implementation  
**Last Updated**: December 10, 2025

