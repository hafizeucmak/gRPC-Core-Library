using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;
using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
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

            var baseQuery = _genericWriteRepository.GetAll<Borrowing>()
                                                           .Where(x => x.BookCopyId == null)
                                                           .GroupBy(x => new { x.BookId, x.Book.Title, x.Book.Publisher, x.Book.Author, x.Book.ISBN })
                                                           .Select(x => new
                                                           {
                                                               BookId = x.Key,
                                                               Title = x.Key.Title,
                                                               Publisher = x.Key.Publisher,
                                                               Author = x.Key.Author,
                                                               ISBN = x.Key.ISBN,
                                                               BorrowedCount = x.Count(),

                                                           }).Where(x => x.BorrowedCount > 0)
                                                             .OrderByDescending(x => x.BorrowedCount);

            var filteredQuery = baseQuery.ApplyFilters(dynamicQueryOptions);

            var mostBorrowedBooks = await filteredQuery.Select(x => new BorrowedBook
            {
                Title = x.Title,
                Author = x.Author,
                Publisher = x.Publisher,
                Isbn = x.ISBN,
                BorrowedCount = x.BorrowedCount
            }).ToListAsync(cancellationToken);

            return new MostBorrowedBooksResponse() { MostBorrowedBooks = { mostBorrowedBooks } };
        }
    }
}
