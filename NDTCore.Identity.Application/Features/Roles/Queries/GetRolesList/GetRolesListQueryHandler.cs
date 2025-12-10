using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Roles.Queries.GetRolesList;

public class GetRolesListQueryHandler : IRequestHandler<GetRolesListQuery, Result<PaginatedCollection<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRolesListQueryHandler> _logger;

    public GetRolesListQueryHandler(
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<GetRolesListQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedCollection<RoleDto>>> Handle(
        GetRolesListQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting roles list - Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var allRoles = await _roleRepository.GetAllAsync(includeSystemRoles: true, cancellationToken);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            allRoles = allRoles.Where(r =>
                r.Name!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.Description != null && r.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Apply pagination
        var totalCount = allRoles.Count;
        var pagedItems = allRoles
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var metadata = new PaginationMetadata(
            currentPage: request.PageNumber,
            pageSize: request.PageSize,
            totalRecords: totalCount);

        var pagedRoles = new PaginatedCollection<AppRole>(items: pagedItems, pagination: metadata);

        var roleDtos = _mapper.Map<List<RoleDto>>(pagedRoles.Items);
        var result = new PaginatedCollection<RoleDto>(items: roleDtos, pagination: pagedRoles.Metadata);

        return Result<PaginatedCollection<RoleDto>>.Success(result, "Roles retrieved successfully");
    }
}

