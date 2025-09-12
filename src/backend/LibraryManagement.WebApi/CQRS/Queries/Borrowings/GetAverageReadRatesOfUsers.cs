using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using MediatR;
using Newtonsoft.Json;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetAverageReadRatesOfUsers : IRequest<IEnumerable<ReadRatesOfUsersOutputDTO>>
    {
        public GetAverageReadRatesOfUsers(DynamicQueryOptions queryOptions)
        {
            QueryOptions = queryOptions;
        }
        public DynamicQueryOptions QueryOptions { get; set; }
    }

    public class GetAverageReadRatesOfUsersHandler : IRequestHandler<GetAverageReadRatesOfUsers, IEnumerable<ReadRatesOfUsersOutputDTO>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetAverageReadRatesOfUsersHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<IEnumerable<ReadRatesOfUsersOutputDTO>> Handle(GetAverageReadRatesOfUsers query, CancellationToken cancellationToken)
        {
            var serializedQueryOptions = JsonConvert.SerializeObject(query.QueryOptions);

            var results = await _borrowingServiceClient.GetAverageReadRatesOfUsers(new ReadRatesOfUsersRequest { QueryOptions = serializedQueryOptions });

            return results.ReadRates.Select(x => new ReadRatesOfUsersOutputDTO
            {
                UserId = x.UserId,
                UserName = x.UserName,
                ReadRate = x.ReadRate
            });
        }
    }
}
