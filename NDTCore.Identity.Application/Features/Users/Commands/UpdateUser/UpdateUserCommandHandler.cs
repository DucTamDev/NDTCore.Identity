using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper,
        IAuditService auditService,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.NotFound($"User with ID '{request.UserId}' was not found");

        var oldUserDto = _mapper.Map<UserDto>(user);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;
        user.City = request.City;
        user.State = request.State;
        user.ZipCode = request.ZipCode;
        user.Country = request.Country;
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());

            return Result.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors);
        }

        var userDto = _mapper.Map<UserDto>(user);
        await _auditService.LogAsync(
            SystemConstants.EntityTypes.User,
            user.Id,
            SystemConstants.AuditActions.Update,
            oldValues: oldUserDto,
            newValues: userDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("User updated successfully: {UserId}", request.UserId);

        return Result.Success("User updated successfully");
    }
}

