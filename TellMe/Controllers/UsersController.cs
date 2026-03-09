using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TellMe.Application.DTOs;
using TellMe.Application.Features.User.Commands;
using TellMe.Application.Features.User.Queries;
using TellMe.Core.Common;

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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Result<ResultWithPaging<UserDto>>>> GetAll([FromQuery] string? username, [FromQuery] Guid? roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllUsersQuery
            {
                Username = username,
                RoleId = roleId,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Result<UserDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Result<UserDto>>> Create([FromBody] CreateUserDto dto)
        {
            var result = await _mediator.Send(new CreateUserCommand { UserDto = dto });

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Result<UserDto>>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var result = await _mediator.Send(new UpdateUserCommand { Id = id, UserDto = dto });

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Result<Unit>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id });

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
