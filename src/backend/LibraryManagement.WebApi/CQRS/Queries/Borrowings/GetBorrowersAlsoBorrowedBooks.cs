using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Assets;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetBorrowersAlsoBorrowedBooks : IRequest<IEnumerable<AlsoBorrowedBooksDTO>>
    {
        private readonly GetBorrowersAlsoBorrowedBooksValidator _validator = new();

        public GetBorrowersAlsoBorrowedBooks(string isbn)
        {
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string Isbn { get; set; }
    }

    public class GetBorrowersAlsoBorrowedBooksValidator : AbstractValidator<GetBorrowersAlsoBorrowedBooks>
    {
        public GetBorrowersAlsoBorrowedBooksValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
        }
    }

    public class GetBorrowersAlsoBorrowedBooksHandler : IRequestHandler<GetBorrowersAlsoBorrowedBooks, IEnumerable<AlsoBorrowedBooksDTO>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;

        public GetBorrowersAlsoBorrowedBooksHandler(IBorrowingServiceClient borrowingServiceClient,
                                                    IAssetManagementServiceClient assetManagementServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
            _assetManagementServiceClient = assetManagementServiceClient;
        }

        public async Task<IEnumerable<AlsoBorrowedBooksDTO>> Handle(GetBorrowersAlsoBorrowedBooks query, CancellationToken cancellationToken)
        {
            var book = _assetManagementServiceClient.GetBookByIsbnAsync(new BookByISBNRequest() { Isbn = query.Isbn });

            if (book == null)
            {
                throw new ArgumentNullException("//TODO: handle with custom exception");
            }

            var results = await _borrowingServiceClient.GetBorrowersAlsoBorrowedBooks(new AlsoBorrowedBooksRequest { Isbn = query.Isbn });

            //TODO: map
            return results.AlsoBorrowedBooks.Adapt<IEnumerable<AlsoBorrowedBooksDTO>>();
        }
    }
}
