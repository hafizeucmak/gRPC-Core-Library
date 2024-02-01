using FluentValidation;
using LibraryManagement.AssetsGRPCService.Data.DataAccess.DbContexts;
using LibraryManagement.AssetsGRPCService.Domains;
using LibraryManagement.Common.Constants;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.AssetsGRPCService.Business.CQRS.Commands
{
    public class CreateBookCopyCommand : IRequest<BookCopyAddResponse>
    {
        private readonly CreateBookCopyCommandValidator _validator = new();
        public CreateBookCopyCommand(string isbn)
        {

            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }
        public string Isbn { get; set; }
    }

    public class CreateBookCopyCommandValidator : AbstractValidator<CreateBookCopyCommand>
    {
        public CreateBookCopyCommandValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
        }
    }

    public class CreateBookCopyCommandHandler : IRequestHandler<CreateBookCopyCommand, BookCopyAddResponse>
    {
        private readonly IGenericWriteRepository<AssetBaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateBookCopyCommandHandler> _logger;

        public CreateBookCopyCommandHandler(IGenericWriteRepository<AssetBaseDbContext> genericWriteRepository,
                                            ILogger<CreateBookCopyCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<BookCopyAddResponse> Handle(CreateBookCopyCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var book = await _genericWriteRepository.GetAll<Book>().FirstOrDefaultAsync(x => x.ISBN.Equals(command.Isbn), cancellationToken);

            if (book == null)
            {
                throw new ResourceNotFoundException($"{nameof(Book)} with {nameof(command.Isbn)} is equal to {command.Isbn} was not found.");
            }

            var bookCopy = new BookCopy(book.Id, book);

            await _genericWriteRepository.AddAsync(bookCopy, cancellationToken);

            return new BookCopyAddResponse();
        }
    }
}
