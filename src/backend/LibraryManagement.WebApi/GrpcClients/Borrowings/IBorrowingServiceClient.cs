using LibraryManagement.BorrowingGrpcService;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public interface IBorrowingServiceClient
    {
        Task<BorrowBookResponse> BorrowBookAsync(BorrowBookRequest request);

        Task<MostBorrowedBooksResponse> GetMostBorrowedBooks(MostBorrowedBooksRequest request);

        Task<BookCopiesAvailabilityResponse> GetBookCopiesAvailability(BookCopiesAvailabilityRequest request);

        Task<TopBorrowersResponse> GetTopBorrowersWithinSpecifiedTimeframe(TopBorrowersRequest request);

        Task<BorrowedBooksByUserResponse> GetBorrowedBooksByUser(BorrowedBooksByUserRequest request);

        Task GetRelatedBooks();

        Task GetReadRate();
    }
}
