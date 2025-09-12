using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetAverageReadRateOfUsersQuery : IRequest<ReadRatesOfUsersResponse>
    {
        public GetAverageReadRateOfUsersQuery(string queryOptions)
        {
            QueryOptions = queryOptions;
        }

        public string QueryOptions { get; set; }
    }

    public class GetAverageReadRateOfUsersQueryHandler : IRequestHandler<GetAverageReadRateOfUsersQuery, ReadRatesOfUsersResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetAverageReadRateOfUsersQuery> _logger;

        public GetAverageReadRateOfUsersQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository, ILogger<GetAverageReadRateOfUsersQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<ReadRatesOfUsersResponse> Handle(GetAverageReadRateOfUsersQuery query, CancellationToken cancellationToken)
        {
            var dynamicQueryOptions = JsonConvert.DeserializeObject<DynamicQueryOptions>(query.QueryOptions);

            var groupedQuery = _genericWriteRepository.GetAll<Borrowing>()
                                                      .Where(b => b.ReturnDate != null)
                                                      .GroupBy(b => new { b.UserId, b.User.FullName })
                                                      .Select(g => new ReadRateOfUser
                                                       {
                                                           UserId = g.Key.UserId,
                                                           UserName = g.Key.FullName,
                                                           ReadRate = Math.Round(g.Sum(b => b.Book!.PageCount) / (double)g.Sum(b => EF.Functions.DateDiffDay(b.BorrowDate, b.ReturnDate!.Value)), 2)
                                                       });

            var filteredQuery = groupedQuery.ApplyFilters(dynamicQueryOptions);

            var userReadRates = await filteredQuery.ToListAsync(cancellationToken);

            var result = new ReadRatesOfUsersResponse();
            result.ReadRates.AddRange(userReadRates);

            return result;
        }
    }
}
