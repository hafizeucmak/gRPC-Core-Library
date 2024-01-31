using FluentValidation;
using LibraryManagement.AssetsGRPCService.DataAccesses.DbContexts;
using LibraryManagement.AssetsGRPCService.Domains;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.AssetsGRPCService.Business.CQRS.Queries
{
    public class GetBookByIsbnQuery : IRequest<BookByISBNResponse>
    {
        private readonly GetBookByIsbnQueryValidator _validator = new();

        public GetBookByIsbnQuery(string isbn)
        {
            ISBN = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string ISBN { get; set; }
    }

    public class GetBookByIsbnQueryValidator : AbstractValidator<GetBookByIsbnQuery>
    {
        public GetBookByIsbnQueryValidator()
        {
            RuleFor(x => x.ISBN).NotEmpty();
        }
    }

    public class GetMostBorrowedBooksHandler : IRequestHandler<GetBookByIsbnQuery, BookByISBNResponse>
    {
        private readonly IGenericWriteRepository<AssetBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetBookByIsbnQuery> _logger;

        public GetMostBorrowedBooksHandler(IGenericWriteRepository<AssetBaseDbContext> genericWriteRepository,
                                           ILogger<GetBookByIsbnQuery> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<BookByISBNResponse> Handle(GetBookByIsbnQuery query, CancellationToken cancellationToken)
        {
            var book = await _genericWriteRepository.GetAll<Book>().FirstOrDefaultAsync(x => x.ISBN.Equals(query.ISBN), cancellationToken);

            if (book == null)
            {
                //TODO:
                throw new ArgumentNullException(nameof(book));
            }

            return new BookByISBNResponse
            {
                Title = book?.Title,
                AuthorName = book?.Author,
                PublicationYear = book?.PublicationYear ?? 0,
                PageCount = book?.PageCount ?? 0,
                Publisher = book?.Publisher,
            };
        }
    }
}
