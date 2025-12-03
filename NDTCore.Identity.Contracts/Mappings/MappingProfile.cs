using AutoMapper;
using NDTCore.Identity.Contracts.Features.Authentication.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.Requests;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<AppUser, UserDto>();
            CreateMap<AppUser, UserInfoDto>();
            CreateMap<CreateUserRequest, AppUser>();
            CreateMap<UpdateUserRequest, AppUser>();

            // Role mappings
            CreateMap<AppRole, RoleDto>();
            CreateMap<CreateRoleRequest, AppRole>();
            CreateMap<UpdateRoleRequest, AppRole>();

            // UserRole mappings
            CreateMap<AppUserRole, UserRoleDto>();

            // Claim mappings
            CreateMap<AppUserClaim, UserClaimDto>();
            CreateMap<AppRoleClaim, RoleClaimDto>();
        }
    }
}
