using Grpc.Core;

namespace LibraryManagement.UserGrpcService.Services
{
    public class UserService : UserGRPCService.UserGRPCServiceBase
    {
        private readonly ILogger<UserService> _logger;
        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public override async Task<UserAddResponse> AddUser(UserAddRequest  request, ServerCallContext context)
        {
            return await Task.FromResult(new UserAddResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new UserUpdateResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<UserDeleteResponse> DeleteUser(UserDeleteRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new UserDeleteResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<UsersResponse> GetAllUser(UsersRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new UsersResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<UserByIdResponse> GetUserById(UserByIdRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new UserByIdResponse
            {
                Message = "Hello " + request.UserId
            });
        }
    }
}