using AutoMapper;
using NDTCore.Identity.Contracts.Features.Authentication.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<AppUser, UserDto>();
        CreateMap<AppUser, UserInfoDto>();

        // Role mappings
        CreateMap<AppRole, RoleDto>();

        // UserRole mappings
        CreateMap<AppUserRole, UserRoleDto>();

        // Claim mappings
        CreateMap<AppUserClaim, UserClaimDto>();
        CreateMap<AppRoleClaim, RoleClaimDto>();
    }
}

