using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.BorrowingGrpcService.Domains.Enums;
using LibraryManagement.Common.GenericRepositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetBookCopiesAvailabilityQuery : IRequest<BookCopiesAvailabilityResponse>
    {
        private readonly GetBookCopiesAvailabilityQueryValidator _validator = new();
        public GetBookCopiesAvailabilityQuery(string isbn)
        {
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string Isbn { get; set; }
    }

    public class GetBookCopiesAvailabilityQueryValidator : AbstractValidator<GetBookCopiesAvailabilityQuery>
    {
        public GetBookCopiesAvailabilityQueryValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
        }
    }

    public class GetBookCopiesAvailabilityQueryHandler : IRequestHandler<GetBookCopiesAvailabilityQuery, BookCopiesAvailabilityResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetMostBorrowedBooksQuery> _logger;

        public GetBookCopiesAvailabilityQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                                       ILogger<GetMostBorrowedBooksQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<BookCopiesAvailabilityResponse> Handle(GetBookCopiesAvailabilityQuery query, CancellationToken cancellationToken)
        {
            var result = await _genericWriteRepository.GetAll<Book>()
                                                      .Select(x => new
                                                      {
                                                          ISBN = x.ISBN,
                                                          BorrowedCopiesCount = x.BookCopies.Count(x => x.Status.Equals(AssetStatus.Available)),
                                                          AvailableCopiesCount = x.BookCopies.Count(x => x.Status.Equals(AssetStatus.Borrowed)),
                                                      })
                                                      .FirstOrDefaultAsync(x => x.ISBN.Equals(query.Isbn), cancellationToken);

            return result?.Adapt<BookCopiesAvailabilityResponse>() ?? new BookCopiesAvailabilityResponse();
        }
    }
}
