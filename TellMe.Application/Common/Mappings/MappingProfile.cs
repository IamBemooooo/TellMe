using AutoMapper;
using TellMe.Core.Entities;
using TellMe.Application.DTOs;
using System.Linq;
using TellMe.Application.Features.Role.Commands;

namespace TellMe.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Basic mappings
            CreateMap<User, UserLoginDto>();
            CreateMap<Permission, PermissionDto>();
            CreateMap<Permission, PermissionBriefDto>();

            // Role mappings
            CreateMap<Role, CreateRoleDto>();
            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<CreateRoleCommand, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.HasValue ? src.CreatedBy.Value.ToString() : string.Empty));

            CreateMap<Role, RoleBriefDto>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));

            // User mappings (including roles)
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

            CreateMap<User, UserBriefDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));
        }
    }
}
