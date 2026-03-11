using MediatR;
using Microsoft.AspNetCore.Mvc;
using TellMe.Application.Features.Role;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role.Commands;
using TellMe.Application.Features.Role.Queries;

namespace TellMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator; [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetRolesQuery());
            return Ok(result);
        }
        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto)
        {
            var result = await _mediator.Send(new CreateRoleCommand { RoleDto = dto });
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

    

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRoleDto dto)
        {
            var result = await _mediator.Send(new UpdateRoleCommand { RoleDto = dto });
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand { Id = id });
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }

}
