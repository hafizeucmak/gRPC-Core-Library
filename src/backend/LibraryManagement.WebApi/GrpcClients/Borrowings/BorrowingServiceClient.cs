using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.Models;
using Mapster;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public class BorrowingServiceClient : IBorrowingServiceClient
    {
        private readonly BorrowGRPCService.BorrowGRPCServiceClient _borrowServiceClient;

        public BorrowingServiceClient(BorrowGRPCService.BorrowGRPCServiceClient borrowServiceClient)
        {
            _borrowServiceClient = borrowServiceClient;
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


        public async Task<MostBorrowedBooksResponse> GetMostBorrowedBooks()
        {
            return await _borrowServiceClient.GetMostBorrowedBooksAsync(new MostBorrowedBooksRequest());
        }
    }
}
