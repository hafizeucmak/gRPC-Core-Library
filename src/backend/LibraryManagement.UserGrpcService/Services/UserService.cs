using Grpc.Core;
using LibraryManagement.UserGrpcService.Business.CQRS.Commands;
using MediatR;

namespace LibraryManagement.UserGrpcService.Services
{
    public class UserService : UserGRPCService.UserGRPCServiceBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMediator _mediator;
        public UserService(ILogger<UserService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<UserAddResponse> AddUser(UserAddRequest request, ServerCallContext context)
        {
            var command = new CreateUserCommand(request.UserEmail, request.UserName, request.FirstName, request.LastName, request.PhoneNumber);
            return await _mediator.Send(command);
        }

        public override async Task<UserByEmailResponse> GetUserByEmail(UserByEmailRequest request, ServerCallContext context)
        {
            //TODO: implement this method for the check user from API side
            return await Task.FromResult(new UserByEmailResponse
            {
            });
        }
    }
}