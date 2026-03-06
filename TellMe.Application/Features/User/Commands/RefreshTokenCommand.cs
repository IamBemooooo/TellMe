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
    public class RefreshTokenCommand : IRequest<Result<LoginResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
