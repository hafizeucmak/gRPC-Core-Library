using Grpc.Core;

namespace LibraryManagement.BorrowingGrpcService.Services
{
    public class BorrowingService : BorrowGRPCService.BorrowGRPCServiceBase
    {
        private readonly ILogger<BorrowingService> _logger;
        public BorrowingService(ILogger<BorrowingService> logger)
        {
            _logger = logger;
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