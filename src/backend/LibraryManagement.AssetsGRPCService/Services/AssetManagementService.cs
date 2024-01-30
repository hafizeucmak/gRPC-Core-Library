using Grpc.Core;
using LibraryManagement.Business.CQRS.Commands;
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
            await _mediator.Send(new CreateBookCommand(), context.CancellationToken);

            return await Task.FromResult(new BookAddResponse
            {
                Message = "Hello " + request.Name
            });
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
    }
}