using Grpc.Core;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Commands;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.Common.SeedManagements.Enums;
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
            return await _mediator.Send(new GetMostBorrowedBooksQuery(request.QueryOptions), context.CancellationToken);
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
            return await _mediator.Send(new GetBorrowedBooksByUserQuery(request.StartDate.ToDateTime(), request.EndDate.ToDateTime(), request.UserEmail), context.CancellationToken);
        }

        public override async Task<ReadRateResponse> GetAverageReadRateForBook(ReadRateRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetAverageReadRateForBookQuery(request.Isbn), context.CancellationToken);
        }

        public override async Task<AlsoBorrowedBooksResponse> GetBorrowersAlsoBorrowedBooks(AlsoBorrowedBooksRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new GetBorrowersAlsoBorrowedBooksQuery(request.Isbn, request.QueryOptions), context.CancellationToken);
        }

        public override async Task<ExecuteSeedResponse> ExecuteSeed(ExecuteSeedRequest request, ServerCallContext context)
        {
            return await _mediator.Send(new ExecuteSeedServiceCommand(SeedServiceTypes.BorrowingStatistic), context.CancellationToken);
        }
    }
}