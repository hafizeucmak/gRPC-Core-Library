using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.Common.Constants;
using LibraryManagement.UserGrpcService;
using LibraryManagement.WebApi.GrpcClients.Assets;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.GrpcClients.Users;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands.Borrowings
{
    public class BorrowBookCommand : IRequest
    {
        private readonly BorrowBookCommandValidator _validator = new();
        public BorrowBookCommand(string isbn,
                                 string userEmail)
        {
            Isbn = isbn;
            UserEmail = userEmail;

            _validator.ValidateAndThrow(this);
        }

        public string UserEmail { get; set; }

        public string Isbn { get; set; }
    }

    public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
    {
        public BorrowBookCommandValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
            RuleFor(x => x.UserEmail).NotEmpty().EmailAddress();
        }
    }

    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogger<BorrowBookCommandHandler> _logger;

        public BorrowBookCommandHandler(IBorrowingServiceClient borrowingServiceClient,
                                        ILogger<BorrowBookCommandHandler> logger,
                                        UserServiceClient userServiceClient,
                                        IAssetManagementServiceClient assetManagementServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
            _assetManagementServiceClient = assetManagementServiceClient;
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        public async Task Handle(BorrowBookCommand command, CancellationToken cancellationToken)
        {

            var book = await _assetManagementServiceClient.GetBookByIsbnAsync(new BookByISBNRequest() { Isbn = command.Isbn });

            if (book == null)
            {
                //TODO: use custom email excaption for API service
                throw new ArgumentNullException($"{nameof(book)} not found with {nameof(command.Isbn)} is equal to {command.Isbn}");
            }

            var user = await _userServiceClient.GetUserByEmailAsync(new UserByEmailRequest() { Email = command.UserEmail });

            if (user == null)
            {
                //TODO: use custom email excaption for API service
                throw new ArgumentNullException($"{nameof(user)} not found with {nameof(command.UserEmail)} is equal to {command.UserEmail}");
            }


            var borrowBookRequest = new BorrowBookRequest
            {
                Isbn = command.Isbn,
                UserEmail = command.UserEmail
            };

            var result = await _borrowingServiceClient.BorrowBookAsync(borrowBookRequest);
        }
    }
}
