using FluentValidation;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.WebApi.GrpcClients.Assets;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.AssetManagements
{
    public class GetBookByIsbn : IRequest<BookDetailsDTO>
    {
        private readonly GetBookByIsbnValidator _validator = new();
        public GetBookByIsbn(string isbn)
        {
            ISBN = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string ISBN { get; set; }
    }

    public class GetBookByIsbnValidator : AbstractValidator<GetBookByIsbn>
    {
        public GetBookByIsbnValidator()
        {
            RuleFor(x => x.ISBN).NotEmpty();
        }
    }
    public class GetBookByIsbnHandler : IRequestHandler<GetBookByIsbn, BookDetailsDTO>
    {
        private readonly IAssetManagementServiceClient _assetManagementServiceClient;

        public GetBookByIsbnHandler(IAssetManagementServiceClient assetManagementServiceClient)
        {
            _assetManagementServiceClient = assetManagementServiceClient;
        }

        public async Task<BookDetailsDTO> Handle(GetBookByIsbn query, CancellationToken cancellationToken)
        {
            var result = await _assetManagementServiceClient.GetBookByIsbnAsync(new BookByISBNRequest() { Isbn = query.ISBN});

            return result.Adapt<BookDetailsDTO>();
        }
    }
}
