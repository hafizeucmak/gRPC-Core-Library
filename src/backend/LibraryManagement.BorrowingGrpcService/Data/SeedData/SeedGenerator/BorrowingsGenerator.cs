using Bogus;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.Constants;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator
{
    public static class BorrowingsGenerator
    {
        private static Faker _faker = new Faker();

        // The number of potential borrowings calculated by selecting random books for each user
        public static List<Borrowing> GenerateBookBorrowings(List<Book> books, List<User> users, bool isReturned = false)
        {
            var borrowings = new List<Borrowing>();

            foreach (var user in users)
            {
                var selectedBooks = _faker.PickRandom<Book>(books, _faker.Random.Number(1, books.Count()));

                foreach (var book in selectedBooks)
                {
                    var borrowing = new Borrowing(book.Id, book, user.Id, null);
                    if (isReturned)
                    {
                        var borrowDate = _faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now);
                        borrowing.GetType()?.GetProperty(nameof(Borrowing.BorrowDate))?.SetValue(borrowing, borrowDate);

                        var expectedReturnDate = borrowDate.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay);
                        borrowing.GetType()?.GetProperty(nameof(Borrowing.ExpectedReturnDate))?.SetValue(borrowing, expectedReturnDate);

                        var returnDate = _faker.Date.Between(borrowDate, borrowDate.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay));
                        borrowing.Returned(returnDate);
                    }
                    borrowings.Add(borrowing);
                }
            }

            return borrowings;
        }

        // The number of potential borrowings calculated by selecting random book copies for each user
        public static List<Borrowing> GenerateBookCopyBorrowings(List<BookCopy> bookCopies, List<User> users, bool isReturned = false)
        {
            var borrowings = new List<Borrowing>();

            foreach (var user in users)
            {
                var selectedBooks = _faker.PickRandom<BookCopy>(bookCopies, _faker.Random.Number(1, bookCopies.Count()));

                foreach (var bookCopy in selectedBooks)
                {
                    var borrowing = new Borrowing(bookCopy.BookId, bookCopy.Book, user.Id, bookCopy.Id);
                    if (isReturned)
                    {
                        var borrowDate = _faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now);
                        borrowing.GetType()?.GetProperty(nameof(Borrowing.BorrowDate))?.SetValue(borrowing, borrowDate);

                        var expectedReturnDate = borrowDate.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay);
                        borrowing.GetType()?.GetProperty(nameof(Borrowing.ExpectedReturnDate))?.SetValue(borrowing, expectedReturnDate);

                        var returnDate = _faker.Date.Between(borrowDate, borrowDate.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay));
                        borrowing.Returned(returnDate);
                    }
                    borrowings.Add(borrowing);
                }
            }

            return borrowings;
        }
    }
}
