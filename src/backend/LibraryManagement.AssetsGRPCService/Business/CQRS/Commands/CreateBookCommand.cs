using LibraryManagement.AssetsGRPCService.DataAccesses.DbContexts;
using LibraryManagement.Common.GenericRepositories;
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

        public CreateBookCommandHandler(IGenericWriteRepository<AssetBaseDbContext> genericWriteRepository, ILogger<CreateBookCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task Handle(CreateBookCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var book = new Book("example titel", "example author", "example isbn", "example publicher", 2017);

            await _genericWriteRepository.AddAsync(book, cancellationToken);

            _genericWriteRepository.CommitTransaction();
        }
    }
}
