using LibraryManagement.BorrowingGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public interface IBorrowingServiceClient
    {
        Task<BorrowBookResponse> BorrowBookAsync(BorrowBookRequest request);

        Task<MostBorrowedBooksResponse> GetMostBorrowedBooks();

        Task GetBookAvailability();

        Task GetTopBorrowers();

        Task GetBorrowedBooksByUser();

        Task GetRelatedBooks();

        Task GetReadRate();
    }
}
