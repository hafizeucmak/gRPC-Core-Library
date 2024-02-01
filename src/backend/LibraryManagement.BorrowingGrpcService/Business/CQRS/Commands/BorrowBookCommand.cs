using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.Constants;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Commands
{
    public class BorrowBookCommand : IRequest<BorrowBookResponse>
    {
        private readonly BorrowBookCommandValidator _validator = new();
        public BorrowBookCommand(string isbn,
                                 string userEmail)
        {
            Isbn = isbn;
            UserEmail = userEmail;

            _validator.ValidateAndThrow(this);
        }

        public string UserEmail { get; set; }

        public string Isbn { get; set; }
    }

    public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
    {
        public BorrowBookCommandValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
            RuleFor(x => x.UserEmail).NotEmpty().EmailAddress();
        }
    }

    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowBookResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<BorrowBookCommandHandler> _logger;

        public BorrowBookCommandHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository, ILogger<BorrowBookCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<BorrowBookResponse> Handle(BorrowBookCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var book = await _genericWriteRepository.GetAll<Book>().FirstOrDefaultAsync(x => x.ISBN.Equals(command.Isbn), cancellationToken);

            if (book == null)
            {
                throw new ResourceNotFoundException($"{nameof(book)} not found with {nameof(command.Isbn)} is equal to {command.Isbn}");
            }

            var user = await _genericWriteRepository.GetAll<User>().FirstOrDefaultAsync(x => x.Email.Equals(command.UserEmail), cancellationToken);

            if (user == null)
            {
                throw new ResourceNotFoundException($"{nameof(user)} not found with {nameof(command.UserEmail)} is equal to {command.UserEmail}");
            }

            var borrowing = new Borrowing(book.Id, book, user.Id, null);

            book.UpdateStatusAsBorrowed();

            await _genericWriteRepository.UpdateAsync(book, cancellationToken);

            await _genericWriteRepository.AddAsync(borrowing, cancellationToken);

            return new BorrowBookResponse { };
        }
    }
}
