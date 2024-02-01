using LibraryManagement.BorrowingGrpcService.Domains;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator
{
    public static class BorrowingsGenerator
    {
        public static List<Borrowing> GenerateBookBorrowings(List<Book> books, List<User> users)
        {
            var borrowings = new List<Borrowing>();

            foreach (var book in books)
            {
                foreach (var user in users)
                {
                    var borrowing = new Borrowing(book.Id, book, user.Id, null);
                    borrowings.Add(borrowing);
                }
            }

            return borrowings;
        }
    }
}
