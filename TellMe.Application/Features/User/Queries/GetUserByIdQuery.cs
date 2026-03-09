using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.User.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
