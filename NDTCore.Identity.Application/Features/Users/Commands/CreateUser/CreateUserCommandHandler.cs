using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        UserManager<AppUser> userManager,
        IMapper mapper,
        IAuditService auditService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user: {Email}", request.Email);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<Guid>.Conflict("Email already exists");

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
            return Result<Guid>.Conflict("Username already exists");

        var user = new AppUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());

            return Result<Guid>.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: errors);
        }

        var userDto = _mapper.Map<UserDto>(user);
        await _auditService.LogAsync(
            SystemConstants.EntityTypes.User,
            user.Id,
            SystemConstants.AuditActions.Create,
            newValues: userDto,
            cancellationToken: cancellationToken);

        _logger.LogInformation("User created successfully: {UserId}", user.Id);

        return Result<Guid>.Created(user.Id, "User created successfully");
    }
}

