using AutoMapper;
using TellMe.Core.Entities;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role;

namespace TellMe.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map entity -> DTO
            CreateMap<User, UserLoginDto>();
            CreateMap<Permission, PermissionDto>();
            CreateMap<Role, CreateRoleDto>();

            // Map command -> entity to simplify creation flows
            CreateMap<CreateRoleCommand, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
