using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, Result<List<string>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<GetRolePermissionsQueryHandler> _logger;

    public GetRolePermissionsQueryHandler(
        IPermissionRepository permissionRepository,
        RoleManager<AppRole> roleManager,
        ILogger<GetRolePermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<List<string>>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions for role: {RoleId}", request.RoleId);

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role == null)
        {
            return Result<List<string>>.Failure("ROLE_NOT_FOUND", "Role not found");
        }

        var permissions = await _permissionRepository.GetByRoleIdAsync(request.RoleId, cancellationToken);
        var permissionNames = permissions.Select(p => p.Name).ToList();

        return Result<List<string>>.Success(permissionNames);
    }
}

