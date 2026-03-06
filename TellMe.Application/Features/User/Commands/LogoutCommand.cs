using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Core.Common;

namespace TellMe.Application.Features.User.Commands
{
    public class LogoutCommand : IRequest<Result<Unit>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
