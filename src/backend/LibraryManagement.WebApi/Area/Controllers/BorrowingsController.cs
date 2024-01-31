using LibraryManagement.WebApi.CQRS.Commands.Borrowings;
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

        [HttpPost("[controller].borrowBook")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> borrowBook([FromBody] BorrowBookCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpGet("getMostBorrowedBooks")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<MostBorrowedBooksDTO>))]

        public async Task<IActionResult> GetMostBorrowBooks([FromQuery] int expectedMostBorrowBookCount, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetMostBorrowedBooks(expectedMostBorrowBookCount), cancellationToken));
        }

        [HttpGet("getBookCopiesAvailability")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BookCopiesAvailabilityDTO))]

        public async Task<IActionResult> GetBookCopiesAvailability([FromQuery] string isbn, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetBookCopiesAvailability(isbn), cancellationToken));
        }

        [HttpGet("getTopBorrowersWithinSpecifiedTimeframe")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BookCopiesAvailabilityDTO))]

        public async Task<IActionResult> GetTopBorrowersWithinSpecifiedTimeframe([FromQuery] DateTime startDate,
                                                                                 [FromQuery] DateTime endDate,
                                                                                 [FromQuery] int expectedTopBorrowerCount,
                                                                                 CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTopBorrowersWithinSpecifiedTimeframe(startDate, endDate, expectedTopBorrowerCount), cancellationToken));
        }

        [HttpGet("getBorrowedBooksByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerator<BorrowedBooksByUserDTO>))]

        public async Task<IActionResult> GetBorrowedBooksByUser([FromQuery] DateTime startDate,
                                                                                [FromQuery] DateTime endDate,
                                                                                [FromQuery] string userEmail,
                                                                                CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetBorrowedBooksByUser(startDate, endDate, userEmail), cancellationToken));
        }
    }
}