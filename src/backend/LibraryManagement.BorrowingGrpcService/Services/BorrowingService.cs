using Grpc.Core;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Commands;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using MediatR;

namespace LibraryManagement.BorrowingGrpcService.Services
{
    public class BorrowingService : BorrowGRPCService.BorrowGRPCServiceBase
    {
        private readonly ILogger<BorrowingService> _logger;
        private readonly IMediator _mediator;
        public BorrowingService(ILogger<BorrowingService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public override async Task<MostBorrowedBooksResponse> GetMostBorrowedBooks(MostBorrowedBooksRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetMostBorrowedBooksQuery(request.ExpectedMostBorrowBookCount), context.CancellationToken);
        }

        public override async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
        {
            var command = new BorrowBookCommand(request.Isbn, request.UserEmail);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<BookCopiesAvailabilityResponse> GetBookCopiesAvailability(BookCopiesAvailabilityRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetBookCopiesAvailabilityQuery(request.Isbn), context.CancellationToken);
        }

        public override async Task<TopBorrowersResponse> GetTopBorrowersWithinSpecifiedTimeframe(TopBorrowersRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetTopBorrowersWithinSpecifiedTimeframeQuery(request.StartDate.ToDateTime(), request.EndDate.ToDateTime(), request.ExpectedTopBorrowerCount), context.CancellationToken);
        }

        public override async Task<BorrowedBooksByUserResponse> GetBorrowedBooksByUser(BorrowedBooksByUserRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BorrowedBooksByUserResponse
            {

            });
        }

        public override async Task<RelatedBooksResponse> GetRelatedBooks(RelatedBooksRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new RelatedBooksResponse
            {

            });
        }

        public override async Task<ReadRateResponse> GetReadRate(ReadRateRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new ReadRateResponse
            {

            });
        }
    }
}