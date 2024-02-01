using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands.Seeds
{
    public class ExecuteSeedServiceCommand : IRequest
    {
        public ExecuteSeedServiceCommand()
        {
        }
    }

    public class ExecuteSeedServiceCommandHandler : IRequestHandler<ExecuteSeedServiceCommand>
    {
        private readonly ILogger<ExecuteSeedServiceCommandHandler> _logger;
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public ExecuteSeedServiceCommandHandler(ILogger<ExecuteSeedServiceCommandHandler> logger, IBorrowingServiceClient borrowingServiceClient)
        {
            _logger = logger;
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task Handle(ExecuteSeedServiceCommand command, CancellationToken cancellationToken)
        {
            await _borrowingServiceClient.ExecuteSeedService(new ExecuteSeedRequest());
        }
    }
}
