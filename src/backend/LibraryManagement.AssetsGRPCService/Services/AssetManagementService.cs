using Grpc.Core;
using LibraryManagement.AssetsGRPCService.Business.CQRS.Commands;
using LibraryManagement.AssetsGRPCService.Business.CQRS.Queries;
using MediatR;

namespace LibraryManagement.AssetsGRPCService.Services
{
    public class AssetManagementService : AssetManagementGRPCService.AssetManagementGRPCServiceBase
    {
        private readonly ILogger<AssetManagementService> _logger;
        private readonly IMediator _mediator;

        public AssetManagementService(ILogger<AssetManagementService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<BookAddResponse> AddBookRecord(BookAddRequest request, ServerCallContext context)
        {
            var command = new CreateBookCommand(request.Title, request.Author, request.Isbn, request.Publisher, request.PublicationYear, request.PageCount);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<BookUpdateResponse> UpdateBookInfo(BookUpdateRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookUpdateResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<BookDeleteResponse> DeleteBookRecord(BookDeleteRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookDeleteResponse
            {
                Message = "Hello " + request.Name
            });
        }
        public override async Task<BookCopyAddResponse> AddBookCopy(BookCopyAddRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookCopyAddResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<BookCopyUpdateResponse> UpdateBookCopy(BookCopyUpdateRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookCopyUpdateResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<BookCopyDeleteResponse> DeleteBookCopy(BookCopyDeleteRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookCopyDeleteResponse
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task<BookByISBNResponse> GetBookByISBN(BookByISBNRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetBookByIsbnQuery(request.Isbn), context.CancellationToken);
        }
    }
}