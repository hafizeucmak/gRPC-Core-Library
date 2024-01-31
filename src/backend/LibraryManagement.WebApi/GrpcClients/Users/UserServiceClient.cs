using LibraryManagement.UserGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Users
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly UserGRPCService.UserGRPCServiceClient _userGRPCServiceClient;

        public UserServiceClient(UserGRPCService.UserGRPCServiceClient userGRPCServiceClient)
        {
            _userGRPCServiceClient = userGRPCServiceClient;
        }

        public async Task AddUserAsync(UserAddRequest request)
        {
            await _userGRPCServiceClient.AddUserAsync(request);
        }

        public async Task<UserByEmailResponse> GetUserByEmailAsync(UserByEmailRequest request)
        {
           return await _userGRPCServiceClient.GetUserByEmailAsync(request);
        }
    }
}
