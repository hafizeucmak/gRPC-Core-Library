using LibraryManagement.WebApi.GrpcClients.Borrows;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagement.WebApi.Area.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServiceClient _borrowServiceClient;

        public UsersController(IUserServiceClient borrowServiceClient)
        {
            _borrowServiceClient = borrowServiceClient;
        }

        [HttpGet("getAllUsers")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]

        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
           throw new NotImplementedException();
        }
    }
}