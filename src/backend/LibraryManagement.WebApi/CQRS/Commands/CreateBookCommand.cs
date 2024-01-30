using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.WebApi.GrpcClients.Assets;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands
{
    public class CreateBookCommand : IRequest
    {
        private readonly CreateBookCommandValidator _validator = new();
        public CreateBookCommand(string title, string authorName, string isbn, string publisherName, int publicationYear)
        {
            Title = title;
            AuthorName = authorName;
            Isbn = isbn;
            PublisherName = publisherName;
            PublicationYear = publicationYear;

            _validator.ValidateAndThrow(this);
        }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string Isbn { get; set; }

        public string PublisherName { get; set; }

        public int PublicationYear { get; set; }
    }

    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.AuthorName).NotEmpty();
            RuleFor(x => x.Isbn).NotEmpty();
            RuleFor(x => x.PublisherName).NotEmpty();
            RuleFor(c => c.PublicationYear).NotEmpty().ExclusiveBetween(1000, DateTime.Today.Year);
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
            };

            await _assetManagementServiceClient.AddBookRecordAsync(bookRecord);
        }
    }
}
