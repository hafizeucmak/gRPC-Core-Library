using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using MediatR;
using Newtonsoft.Json;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetMostBorrowedBooks : IRequest<IEnumerable<MostBorrowedBooksDTO>>
    {
        public GetMostBorrowedBooks(DynamicQueryOptions queryOptions)
        {
            QueryOptions = queryOptions;
        }

        public DynamicQueryOptions QueryOptions { get; set; }
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
            var serializedOptions = JsonConvert.SerializeObject(query.QueryOptions);

            var results = await _borrowingServiceClient.GetMostBorrowedBooks(new MostBorrowedBooksRequest() { QueryOptions = serializedOptions });

            return results.MostBorrowedBooks.Select(x => new MostBorrowedBooksDTO 
                                                   { 
                                                      Title = x.Title,
                                                      AuthorName = x.Author,
                                                      PublisherName = x.Publisher,
                                                      Isbn = x.Isbn,
                                                      BorrowedCount = x.BorrowedCount,
                                                   });
        }
    }
}
