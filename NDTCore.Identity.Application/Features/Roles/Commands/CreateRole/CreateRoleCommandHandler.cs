using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(
        RoleManager<AppRole> roleManager,
        IMapper mapper,
        IAuditService auditService,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _roleManager = roleManager;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating role: {RoleName}", request.Name);

        var existingRole = await _roleManager.FindByNameAsync(request.Name);
        if (existingRole != null)
            return Result<Guid>.Conflict($"Role '{request.Name}' already exists");

        var role = new AppRole
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());

            return Result<Guid>.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors);
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        await _auditService.LogAsync(
            SystemConstants.EntityTypes.Role,
            role.Id,
            SystemConstants.AuditActions.Create,
            newValues: roleDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Role created successfully: {RoleId}", role.Id);

        return Result<Guid>.Created(role.Id, "Role created successfully");
    }
}

