using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;

namespace TellMe.Application.Features.Permission.Queries
{
    public class GetUserPermissionsQuery : IRequest<List<PermissionDto>>
    {
        public Guid UserId { get; set; }
    }
}
