using AutoMapper;
using NDTCore.Identity.Contracts.DTOs.Auth;
using NDTCore.Identity.Contracts.DTOs.Roles;
using NDTCore.Identity.Contracts.DTOs.Users;
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
        }
    }
}
