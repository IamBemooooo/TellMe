using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Core.Common;

namespace TellMe.Application.Features.User.Commands
{
    public class DeleteUserCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }
}
