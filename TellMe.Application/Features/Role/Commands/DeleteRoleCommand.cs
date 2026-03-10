using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Core.Common;

namespace TellMe.Application.Features.Role.Commands
{
    public class DeleteRoleCommand : IRequest<Result<bool>> { public Guid Id { get; set; } }
}
