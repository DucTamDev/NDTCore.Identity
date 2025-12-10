using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    public DeleteRoleCommandHandler(
        RoleManager<AppRole> roleManager,
        IRoleRepository roleRepository,
        IMapper mapper,
        IAuditService auditService,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting role: {RoleId}", request.RoleId);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return Result.NotFound($"Role with ID '{request.RoleId}' was not found");

        var oldRoleDto = _mapper.Map<RoleDto>(role);

        // Soft delete by just deleting the role
        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());

            return Result.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors);
        }

        await _auditService.LogAsync(
            SystemConstants.EntityTypes.Role,
            role.Id,
            SystemConstants.AuditActions.Delete,
            oldValues: oldRoleDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Role deleted successfully: {RoleId}", request.RoleId);

        return Result.Success("Role deleted successfully");
    }
}

