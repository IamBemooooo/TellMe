using MediatR;
using Microsoft.AspNetCore.Mvc;
using TellMe.Application.Features.Permission.Queries;

namespace TellMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetALlRolePermissionQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
