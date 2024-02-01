using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetBorrowersAlsoBorrowedBooksQuery : IRequest<AlsoBorrowedBooksResponse>
    {
        private readonly GetBorrowersAlsoBorrowedBooksQueryValidator _validator = new();
        public GetBorrowersAlsoBorrowedBooksQuery(string isbn)
        {
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string Isbn { get; set; }
    }

    public class GetBorrowersAlsoBorrowedBooksQueryValidator : AbstractValidator<GetBorrowersAlsoBorrowedBooksQuery>
    {
        public GetBorrowersAlsoBorrowedBooksQueryValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
        }
    }

    public class GetBorrowersAlsoBorrowedBooksQueryHandler : IRequestHandler<GetBorrowersAlsoBorrowedBooksQuery, AlsoBorrowedBooksResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetMostBorrowedBooksQuery> _logger;

        public GetBorrowersAlsoBorrowedBooksQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                                       ILogger<GetMostBorrowedBooksQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<AlsoBorrowedBooksResponse> Handle(GetBorrowersAlsoBorrowedBooksQuery query, CancellationToken cancellationToken)
        {
            var book = await _genericWriteRepository.GetAll<Book>().FirstOrDefaultAsync(x => x.ISBN.Equals(query.Isbn), cancellationToken);

            if (book == null)
            {
                throw new ResourceNotFoundException($"{nameof(book)} not found with {nameof(query.Isbn)} is equal to {query.Isbn}");
            }

            var borrowerIds = _genericWriteRepository.GetAll<Borrowing>().Where(x => x.BookId.Equals(book.Id) && x.BookCopyId == null).Select(x => x.UserId);

            var alsoBorrowedBooksByUsers = await _genericWriteRepository.GetAll<Borrowing>()
                                                                   .Where(x => borrowerIds.Contains(x.UserId) && !x.BookId.Equals(book.Id) && x.BookCopyId == null)
                                                                   .Select(x => new AlsoBorrowedBookDetail
                                                                   {
                                                                       Title = x.Book.Title,
                                                                       Author = x.Book.Author,
                                                                       Publisher = x.Book.Publisher,
                                                                   }).ToListAsync(cancellationToken);

            return new AlsoBorrowedBooksResponse
            {
                AlsoBorrowedBooks = { alsoBorrowedBooksByUsers }
            };
        }
    }
}
