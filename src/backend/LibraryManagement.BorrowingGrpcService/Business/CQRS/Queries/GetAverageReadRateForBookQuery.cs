using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetAverageReadRateForBookQuery : IRequest<ReadRateResponse>
    {
        private readonly GetAverageReadRateForBookQueryValidator _validator = new();
        public GetAverageReadRateForBookQuery(string isbn)
        {
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string Isbn { get; set; }
    }

    public class GetAverageReadRateForBookQueryValidator : AbstractValidator<GetAverageReadRateForBookQuery>
    {
        public GetAverageReadRateForBookQueryValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
        }
    }

    public class GetAverageReadRateForBookQueryHandler : IRequestHandler<GetAverageReadRateForBookQuery, ReadRateResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetMostBorrowedBooksQuery> _logger;

        public GetAverageReadRateForBookQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                                       ILogger<GetMostBorrowedBooksQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<ReadRateResponse> Handle(GetAverageReadRateForBookQuery query, CancellationToken cancellationToken)
        {

            var book = await _genericWriteRepository.GetAll<Book>().FirstOrDefaultAsync(x => x.ISBN.Equals(query.Isbn), cancellationToken);

            if (book == null)
            {
                throw new ResourceNotFoundException($"{nameof(book)} not found with {nameof(query.Isbn)} is equal to {query.Isbn}");
            }

            var borrowingsOfBook = _genericWriteRepository.GetAll<Borrowing>()
                                                          .Where(x => x.BookId.Equals(book.Id) && x.BookCopyId == null && x.ReturnDate != null);

            var borrowingsCount = borrowingsOfBook.Count();

            if (borrowingsCount == 0)
            {
                throw new ResourceNotFoundException($"{nameof(book)} never borrowed.");
            }

            var totalBorrowDays = await borrowingsOfBook.SumAsync(x => EF.Functions.DateDiffDay(x.BorrowDate, x.ReturnDate));

            var totalReadPageCount = borrowingsCount * book.PageCount;

            var result = ((double)totalReadPageCount / totalBorrowDays);

            return new ReadRateResponse() { ReadRate = result ?? 0 };
        }
    }
}
