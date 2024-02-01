using LibraryManagement.Common.SeedManagements.Enums;
using LibraryManagement.WebApi.CQRS.Commands.Seeds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebApi.Area.Dev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeedsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Seed")]
        public async Task<IActionResult> Seed(CancellationToken cancellationToken)
        {
            await _mediator.Send(new ExecuteSeedServiceCommand(), cancellationToken);
            return NoContent();
        }
    }
}
