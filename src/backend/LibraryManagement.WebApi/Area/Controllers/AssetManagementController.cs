using LibraryManagement.WebApi.CQRS.Commands.AssetManagements;
using LibraryManagement.WebApi.CQRS.Queries.AssetManagements;
using LibraryManagement.WebApi.GrpcClients.Assets;
using LibraryManagement.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetManagementController : ControllerBase
    {
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;
        private readonly IMediator _mediator;

        public AssetManagementController(IAssetManagementServiceClient assetManagementServiceClient, IMediator mediator)
        {
            _assetManagementServiceClient = assetManagementServiceClient;
            _mediator = mediator;
        }

        [HttpPost("[controller].createBook")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("[controller].getBookByIsbn")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(BookDetailsDTO))]
        public async Task<IActionResult> GetBookByIsbn([FromQuery] string isbn, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetBookByIsbn(isbn), cancellationToken));
        }
    }
}