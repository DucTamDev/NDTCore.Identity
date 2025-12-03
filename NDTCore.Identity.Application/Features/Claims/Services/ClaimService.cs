using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Claims.Services;

/// <summary>
/// Service for handling claim management operations
/// </summary>
public class ClaimService : IClaimService
{
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;

    public ClaimService(
        IUserClaimRepository userClaimRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMapper mapper)
    {
        _userClaimRepository = userClaimRepository;
        _roleClaimRepository = roleClaimRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _mapper = mapper;
    }

    // User Claims
    public async Task<ApiResponse<List<UserClaimDto>>> GetUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<List<UserClaimDto>>.FailureResponse("User not found", 404);

        var claimsResult = await _userClaimRepository.GetClaimsByUserIdAsync(userId, cancellationToken);
        if (claimsResult.IsFailure)
            return ApiResponse<List<UserClaimDto>>.FailureResponse("Failed to retrieve user claims", 500);

        var claims = claimsResult.Value!;
        var dtos = claims.Select(MapToUserClaimDto).ToList();
        return ApiResponse<List<UserClaimDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<UserClaimDto>> GetUserClaimByIdAsync(int claimId, CancellationToken cancellationToken = default)
    {
        var claimResult = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse<UserClaimDto>.FailureResponse("Claim not found", 404);

        var dto = MapToUserClaimDto(claimResult.Value!);
        return ApiResponse<UserClaimDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<UserClaimDto>> AddUserClaimAsync(Guid userId, CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<UserClaimDto>.FailureResponse("User not found", 404);

        var user = userResult.Value!;

        // Use UserManager to add claim (for Identity framework consistency)
        var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
        var addResult = await _userManager.AddClaimAsync(user, claim);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserClaimDto>.FailureResponse("Failed to add claim", 400, errors);
        }

        // Get the created claim
        var claimResult = await _userClaimRepository.GetUserClaimAsync(userId, request.ClaimType, request.ClaimValue, cancellationToken);
        if (claimResult.IsSuccess && claimResult.Value != null)
        {
            var dto = MapToUserClaimDto(claimResult.Value);
            return ApiResponse<UserClaimDto>.SuccessResponse(dto, "Claim added successfully", 201);
        }

        // Fallback DTO
        var fallbackDto = new UserClaimDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            ClaimType = request.ClaimType,
            ClaimValue = request.ClaimValue
        };

