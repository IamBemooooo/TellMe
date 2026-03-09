using MediatR;
using Microsoft.AspNetCore.Mvc;
using TellMe.Application.Features.Role;
using TellMe.Application.DTOs;

namespace TellMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto)
        {
            var result = await _mediator.Send(new CreateRoleCommand { RoleDto = dto });
            return Ok(result);
        }
    }
}
