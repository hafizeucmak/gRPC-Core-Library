using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.Common.Constants;
using LibraryManagement.WebApi.GrpcClients.Assets;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands.AssetManagements
{
    public class CreateBookCopyCommand : IRequest
    {
        private readonly CreateBookCopyCommandValidator _validator = new();
        public CreateBookCopyCommand(string isbn)
        {
           
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }
        public string Isbn { get; set; }
    }

    public class CreateBookCopyCommandValidator : AbstractValidator<CreateBookCopyCommand>
    {
        public CreateBookCopyCommandValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
        }
    }

    public class CreateBookCopyCommandHandler : IRequestHandler<CreateBookCopyCommand>
    {
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;
        private readonly ILogger<CreateBookCopyCommandHandler> _logger;

        public CreateBookCopyCommandHandler(IAssetManagementServiceClient assetManagementServiceClient, ILogger<CreateBookCopyCommandHandler> logger)
        {
            _assetManagementServiceClient = assetManagementServiceClient;
            _logger = logger;
        }

        public async Task Handle(CreateBookCopyCommand command, CancellationToken cancellationToken)
        {
            var bookOfCopy = await _assetManagementServiceClient.GetBookByIsbnAsync(new BookByISBNRequest() { Isbn = command.Isbn });

            if (bookOfCopy == null)
            {
                throw new ArgumentNullException("//TODO:");
            }

            await _assetManagementServiceClient.AddBookCopyAsync(new BookCopyAddRequest() { BookId = bookOfCopy.Id, Isbn = bookOfCopy.Isbn });
        }
    }
}