        return ApiResponse<UserClaimDto>.SuccessResponse(fallbackDto, "Claim added successfully", 201);
    }

    public async Task<ApiResponse<UserClaimDto>> UpdateUserClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var claimResult = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse<UserClaimDto>.FailureResponse("Claim not found", 404);

        var oldClaim = claimResult.Value!;
        var userResult = await _userRepository.GetByIdAsync(oldClaim.UserId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<UserClaimDto>.FailureResponse("User not found", 404);

        var user = userResult.Value!;

        // Remove old claim and add new claim
        var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
        var removeResult = await _userManager.RemoveClaimAsync(user, oldClaimObj);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserClaimDto>.FailureResponse("Failed to update claim", 400, errors);
        }

        var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
        var addResult = await _userManager.AddClaimAsync(user, newClaim);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserClaimDto>.FailureResponse("Failed to update claim", 400, errors);
        }

        // Get updated claim
        var updatedClaimResult = await _userClaimRepository.GetUserClaimAsync(user.Id, request.ClaimType, request.ClaimValue, cancellationToken);
        if (updatedClaimResult.IsSuccess && updatedClaimResult.Value != null)
        {
            var dto = MapToUserClaimDto(updatedClaimResult.Value);
            return ApiResponse<UserClaimDto>.SuccessResponse(dto, "Claim updated successfully");
        }

        return ApiResponse<UserClaimDto>.FailureResponse("Failed to retrieve updated claim", 500);
    }

    public async Task<ApiResponse> RemoveUserClaimAsync(int claimId, CancellationToken cancellationToken = default)
    {
        var claimResult = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse.FailureResponse("Claim not found", 404);

        var claim = claimResult.Value!;
        var userResult = await _userRepository.GetByIdAsync(claim.UserId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse.FailureResponse("User not found", 404);

        var user = userResult.Value!;
        var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
        var removeResult = await _userManager.RemoveClaimAsync(user, claimObj);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Failed to remove claim", 400, errors);
        }

        return ApiResponse.SuccessResponse("Claim removed successfully");
    }

    // Role Claims
    public async Task<ApiResponse<List<RoleClaimDto>>> GetRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<List<RoleClaimDto>>.FailureResponse("Role not found", 404);

        var claimsResult = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);
        if (claimsResult.IsFailure)
            return ApiResponse<List<RoleClaimDto>>.FailureResponse("Failed to retrieve role claims", 500);

        var claims = claimsResult.Value!;
        var dtos = claims.Select(MapToRoleClaimDto).ToList();
        return ApiResponse<List<RoleClaimDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<RoleClaimDto>> GetRoleClaimByIdAsync(int claimId, CancellationToken cancellationToken = default)
    {
        var claimResult = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse<RoleClaimDto>.FailureResponse("Claim not found", 404);

        var dto = MapToRoleClaimDto(claimResult.Value!);
        return ApiResponse<RoleClaimDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<RoleClaimDto>> AddRoleClaimAsync(Guid roleId, CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<RoleClaimDto>.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;

        // Use RoleManager to add claim
        var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
        var addResult = await _roleManager.AddClaimAsync(role, claim);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RoleClaimDto>.FailureResponse("Failed to add claim", 400, errors);
        }

        // Get the created claim
        var claimResult = await _roleClaimRepository.GetRoleClaimAsync(roleId, request.ClaimType, request.ClaimValue, cancellationToken);
        if (claimResult.IsSuccess && claimResult.Value != null)
        {
            var dto = MapToRoleClaimDto(claimResult.Value);
            return ApiResponse<RoleClaimDto>.SuccessResponse(dto, "Claim added successfully", 201);
        }

        // Fallback DTO
        var fallbackDto = new RoleClaimDto
        {
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            ClaimType = request.ClaimType,
            ClaimValue = request.ClaimValue
        };

        return ApiResponse<RoleClaimDto>.SuccessResponse(fallbackDto, "Claim added successfully", 201);
    }

    public async Task<ApiResponse<RoleClaimDto>> UpdateRoleClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default)
    {
        var claimResult = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse<RoleClaimDto>.FailureResponse("Claim not found", 404);

        var oldClaim = claimResult.Value!;
        var roleResult = await _roleRepository.GetByIdAsync(oldClaim.RoleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<RoleClaimDto>.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;

        // Remove old claim and add new claim
        var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
        var removeResult = await _roleManager.RemoveClaimAsync(role, oldClaimObj);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RoleClaimDto>.FailureResponse("Failed to update claim", 400, errors);
        }

        var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
        var addResult = await _roleManager.AddClaimAsync(role, newClaim);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RoleClaimDto>.FailureResponse("Failed to update claim", 400, errors);
        }

        // Get updated claim
        var updatedClaimResult = await _roleClaimRepository.GetRoleClaimAsync(role.Id, request.ClaimType, request.ClaimValue, cancellationToken);
        if (updatedClaimResult.IsSuccess && updatedClaimResult.Value != null)
        {
            var dto = MapToRoleClaimDto(updatedClaimResult.Value);
            return ApiResponse<RoleClaimDto>.SuccessResponse(dto, "Claim updated successfully");
        }

        return ApiResponse<RoleClaimDto>.FailureResponse("Failed to retrieve updated claim", 500);
    }

    public async Task<ApiResponse> RemoveRoleClaimAsync(int claimId, CancellationToken cancellationToken = default)
    {
        var claimResult = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
        if (claimResult.IsFailure)
            return ApiResponse.FailureResponse("Claim not found", 404);

        var claim = claimResult.Value!;
        var roleResult = await _roleRepository.GetByIdAsync(claim.RoleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;
        var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
        var removeResult = await _roleManager.RemoveClaimAsync(role, claimObj);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Failed to remove claim", 400, errors);
        }

        return ApiResponse.SuccessResponse("Claim removed successfully");
    }

    private UserClaimDto MapToUserClaimDto(AppUserClaim claim)
    {
        return new UserClaimDto
        {
            Id = claim.Id,
            UserId = claim.UserId,
            UserName = claim.AppUser?.UserName ?? string.Empty,
            ClaimType = claim.ClaimType ?? string.Empty,
            ClaimValue = claim.ClaimValue ?? string.Empty
        };
    }

    private RoleClaimDto MapToRoleClaimDto(AppRoleClaim claim)
    {
        return new RoleClaimDto
        {
            Id = claim.Id,
            RoleId = claim.RoleId,
            RoleName = claim.AppRole?.Name ?? string.Empty,
            ClaimType = claim.ClaimType ?? string.Empty,
            ClaimValue = claim.ClaimValue ?? string.Empty
        };
    }
}

