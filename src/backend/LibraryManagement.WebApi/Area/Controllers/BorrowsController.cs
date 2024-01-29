using LibraryManagement.WebApi.GrpcClients.Borrows;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowsController : ControllerBase
    {
        private readonly IBorrowingServiceClient _borrowServiceClient;

        public BorrowsController(IBorrowingServiceClient borrowServiceClient)
        {
            _borrowServiceClient = borrowServiceClient;
        }

        [HttpGet("getMostBorrowBooks")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]

        public async Task<IActionResult> GetMostBorrowBooks(CancellationToken cancellationToken)
        {
            return Ok(await _borrowServiceClient.GetMostBorrowedBooks());
        }
    }
}