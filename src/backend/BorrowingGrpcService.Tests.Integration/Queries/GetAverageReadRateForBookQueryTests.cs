using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BorrowingGrpcService.Tests.Integration.Queries
{
    public class GetAverageReadRateForBookQueryTests
    {
        private Book CreateBook(int id, string title, string author, string publisher, string isbn, int pages = 100)
        {
            var book = new Book(title, author, isbn, publisher, 2023, pages);
            typeof(Book).GetProperty("Id")!.SetValue(book, id);
            return book;
        }

        private Borrowing CreateBorrowing(int bookId, Book book, int userId, DateTime borrowDate, DateTime? returnDate)
        {
            var borrowing = new Borrowing(bookId, book, userId, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing, borrowDate);
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing, returnDate);
            return borrowing;
        }

        private BorrowingBaseDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;

            return new BorrowingBaseDbContext(options);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBookDoesNotExist()
        {
            using var context = CreateDbContext();
            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>();
            var handler = new GetAverageReadRateForBookQueryHandler(repository, logger);

            var query = new GetAverageReadRateForBookQuery("nonexistent-isbn");

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenBookNeverBorrowed()
        {
            using var context = CreateDbContext();

            var book = CreateBook(1, "Never Borrowed", "Author", "Publisher", "00001");
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetAverageReadRateForBookQueryHandler(repository, new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>());

            var query = new GetAverageReadRateForBookQuery("00001");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.ReadRate);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenBorrowDaysAreZero()
        {
            using var context = CreateDbContext();

            var book = CreateBook(1, "Instant Return", "Author", "Publisher", "00002", 50);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var now = DateTime.Now;
            var borrowing = CreateBorrowing(book.Id, book, 1, now, now);
            context.Borrowings.Add(borrowing);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetAverageReadRateForBookQueryHandler(repository, new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>());

            var query = new GetAverageReadRateForBookQuery("00002");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.ReadRate);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectReadRate_WhenBookExistsAndBorrowed()
        {
            using var context = CreateDbContext();

            var book = CreateBook(1, "Test Book", "Author", "Publisher", "12345", 100);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var borrowing1 = CreateBorrowing(book.Id, book, 1, DateTime.Now.AddDays(-5), DateTime.Now);
            var borrowing2 = CreateBorrowing(book.Id, book, 2, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5));

            context.Borrowings.AddRange(borrowing1, borrowing2);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetAverageReadRateForBookQueryHandler(repository, new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>());

            var query = new GetAverageReadRateForBookQuery("12345");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(20, result.ReadRate);
        }
    }
}