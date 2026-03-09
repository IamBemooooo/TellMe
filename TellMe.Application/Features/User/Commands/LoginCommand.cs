using MediatR;
using TellMe.Application.DTOs;
using TellMe.Core.Common;

namespace TellMe.Application.Features.User.Commands
{
    public class LoginCommand : IRequest<Result<LoginResponseDto>>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }
}
