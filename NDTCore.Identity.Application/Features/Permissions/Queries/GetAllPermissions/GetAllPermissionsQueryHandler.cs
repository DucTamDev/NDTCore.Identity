using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Authorization;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<List<PermissionDto>>>
{
    private readonly IPermissionModuleRegistrar _permissionRegistry;
    private readonly ILogger<GetAllPermissionsQueryHandler> _logger;

    public GetAllPermissionsQueryHandler(
        IPermissionModuleRegistrar permissionRegistry,
        ILogger<GetAllPermissionsQueryHandler> logger)
    {
        _permissionRegistry = permissionRegistry;
        _logger = logger;
    }

    public async Task<Result<List<PermissionDto>>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all permissions");

        var permissions = _permissionRegistry.GetAllPermissions();
        var permissionDtos = permissions.Select(p => new PermissionDto
        {
            Name = p.Name,
            Module = p.Module,
            Description = p.Description
        }).ToList();

        return await Task.FromResult(Result<List<PermissionDto>>.Success(permissionDtos));
    }
}

