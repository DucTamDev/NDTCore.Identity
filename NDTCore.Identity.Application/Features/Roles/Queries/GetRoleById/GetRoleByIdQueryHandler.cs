using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;

namespace NDTCore.Identity.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    public GetRoleByIdQueryHandler(
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<GetRoleByIdQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting role by ID: {RoleId}", request.RoleId);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return Result<RoleDto>.NotFound($"Role with ID '{request.RoleId}' was not found");

        var roleDto = _mapper.Map<RoleDto>(role);

        return Result<RoleDto>.Success(roleDto, "Role retrieved successfully");
    }
}

