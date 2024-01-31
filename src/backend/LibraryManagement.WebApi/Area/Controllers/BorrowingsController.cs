using LibraryManagement.WebApi.CQRS.Queries.Borrowings;
using LibraryManagement.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BorrowingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("getMostBorrowedBooks")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<MostBorrowedBooksDTO>))]

        public async Task<IActionResult> GetMostBorrowBooks(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetMostBorrowedBooks(), cancellationToken));
        }
    }
}