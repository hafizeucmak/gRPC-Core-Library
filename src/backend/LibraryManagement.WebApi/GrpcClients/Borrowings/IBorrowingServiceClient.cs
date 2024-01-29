using LibraryManagement.WebApi.Models;

namespace LibraryManagement.WebApi.GrpcClients.Borrows
{
    public interface IBorrowingServiceClient
    {
        Task<IEnumerable<MostBorrowedBooksDTO>> GetMostBorrowedBooks();

        Task GetBookAvailability();

        Task GetTopBorrowers();

        Task GetBorrowedBooksByUser();

        Task GetRelatedBooks();

        Task GetReadRate();
    }
}
