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

        public async Task<MostBorrowedBooksResponse> GetMostBorrowedBooks()
        {
            return await _borrowServiceClient.GetMostBorrowedBooksAsync(new MostBorrowedBooksRequest());
        }

        public async Task<BorrowBookResponse> BorrowBookAsync(BorrowBookRequest request)
        {
            return await _borrowServiceClient.BorrowBookAsync(request);
        }

        public Task GetBookAvailability()
        {
            throw new NotImplementedException();
        }

        public Task GetBorrowedBooksByUser()
        {
            throw new NotImplementedException();
        }

        public Task GetReadRate()
        {
            throw new NotImplementedException();
        }

        public Task GetRelatedBooks()
        {
            throw new NotImplementedException();
        }

        public Task GetTopBorrowers()
        {
            throw new NotImplementedException();
        }
    }
}
