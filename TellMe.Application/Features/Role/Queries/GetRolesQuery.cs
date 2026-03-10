using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.Role.Queries
{
    public class GetRolesQuery : IRequest<Result<List<RoleBriefDto>>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public GetRolesQuery() { }
        public GetRolesQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
        }
    }

    public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
    {
        public Guid Id { get; set; }
        public GetRoleByIdQuery(Guid id) => Id = id;
    }

   
}