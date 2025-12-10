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

namespace NDTCore.Identity.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper,
        IAuditService auditService,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.NotFound($"User with ID '{request.UserId}' was not found");

        var oldUserDto = _mapper.Map<UserDto>(user);

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;

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

        await _auditService.LogAsync(
            SystemConstants.EntityTypes.User,
            user.Id,
            SystemConstants.AuditActions.Delete,
            oldValues: oldUserDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("User deleted successfully: {UserId}", request.UserId);

        return Result.Success("User deleted successfully");
    }
}

