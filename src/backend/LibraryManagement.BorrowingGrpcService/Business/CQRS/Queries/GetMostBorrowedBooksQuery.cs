using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;
using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetMostBorrowedBooksQuery : IRequest<MostBorrowedBooksResponse>
    {
        public GetMostBorrowedBooksQuery(string queryOptions) 
        {
            QueryOptions = queryOptions;
        }

        public string QueryOptions { get; set; }
    }
    public class GetMostBorrowedBooksQueryHandler : IRequestHandler<GetMostBorrowedBooksQuery, MostBorrowedBooksResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetMostBorrowedBooksQuery> _logger;

        public GetMostBorrowedBooksQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                               ILogger<GetMostBorrowedBooksQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<MostBorrowedBooksResponse> Handle(GetMostBorrowedBooksQuery query, CancellationToken cancellationToken)
        {
            var dynamicQueryOptions = JsonConvert.DeserializeObject<DynamicQueryOptions>(query.QueryOptions);

            var groupedQuery = _genericWriteRepository.GetAll<Borrowing>()
                               .Where(x => x.BookCopyId == null)
                               .GroupBy(x => new { x.BookId, x.Book.Title, x.Book.Publisher, x.Book.Author, x.Book.ISBN })
                               .Select(g => new
                               {
                                   g.Key.BookId,
                                   g.Key.Title,
                                   g.Key.Author,
                                   g.Key.Publisher,
                                   Count = g.Count()
                               });

            // Now apply dynamic filter on anonymous type
            var filtered = groupedQuery.ApplyFilters(dynamicQueryOptions);

            var result = filtered.Select(x => new BorrowedBook
                                {
                                    Title = x.Title,
                                    Author = x.Author,
                                    Publisher = x.Publisher,
                                    BorrowedCount = x.Count
                                }).ToList();


            return new MostBorrowedBooksResponse() { MostBorrowedBooks = { result } };
        }
    }
}
