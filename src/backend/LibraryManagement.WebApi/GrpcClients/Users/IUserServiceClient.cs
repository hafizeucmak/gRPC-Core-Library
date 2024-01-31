using LibraryManagement.UserGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Users
{
    public interface IUserServiceClient
    {
        Task AddUserAsync(UserAddRequest request);

        Task<UserByEmailResponse> GetUserByEmailAsync(UserByEmailRequest request);
    }
}
