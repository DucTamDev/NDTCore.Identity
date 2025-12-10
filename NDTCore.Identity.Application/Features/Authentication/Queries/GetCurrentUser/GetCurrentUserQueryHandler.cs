using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;

namespace NDTCore.Identity.Application.Features.Authentication.Queries.GetCurrentUser;

/// <summary>
/// Handler for GetCurrentUserQuery
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<UserDto>.NotFound($"User with ID '{request.UserId}' was not found");
        }

        var roles = await _userRepository.GetUserRolesAsync(request.UserId, cancellationToken);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles;

        _logger.LogInformation("Current user retrieved successfully: {UserId}", request.UserId);

        return Result<UserDto>.Success(userDto, "User retrieved successfully");
    }
}

