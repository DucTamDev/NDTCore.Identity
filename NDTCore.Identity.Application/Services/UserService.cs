using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.DTOs.Users;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Contracts.Responses;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return ApiResponse<UserDto>.FailureResponse("User not found", 404);

        var roles = await _userRepository.GetUserRolesAsync(id, cancellationToken);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles;

        return ApiResponse<UserDto>.SuccessResponse(userDto);
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var pagedUsers = await _userRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        var userDtos = new List<UserDto>();
        foreach (var user in pagedUsers.Items)
        {
            var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = roles;
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
        return ApiResponse<UserDto>.SuccessResponse(userDto, "User created successfully", 201);
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return ApiResponse<UserDto>.FailureResponse("User not found", 404);

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
        return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
    }

    public async Task<ApiResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return ApiResponse.FailureResponse("User not found", 404);

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("User deletion failed", 400, errors);
        }

        return ApiResponse.SuccessResponse("User deleted successfully");
    }
}

