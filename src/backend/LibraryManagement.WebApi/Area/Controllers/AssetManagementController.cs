using LibraryManagement.WebApi.GTaskClients.Assets;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetManagementController : ControllerBase
    {
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;

        public AssetManagementController(IAssetManagementServiceClient assetManagementServiceClient)
        {
            _assetManagementServiceClient = assetManagementServiceClient;
        }

        [HttpGet("addBookRecord")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]

        public async Task<IActionResult> AddBookRecord(CancellationToken cancellationToken)
        {
            await _assetManagementServiceClient.AddBookRecordAsync();
            throw new NotImplementedException("anani sikim");
        }
    }
}