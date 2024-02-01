using LibraryManagement.BorrowingGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public class BorrowingServiceClient : IBorrowingServiceClient
    {
        private readonly BorrowGRPCService.BorrowGRPCServiceClient _borrowServiceClient;

        public BorrowingServiceClient(BorrowGRPCService.BorrowGRPCServiceClient borrowServiceClient)
        {
            _borrowServiceClient = borrowServiceClient;
        }

        public async Task<MostBorrowedBooksResponse> GetMostBorrowedBooks(MostBorrowedBooksRequest request)
        {
            return await _borrowServiceClient.GetMostBorrowedBooksAsync(request);
        }

        public async Task<BorrowBookResponse> BorrowBookAsync(BorrowBookRequest request)
        {
            return await _borrowServiceClient.BorrowBookAsync(request);
        }

        public async Task<BookCopiesAvailabilityResponse> GetBookCopiesAvailability(BookCopiesAvailabilityRequest request)
        {
            return await _borrowServiceClient.GetBookCopiesAvailabilityAsync(request);
        }

        public async Task<TopBorrowersResponse> GetTopBorrowersWithinSpecifiedTimeframe(TopBorrowersRequest request)
        {
            return await _borrowServiceClient.GetTopBorrowersWithinSpecifiedTimeframeAsync(request);
        }

        public async Task<BorrowedBooksByUserResponse> GetBorrowedBooksByUser(BorrowedBooksByUserRequest request)
        {
            return await _borrowServiceClient.GetBorrowedBooksByUserAsync(request);
        }

        public async Task<ReadRateResponse> GetAverageReadRateForBook(ReadRateRequest request)
        {
            return await _borrowServiceClient.GetAverageReadRateForBookAsync(request);
        }

        public async Task<AlsoBorrowedBooksResponse> GetBorrowersAlsoBorrowedBooks(AlsoBorrowedBooksRequest request)
        {
            return await _borrowServiceClient.GetBorrowersAlsoBorrowedBooksAsync(request);
        }
    }
}
