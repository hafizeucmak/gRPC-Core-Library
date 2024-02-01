using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetMostBorrowedBooksQuery : IRequest<MostBorrowedBooksResponse>
    {
        private readonly GetMostBorrowedBooksQueryValidator _validator = new();
        public GetMostBorrowedBooksQuery(int expectedMostBorrowBookCount)
        {
            ExpectedMostBorrowBookCount = expectedMostBorrowBookCount;

            _validator.ValidateAndThrow(this);
        }

        public int ExpectedMostBorrowBookCount { get; set; }
    }

    public class GetMostBorrowedBooksQueryValidator : AbstractValidator<GetMostBorrowedBooksQuery>
    {
        public GetMostBorrowedBooksQueryValidator()
        {
            RuleFor(x => x.ExpectedMostBorrowBookCount).GreaterThan(0);
        }
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
            var mostBorrowedBookIds = _genericWriteRepository.GetAll<Borrowing>()
                                                             .GroupBy(x => x.BookId)
                                                             .Select(x => new { BookId = x.Key, Count = x.Count() })
                                                             .OrderByDescending(x => x.Count)
                                                             .Select(x => x.BookId)
                                                             .Take(query.ExpectedMostBorrowBookCount);
                                                         

            var mostBorrowedBooks = _genericWriteRepository.GetAll<Book>().Where(x => mostBorrowedBookIds.Contains(x.Id));


            // Define mapping from Book entity to BorrowedBook Protobuf message
            TypeAdapterConfig<Book, BorrowedBook>.NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Author, src => src.Author)
                .Map(dest => dest.PublicationYear, src => src.PublicationYear)
                .Map(dest => dest.Publisher, src => src.Publisher)
                .Map(dest => dest.PageCount, src => src.PageCount);

            // Map mostBorrowedBooks to MostBorrowedBooksResponse using Mapster
            var response = mostBorrowedBooks.Adapt<MostBorrowedBooksResponse>();

            return response;


            //var response = new MostBorrowedBooksResponse();

            //foreach (var borrowedBook in mostBorrowedBooks)
            //{
            //    var protobufBorrowedBook = new BorrowedBook
            //    {
            //       Title = borrowedBook.Title,
            //       Author = borrowedBook.Author,
            //       PublicationYear = borrowedBook.PublicationYear,
            //       Publisher = borrowedBook.Publisher,
            //       PageCount = borrowedBook.PageCount
            //    };

            //    response.MostBorrowedBooks.Add(protobufBorrowedBook);
            //}

            //return response;
        }
    }
}
