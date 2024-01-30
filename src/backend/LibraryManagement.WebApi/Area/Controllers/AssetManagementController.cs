using LibraryManagement.WebApi.CQRS.Commands;
using LibraryManagement.WebApi.GrpcClients.Assets;
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

        [HttpPost("[controller].bookCreate")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}