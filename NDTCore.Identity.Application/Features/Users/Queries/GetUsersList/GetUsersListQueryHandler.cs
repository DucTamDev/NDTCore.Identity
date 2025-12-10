using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;

namespace NDTCore.Identity.Application.Features.Users.Queries.GetUsersList;

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<PaginatedCollection<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersListQueryHandler> _logger;

    public GetUsersListQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersListQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedCollection<UserDto>>> Handle(
        GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting users list - Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var pagedUsers = await _userRepository.GetAllAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            includeDeleted: false,
            cancellationToken);

        var userDtos = new List<UserDto>();
        var roleTasks = pagedUsers.Items.Select(async user =>
        {
            try
            {
                var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
                return new { User = user, Roles = roles };
            }
            catch
            {
                return new { User = user, Roles = new List<string>() };
            }
        });

        var userWithRoles = await Task.WhenAll(roleTasks);

        foreach (var item in userWithRoles)
        {
            var userDto = _mapper.Map<UserDto>(item.User);
            userDto.Roles = item.Roles;
            userDtos.Add(userDto);
        }

        var result = new PaginatedCollection<UserDto>(items: userDtos, pagination: pagedUsers.Metadata);

        return Result<PaginatedCollection<UserDto>>.Success(result, "Users retrieved successfully");
    }
}

