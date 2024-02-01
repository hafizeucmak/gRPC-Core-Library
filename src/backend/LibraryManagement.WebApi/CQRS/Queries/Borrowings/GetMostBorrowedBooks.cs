using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetMostBorrowedBooks : IRequest<IQueryable<BorrowedBook>>
    {
    }

    public class GetMostBorrowedBooksHandler : IRequestHandler<GetMostBorrowedBooks, IQueryable<BorrowedBook>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetMostBorrowedBooksHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<IQueryable<BorrowedBook>> Handle(GetMostBorrowedBooks query, CancellationToken cancellationToken)
        {
            var results = await _borrowingServiceClient.GetMostBorrowedBooks(new MostBorrowedBooksRequest());

            return results.MostBorrowedBooks.AsQueryable();
        }
    }
}
