using Grpc.Core;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Commands;
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
            MostBorrowedBooksResponse response = new()
            {
                MostBorrowedBooks = { new BorrowedBook
                                   {
                                       Name = "İnsan Olmak",
                                       Author = "Engin Gençtan",
                                       Page = 1030,
                                       Isbn = "flkssfkmsl"
                                   }}
            };

            // Return the response asynchronously using Task.FromResult
            return await Task.FromResult(response);
        }
        public override async Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
        {
            var command = new BorrowBookCommand(request.Isbn, request.UserEmail);
            return await _mediator.Send(command, context.CancellationToken);
        }

        public override async Task<BookAvailabilityResponse> GetBookAvailability(BookAvailabilityRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new BookAvailabilityResponse
            {
                Status = "available"
            });
        }


        public override async Task<TopBorrowersResponse> GetTopBorrowers(TopBorrowersRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new TopBorrowersResponse
            {

            });
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