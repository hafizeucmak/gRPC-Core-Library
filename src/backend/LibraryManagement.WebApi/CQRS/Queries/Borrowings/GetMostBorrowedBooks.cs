using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetMostBorrowedBooks : IRequest<IEnumerable<MostBorrowedBooksDTO>>
    {
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
            var a = await _borrowingServiceClient.GetMostBorrowedBooks();

            return a.MostBorrowedBooks.Select(x => new MostBorrowedBooksDTO
            {
                Name = x.Name,
            });
        }
    }
}
