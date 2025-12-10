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

namespace NDTCore.Identity.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    public UpdateRoleCommandHandler(
        RoleManager<AppRole> roleManager,
        IRoleRepository roleRepository,
        IMapper mapper,
        IAuditService auditService,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating role: {RoleId}", request.RoleId);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return Result.NotFound($"Role with ID '{request.RoleId}' was not found");

        var oldRoleDto = _mapper.Map<RoleDto>(role);

        // Check if name is changing and if new name already exists
        if (role.Name != request.Name)
        {
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null && existingRole.Id != request.RoleId)
                return Result.Conflict($"Role name '{request.Name}' is already taken");
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());

            return Result.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors);
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        await _auditService.LogAsync(
            SystemConstants.EntityTypes.Role,
            role.Id,
            SystemConstants.AuditActions.Update,
            oldValues: oldRoleDto,
            newValues: roleDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Role updated successfully: {RoleId}", request.RoleId);

        return Result.Success("Role updated successfully");
    }
}

