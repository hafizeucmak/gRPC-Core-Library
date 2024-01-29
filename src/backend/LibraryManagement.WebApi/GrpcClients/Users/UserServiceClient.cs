using LibraryManagement.UserGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly UserGRPCService.UserGRPCServiceClient _userGRPCServiceClient;

        public UserServiceClient(UserGRPCService.UserGRPCServiceClient userGRPCServiceClient)
        {
            _userGRPCServiceClient = userGRPCServiceClient;
        }


    }
}
