using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.Common.Constants;
using LibraryManagement.WebApi.GrpcClients.Assets;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands.AssetManagements
{
    public class CreateBookCommand : IRequest
    {
        private readonly CreateBookCommandValidator _validator = new();
        public CreateBookCommand(string title, 
                                 string authorName, 
                                 string isbn, 
                                 string publisherName, 
                                 int publicationYear, 
                                 int pageCount)
        {
            Title = title;
            AuthorName = authorName;
            Isbn = isbn;
            PublisherName = publisherName;
            PublicationYear = publicationYear;
            PageCount = pageCount;

            _validator.ValidateAndThrow(this);
        }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string Isbn { get; set; }

        public string PublisherName { get; set; }

        public int PublicationYear { get; set; }

        public int PageCount { get; set; }
    }

    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_TITLE);
            RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_AUTHOR);
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
            RuleFor(x => x.PublisherName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_PUBLISHER);
            RuleFor(c => c.PublicationYear).NotEmpty().ExclusiveBetween(1000, DateTime.Today.Year);
            RuleFor(c => c.PageCount).NotEmpty().GreaterThan(0);
        }
    }

    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand>
    {
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;
        private readonly ILogger<CreateBookCommandHandler> _logger;

        public CreateBookCommandHandler(IAssetManagementServiceClient assetManagementServiceClient, ILogger<CreateBookCommandHandler> logger)
        {
            _assetManagementServiceClient = assetManagementServiceClient;
            _logger = logger;
        }

        public async Task Handle(CreateBookCommand command, CancellationToken cancellationToken)
        {
            var bookRecord = new BookAddRequest
            {
                Title = command.Title,
                Author = command.AuthorName,
                Isbn = command.Isbn,
                PublicationYear = command.PublicationYear,
                Publisher = command.PublisherName,
                PageCount = command.PageCount
            };

            await _assetManagementServiceClient.AddBookRecordAsync(bookRecord);
        }
    }
}
