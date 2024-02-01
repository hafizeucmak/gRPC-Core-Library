using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.SeedData;
using LibraryManagement.BorrowingGrpcService.Data.SeedData.DbHandler;
using LibraryManagement.Common.SeedManagements.Enums;
using MediatR;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Commands
{
    public class ExecuteSeedServiceCommand : IRequest<ExecuteSeedResponse>
    {
        private readonly ExecuteSeedServiceCommandValidator _validator = new();
        public ExecuteSeedServiceCommand(SeedServiceTypes seedServiceType)
        {
            SeedServiceType = seedServiceType;

            _validator.ValidateAndThrow(this);
        }

        public SeedServiceTypes SeedServiceType { get; set; }
    }

    public class ExecuteSeedServiceCommandValidator : AbstractValidator<ExecuteSeedServiceCommand>
    {
        public ExecuteSeedServiceCommandValidator()
        {
            RuleFor(x => x.SeedServiceType).IsInEnum();
        }
    }

    public class ExecuteSeedServiceCommandHandler : IRequestHandler<ExecuteSeedServiceCommand, ExecuteSeedResponse>
    {
        private readonly ILogger<ExecuteSeedServiceCommandHandler> _logger;
        private readonly ISeedInitializer _seedInitializer;
        private readonly RecreateDbHandler _recreateDBHandler;


        public ExecuteSeedServiceCommandHandler(ILogger<ExecuteSeedServiceCommandHandler> logger,
                                                ISeedInitializer seedInitializer,
                                                RecreateDbHandler recreateDbHandler)
        {
            _seedInitializer = seedInitializer;
            _recreateDBHandler = recreateDbHandler;
            _logger = logger;
        }

        public async Task<ExecuteSeedResponse> Handle(ExecuteSeedServiceCommand command, CancellationToken cancellationToken)
        {

            await _recreateDBHandler.Handle();
            await _seedInitializer.Seed(command.SeedServiceType);

            return new ExecuteSeedResponse();
        }
    }
}