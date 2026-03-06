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
        public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        {
            var id = await _mediator.Send(command);
            var dto = new CreateRoleDto { Id = id, Name = command.Name, Description = command.Description };
            return Ok();
        }
    }
}
