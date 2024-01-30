using LibraryManagement.AssetsGRPCService.Business.Constants.EventConstants;
using LibraryManagement.AssetsGRPCService.Business.Events;
using LibraryManagement.AssetsGRPCService.DataAccesses.DbContexts;
using LibraryManagement.Common.GenericRepositories;
using LibraryManagement.Common.RabbitMQEvents;
using LibraryManagement.Domain.Entities.Books;
using MediatR;

namespace LibraryManagement.Business.CQRS.Commands
{
    public class CreateBookCommand : IRequest
    {
        public CreateBookCommand()
        {
        }

    }

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand>
    {
        private readonly IGenericWriteRepository<AssetBaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateBookCommandHandler> _logger;
        private readonly RegisteredEventCommands _registeredEventCommands;

        public CreateBookCommandHandler(IGenericWriteRepository<AssetBaseDbContext> genericWriteRepository,
                                        ILogger<CreateBookCommandHandler> logger,
                                        RegisteredEventCommands registeredEventCommands)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
            _registeredEventCommands = registeredEventCommands;
        }

        public async Task Handle(CreateBookCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var book = new Book("example titel", "example author", "example isbn", "example publicher", 2017);

            _registeredEventCommands.RegisteredEventCommand(new QueueEventCommand<BookCreatedEvent>(EventConstants.BookCreatedQueueName));

            await _genericWriteRepository.AddAsync(book, cancellationToken);
        }
    }
}
