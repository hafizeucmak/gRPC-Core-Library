using FluentValidation;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetMostBorrowedBooks : IRequest<IEnumerable<MostBorrowedBooksDTO>>
    {
        private readonly GetMostBorrowedBooksValidator _validator = new();

        public GetMostBorrowedBooks(int expectedMostBorrowBookCount)
        {
            ExpectedMostBorrowBookCount = expectedMostBorrowBookCount;

            _validator.ValidateAndThrow(this);
        }

        public int ExpectedMostBorrowBookCount { get; set; }
    }

    public class GetMostBorrowedBooksValidator : AbstractValidator<GetMostBorrowedBooks>
    {
        public GetMostBorrowedBooksValidator()
        {
            RuleFor(x => x.ExpectedMostBorrowBookCount).GreaterThan(0);
        }
    }

    public class GetMostBorrowedBooksHandler : IRequestHandler<GetMostBorrowedBooks, IEnumerable<MostBorrowedBooksDTO>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetMostBorrowedBooksHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<IEnumerable<MostBorrowedBooksDTO>> Handle(GetMostBorrowedBooks query, CancellationToken cancellationToken)
        {
            var requets = new MostBorrowedBooksRequest() { ExpectedMostBorrowBookCount = query.ExpectedMostBorrowBookCount };

            var results = await _borrowingServiceClient.GetMostBorrowedBooks(requets);

            //TODO: map
            return results.MostBorrowedBooks.Select(x => new MostBorrowedBooksDTO
            {
                Name = x.Title,
            });
        }
    }
}
