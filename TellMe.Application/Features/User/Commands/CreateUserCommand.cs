using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.User.Commands
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public CreateUserDto UserDto { get; set; } = null!;
    }
}
