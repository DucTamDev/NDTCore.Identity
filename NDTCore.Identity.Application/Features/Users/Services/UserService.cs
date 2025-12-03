using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;

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

    public UserService(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper,
        IAuditService auditService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<UserDto>.FailureResponse("User not found", 404);

        var rolesResult = await _userRepository.GetUserRolesAsync(id, cancellationToken);

        var user = userResult.Value!;
        var roles = rolesResult.Value!;
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles;

        return ApiResponse<UserDto>.SuccessResponse(userDto);
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var pagedUsersResult = await _userRepository.GetAllAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            includeDeleted: false,
            cancellationToken);

        if (pagedUsersResult.IsFailure)
            return ApiResponse<PagedResult<UserDto>>.FailureResponse("Failed to retrieve users", 500);

        var pagedUsers = pagedUsersResult.Value!;
        var userDtos = new List<UserDto>();

        // Optimize: Load all roles in parallel
        var roleTasks = pagedUsers.Items.Select(async user =>
        {
            var rolesResult = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
            return new { User = user, Roles = rolesResult.IsSuccess ? rolesResult.Value ?? new List<string>() : new List<string>() };
        });

        var userWithRoles = await Task.WhenAll(roleTasks);

        foreach (var item in userWithRoles)
        {
            var userDto = _mapper.Map<UserDto>(item.User);
            userDto.Roles = item.Roles;
            userDtos.Add(userDto);
        }

        var result = new PagedResult<UserDto>
        {
            Items = userDtos,
            PageNumber = pagedUsers.PageNumber,
            PageSize = pagedUsers.PageSize,
            TotalCount = pagedUsers.TotalCount
        };

        return ApiResponse<PagedResult<UserDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return ApiResponse<UserDto>.FailureResponse("Email already exists", 400);

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
            return ApiResponse<UserDto>.FailureResponse("Username already exists", 400);

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
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserDto>.FailureResponse("User creation failed", 400, errors);
        }

        var userDto = _mapper.Map<UserDto>(user);

        // Audit logging
        await _auditService.LogAsync(
            entityType: "User",
            entityId: user.Id,
            action: "Create",
            newValues: userDto,
            cancellationToken: cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse(userDto, "User created successfully", 201);
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (userResult.IsFailure)
            return ApiResponse<UserDto>.FailureResponse("User not found", 404);

        var user = userResult.Value!;

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
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserDto>.FailureResponse("User update failed", 400, errors);
        }

        var userDto = _mapper.Map<UserDto>(user);

        // Audit logging
        await _auditService.LogAsync(
            entityType: "User",
            entityId: user.Id,
            action: "Update",
            oldValues: oldUserDto,
            newValues: userDto,
            cancellationToken: cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
    }

    public async Task<ApiResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse.FailureResponse("User not found", 404);

        var user = userResult.Value!;

        // Store old values for audit
        var oldUserDto = _mapper.Map<UserDto>(user);

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse(message: "User deletion failed", 400, errors);
        }

        // Audit logging
        await _auditService.LogAsync(
            entityType: "User",
            entityId: user.Id,
            action: "Delete",
            oldValues: oldUserDto,
            cancellationToken: cancellationToken);

        return ApiResponse.SuccessResponse(message: "User deleted successfully");
    }
}