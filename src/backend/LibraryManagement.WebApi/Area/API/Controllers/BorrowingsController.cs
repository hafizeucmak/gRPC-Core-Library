using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;
using LibraryManagement.WebApi.CQRS.Commands.Borrowings;
using LibraryManagement.WebApi.CQRS.Queries.Borrowings;
using LibraryManagement.WebApi.Models;
using LibraryManagement.WebApi.Swagger;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.API.Controllers
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

        [HttpGet("getBorrowedBooksByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerator<BorrowedBooksByUserDTO>))]

        public async Task<IActionResult> GetBorrowedBooksByUser([FromQuery] DateTime startDate,
                                                                               [FromQuery] DateTime endDate,
                                                                               [FromQuery] string userEmail,
                                                                               CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetBorrowedBooksByUser(startDate, endDate, userEmail), cancellationToken));
        }

        /// <summary>
        /// Retrieves the most borrowed books with support for filtering, sorting, and pagination.
        /// </summary>
        /// <remarks>
        /// Example queries:
        /// - Filter by author name, sort by borrowed count (descending), and paginate:  
        ///   dqb=&o=Equals&p=authorName&v={authorName}&s=borrowedCount,desc&offset=0&count=5
        ///
        /// - Pagination only:  
        ///   dqb=&offset=0&count=10
        ///
        /// - Sort and paginate:  
        ///   dqb=&s=borrowedCount,desc&offset=10&count=10
        /// </remarks>
        [DynamicQuery]
        [HttpGet("getMostBorrowedBooks")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DynamicQueryOutputDTO<MostBorrowedBooksDTO>))]

        public async Task<IActionResult> GetMostBorrowBooks([FromQuery] DynamicQueryOptions dynamicQueryOptions, CancellationToken cancellationToken)
        {
            var results = await _mediator.Send(new GetMostBorrowedBooks(dynamicQueryOptions), cancellationToken);
            return Ok(new DynamicQueryOutputDTO<MostBorrowedBooksDTO>(dynamicQueryOptions, results));
        }

        /// <summary>
        /// Retrieves the users who have borrowed the most books within a given time frame.
        /// </summary>
        /// <remarks>
        /// Example query:
        /// GET /getTopBorrowersWithinSpecifiedTimeframe?startDate=2025-01-01&endDate=2025-03-01
        /// → Returns the users who borrowed the most books between January 1, 2025 and March 1, 2025.
        /// </remarks>
        [HttpGet("getTopBorrowersWithinSpecifiedTimeframe")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TopBorrowersWithinTimeframeDTO))]

        public async Task<IActionResult> GetTopBorrowersWithinSpecifiedTimeframe([FromQuery][SwaggerInclude] DateTime startDate,
                                                                                 [FromQuery] DateTime endDate,
                                                                                 CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetTopBorrowersWithinSpecifiedTimeframe(startDate, endDate), cancellationToken));
        }

        /// <summary>
        /// Retrieves the list of books that were also borrowed by users who borrowed the specified book.
        /// </summary>
        [DynamicQuery]
        [HttpGet("getBorrowersAlsoBorrowedBooks")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DynamicQueryOutputDTO<AlsoBorrowedBooksDTO>))]

        public async Task<IActionResult> GetBorrowersAlsoBorrowedBooks([FromQuery] DynamicQueryOptions dynamicQueryOptions,
                                                                      [FromQuery][SwaggerInclude] string isbn,
                                                                      CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetBorrowersAlsoBorrowedBooks(dynamicQueryOptions, isbn), cancellationToken));
        }

        /// <summary>
        /// Estimates the average reading pace of users (in pages per day) based on the borrow and return duration of books,
        /// assuming continuous reading.
        /// </summary>
        /// <remarks>
        /// Example query with pagination only :
        /// GET /getAverageReadRatesOfUsers?dqb=&offset=0&count=5  
        /// → Returns the first 5 users' reading pace results, without any filtering or sorting.
        /// </remarks>
        /// /// <remarks>
        /// Example query with userName filter :
        /// GET /getAverageReadRatesOfUsers?dqb=&o=Equals&p=userName&v=Ronnie Jacobs&s=readRate,desc&offset=0&count=5  
        /// → Returns the top 5 reading pace results for user "Ronnie Jacobs", sorted by read rate (pages per day) in descending order.
        /// </remarks>
        [DynamicQuery]
        [HttpGet("getAverageReadRatesOfUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DynamicQueryOutputDTO<IEnumerable<ReadRatesOfUsersOutputDTO>>))]
        public async Task<IActionResult> GetAverageReadRatesOfUsers([FromQuery] DynamicQueryOptions dynamicQueryOptions, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAverageReadRatesOfUsers(dynamicQueryOptions), cancellationToken));
        }


        /// <summary>
        /// Estimates the average reading pace (pages per day) for a specific book based on the borrow and return durations of all users who borrowed it.
        /// </summary>
        /// <remarks>
        /// Example query:
        /// GET /getAverageReadRateForBook?isbn=9781234567890  
        /// → Returns the average reading pace for the book with ISBN 9781234567890.
        /// </remarks>
        [HttpGet("getAverageReadRateForBook")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReadRateForBookOutputDTO))]

        public async Task<IActionResult> GetAverageReadRateForBook([FromQuery] string isbn, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAverageReadRateForBook(isbn), cancellationToken));
        }
    }
}