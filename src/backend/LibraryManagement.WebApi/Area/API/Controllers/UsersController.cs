using LibraryManagement.WebApi.CQRS.Commands.Users;
using LibraryManagement.WebApi.CQRS.Queries.Users;
using LibraryManagement.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[controller].createUser")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("[controller].getUserByEmail")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserDetailsDTO))]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserByEmail(email), cancellationToken));
        }
    }
}