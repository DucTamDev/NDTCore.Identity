using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.Users.Services;

/// <summary>
/// Service for handling user management operations
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper,
        IAuditService auditService,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return Result<UserDto>.NotFound($"User with ID '{id}' was not found");

            var roles = await _userRepository.GetUserRolesAsync(id, cancellationToken);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles;

            return Result<UserDto>.Success(userDto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
            return Result<UserDto>.InternalError("An error occurred while retrieving the user");
        }
    }

    public async Task<Result<PaginatedCollection<UserDto>>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedUsers = await _userRepository.GetAllAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchTerm: request.SearchTerm,
                includeDeleted: false,
                cancellationToken);

            var userDtos = new List<UserDto>();

            // Optimize: Load all roles in parallel
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return Result<PaginatedCollection<UserDto>>.InternalError("An error occurred while retrieving users");
        }
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<UserDto>.Conflict("Email already exists");

            var existingUserName = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserName != null)
                return Result<UserDto>.Conflict("Username already exists");

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
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var userDto = _mapper.Map<UserDto>(user);

            // Audit logging
            await _auditService.LogAsync(
                entityType: SystemConstants.EntityTypes.User,
                entityId: user.Id,
                action: SystemConstants.AuditActions.Create,
                newValues: userDto,
                cancellationToken: cancellationToken);

            return Result<UserDto>.Created(userDto, "User created successfully");
        }
        catch (ConflictException ex)
        {
            return Result<UserDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Result<UserDto>.InternalError("An error occurred while creating the user");
        }
    }

    public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return Result<UserDto>.NotFound($"User with ID '{id}' was not found");

            // Store old values for audit
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
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var userDto = _mapper.Map<UserDto>(user);

            // Audit logging
            await _auditService.LogAsync(
                entityType: SystemConstants.EntityTypes.User,
                entityId: user.Id,
                action: SystemConstants.AuditActions.Update,
                oldValues: oldUserDto,
                newValues: userDto,
                cancellationToken: cancellationToken);

            return Result<UserDto>.Success(userDto, "User updated successfully");
        }
        catch (ConflictException ex)
        {
            return Result<UserDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", id);
            return Result<UserDto>.InternalError("An error occurred while updating the user");
        }
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{id}' was not found");

            // Store old values for audit
            var oldUserDto = _mapper.Map<UserDto>(user);

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Audit logging
            await _auditService.LogAsync(
                entityType: SystemConstants.EntityTypes.User,
                entityId: user.Id,
                action: SystemConstants.AuditActions.Delete,
                oldValues: oldUserDto,
                cancellationToken: cancellationToken);

            return Result.Success("User deleted successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", id);
            return Result.InternalError("An error occurred while deleting the user");
        }
    }
}