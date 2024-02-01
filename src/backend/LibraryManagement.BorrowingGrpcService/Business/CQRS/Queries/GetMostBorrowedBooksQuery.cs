using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using MediatR;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetMostBorrowedBooksQuery : IRequest<MostBorrowedBooksResponse>
    {
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

            // count > 0
            var mostBorrowedBooks = await Task.Run(() => _genericWriteRepository.GetAll<Borrowing>()
                                                             .Where(x=> x.BookCopyId == null)
                                                             .GroupBy(x => new { x.BookId, x.Book.Title, x.Book.Publisher, x.Book.Author, x.Book.ISBN })
                                                             .Select(x => new 
                                                             { 
                                                                 BookId = x.Key, 
                                                                 Title = x.Key.Title, 
                                                                 Publisher = x.Key.Publisher,
                                                                 Author = x.Key.Author,
                                                                 ISBN = x.Key.ISBN,
                                                                 Count = x.Count(),

                                                             }).OrderByDescending(x => x.Count)
                                                               .Select(x => new BorrowedBook
                                                               {
                                                                   Title = x.Title,
                                                                   Author = x.Author,
                                                                   Publisher = x.Publisher,
                                                                   Isbn = x.ISBN,
                                                                   BorrowedCount = x.Count
                                                               }));


            return new MostBorrowedBooksResponse() { MostBorrowedBooks = { mostBorrowedBooks } };
        }
    }
}
