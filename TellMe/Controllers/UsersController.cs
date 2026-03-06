using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TellMe.Application.DTOs;
using TellMe.Application.Features.User.Commands;

namespace TellMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _mediator.Send(new LoginCommand { LoginDto = dto });
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand dto)
        {
            var result = await _mediator.Send(new LogoutCommand
            {
                RefreshToken = dto.RefreshToken
            });

            return Ok(result);
        }
    }
}
