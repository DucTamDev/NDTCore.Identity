using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

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
    private readonly ILogger<ClaimService> _logger;

    public ClaimService(
        IUserClaimRepository userClaimRepository,
        IRoleClaimRepository roleClaimRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<ClaimService> logger)
    {
        _userClaimRepository = userClaimRepository;
        _roleClaimRepository = roleClaimRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    // User Claims
    public async Task<Result<List<UserClaimDto>>> GetUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result<List<UserClaimDto>>.NotFound($"User with ID '{userId}' was not found");

            var claims = await _userClaimRepository.GetClaimsByUserIdAsync(userId, cancellationToken);
            var dtos = claims.Select(MapToUserClaimDto).ToList();
            return Result<List<UserClaimDto>>.Success(dtos, "User claims retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claims: {UserId}", userId);
            return Result<List<UserClaimDto>>.InternalError("An error occurred while retrieving user claims");
        }
    }

    public async Task<Result<UserClaimDto>> GetUserClaimByIdAsync(int claimId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim == null)
                return Result<UserClaimDto>.NotFound($"User claim with ID '{claimId}' was not found");

            var dto = MapToUserClaimDto(claim);
            return Result<UserClaimDto>.Success(dto, "User claim retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claim: {ClaimId}", claimId);
            return Result<UserClaimDto>.InternalError("An error occurred while retrieving the user claim");
        }
    }

    public async Task<Result<UserClaimDto>> AddUserClaimAsync(Guid userId, CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result<UserClaimDto>.NotFound($"User with ID '{userId}' was not found");

            // Use UserManager to add claim (for Identity framework consistency)
            var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, claim);
            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get the created claim
            var createdClaim = await _userClaimRepository.GetUserClaimAsync(userId, request.ClaimType, request.ClaimValue, cancellationToken);
            if (createdClaim != null)
            {
                var dto = MapToUserClaimDto(createdClaim);
                return Result<UserClaimDto>.Created(dto, "Claim added successfully");
            }

            // Fallback DTO
            var fallbackDto = new UserClaimDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            return Result<UserClaimDto>.Created(fallbackDto, "Claim added successfully");
        }
        catch (ConflictException ex)
        {
            return Result<UserClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user claim");
            return Result<UserClaimDto>.InternalError("An error occurred while adding the claim");
        }
    }

    public async Task<Result<UserClaimDto>> UpdateUserClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var oldClaim = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (oldClaim == null)
                return Result<UserClaimDto>.NotFound($"User claim with ID '{claimId}' was not found");

            var user = await _userRepository.GetByIdAsync(oldClaim.UserId, cancellationToken);
            if (user == null)
                return Result<UserClaimDto>.NotFound($"User with ID '{oldClaim.UserId}' was not found");

            // Remove old claim and add new claim
            var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
            var removeResult = await _userManager.RemoveClaimAsync(user, oldClaimObj);
            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, newClaim);
            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get updated claim
            var updatedClaim = await _userClaimRepository.GetUserClaimAsync(user.Id, request.ClaimType, request.ClaimValue, cancellationToken);
            if (updatedClaim != null)
            {
                var dto = MapToUserClaimDto(updatedClaim);
                return Result<UserClaimDto>.Success(dto, "Claim updated successfully");
            }

            return Result<UserClaimDto>.InternalError("Failed to retrieve updated claim");
        }
        catch (ConflictException ex)
        {
            return Result<UserClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user claim: {ClaimId}", claimId);
            return Result<UserClaimDto>.InternalError("An error occurred while updating the claim");
        }
    }

    public async Task<Result> RemoveUserClaimAsync(int claimId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _userClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim == null)
                return Result.NotFound($"User claim with ID '{claimId}' was not found");

            var user = await _userRepository.GetByIdAsync(claim.UserId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{claim.UserId}' was not found");

            var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
            var removeResult = await _userManager.RemoveClaimAsync(user, claimObj);
            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Claim removed successfully");
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
            _logger.LogError(ex, "Error removing user claim: {ClaimId}", claimId);
            return Result.InternalError("An error occurred while removing the claim");
        }
    }

    // Role Claims
    public async Task<Result<List<RoleClaimDto>>> GetRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result<List<RoleClaimDto>>.NotFound($"Role with ID '{roleId}' was not found");

            var claims = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);
            var dtos = claims.Select(MapToRoleClaimDto).ToList();
            return Result<List<RoleClaimDto>>.Success(dtos, "Role claims retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claims: {RoleId}", roleId);
            return Result<List<RoleClaimDto>>.InternalError("An error occurred while retrieving role claims");
        }
    }

    public async Task<Result<RoleClaimDto>> GetRoleClaimByIdAsync(int claimId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim == null)
                return Result<RoleClaimDto>.NotFound($"Role claim with ID '{claimId}' was not found");

            var dto = MapToRoleClaimDto(claim);
            return Result<RoleClaimDto>.Success(dto, "Role claim retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claim: {ClaimId}", claimId);
            return Result<RoleClaimDto>.InternalError("An error occurred while retrieving the role claim");
        }
    }

    public async Task<Result<RoleClaimDto>> AddRoleClaimAsync(Guid roleId, CreateClaimRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result<RoleClaimDto>.NotFound($"Role with ID '{roleId}' was not found");

            // Use RoleManager to add claim
            var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _roleManager.AddClaimAsync(role, claim);
            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get the created claim
            var createdClaim = await _roleClaimRepository.GetRoleClaimAsync(roleId, request.ClaimType, request.ClaimValue, cancellationToken);
            if (createdClaim != null)
            {
                var dto = MapToRoleClaimDto(createdClaim);
                return Result<RoleClaimDto>.Created(dto, "Claim added successfully");
            }

            // Fallback DTO
            var fallbackDto = new RoleClaimDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            return Result<RoleClaimDto>.Created(fallbackDto, "Claim added successfully");
        }
        catch (ConflictException ex)
        {
            return Result<RoleClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<RoleClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role claim");
            return Result<RoleClaimDto>.InternalError("An error occurred while adding the claim");
        }
    }

    public async Task<Result<RoleClaimDto>> UpdateRoleClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var oldClaim = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (oldClaim == null)
                return Result<RoleClaimDto>.NotFound($"Role claim with ID '{claimId}' was not found");

            var role = await _roleRepository.GetByIdAsync(oldClaim.RoleId, cancellationToken);
            if (role == null)
                return Result<RoleClaimDto>.NotFound($"Role with ID '{oldClaim.RoleId}' was not found");

            // Remove old claim and add new claim
            var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
            var removeResult = await _roleManager.RemoveClaimAsync(role, oldClaimObj);
            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleClaimDto>.BadRequest(
                    "One or more validation errors occurred",
                    "VALIDATION_ERROR",
                    validationErrors);
            }

            var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _roleManager.AddClaimAsync(role, newClaim);
            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get updated claim
            var updatedClaim = await _roleClaimRepository.GetRoleClaimAsync(role.Id, request.ClaimType, request.ClaimValue, cancellationToken);
            if (updatedClaim != null)
            {
                var dto = MapToRoleClaimDto(updatedClaim);
                return Result<RoleClaimDto>.Success(dto, "Claim updated successfully");
            }

            return Result<RoleClaimDto>.InternalError("Failed to retrieve updated claim");
        }
        catch (ConflictException ex)
        {
            return Result<RoleClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<RoleClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role claim: {ClaimId}", claimId);
            return Result<RoleClaimDto>.InternalError("An error occurred while updating the claim");
        }
    }

    public async Task<Result> RemoveRoleClaimAsync(int claimId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claim = await _roleClaimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim == null)
                return Result.NotFound($"Role claim with ID '{claimId}' was not found");

            var role = await _roleRepository.GetByIdAsync(claim.RoleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{claim.RoleId}' was not found");

            var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
            var removeResult = await _roleManager.RemoveClaimAsync(role, claimObj);
            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    "One or more validation errors occurred",
                    ErrorCodes.ValidationError,
                    validationErrors);
            }

            return Result.Success("Claim removed successfully");
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
            _logger.LogError(ex, "Error removing role claim: {ClaimId}", claimId);
            return Result.InternalError("An error occurred while removing the claim");
        }
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