using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace BorrowingGrpcService.Tests.Unit.Queries
{
    public class GetAverageReadRateForBookQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetAverageReadRateForBookQuery>> _mockLogger;
        private readonly GetAverageReadRateForBookQueryHandler _handler;

        public GetAverageReadRateForBookQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _mockLogger = new Mock<ILogger<GetAverageReadRateForBookQuery>>();
            _handler = new GetAverageReadRateForBookQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBookDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .LogTo(Console.WriteLine, LogLevel.Information)
               .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;

            using var context = new BorrowingBaseDbContext(options);

            var book = new Book("Test Book", "Author", "12345", "Publisher", 2022, 100);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>();
            var handler = new GetAverageReadRateForBookQueryHandler(repository, logger);

            var query = new GetAverageReadRateForBookQuery("nonexistent-isbn");

            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>  handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBookNeverBorrowed()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .LogTo(Console.WriteLine, LogLevel.Information)
             .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
             .Options;

            using var context = new BorrowingBaseDbContext(options);

            var book = new Book("Never Borrowed", "Author", "00001", "Publisher", 2023, 150);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>();
            var handler = new GetAverageReadRateForBookQueryHandler(repository, logger);

            var query = new GetAverageReadRateForBookQuery("00001");

            var ex = await Assert.ThrowsAsync<ResourceNotFoundException>(() =>  handler.Handle(query, CancellationToken.None));

            Assert.Contains("never borrowed", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenBorrowDaysAreZero()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .LogTo(Console.WriteLine, LogLevel.Information)
                               .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                               .Options;

            using var context = new BorrowingBaseDbContext(options);

            var book = new Book("Instant Return", "Author", "00002", "Publisher", 2023, 50);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var borrowing1 = new Borrowing(book.Id, book, 1, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing1, DateTime.Now);
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing1, DateTime.Now);

            context.Borrowings.AddRange(borrowing1);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>();
            var handler = new GetAverageReadRateForBookQueryHandler(repository, logger);

            var query = new GetAverageReadRateForBookQuery("00002");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.ReadRate);
        }


        [Fact]
        public async Task Handle_ShouldReturnCorrectReadRate_WhenBookExistsAndBorrowed()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .LogTo(Console.WriteLine, LogLevel.Information)
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var book = new Book("Test Book", "Author", "12345", "Publisher", 2022, 100);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            context.Books.Add(book);
            await context.SaveChangesAsync();

            var borrowing1 = new Borrowing(book.Id, book, 1, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing1, DateTime.Now.AddDays(-5));
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing1, DateTime.Now);

            var borrowing2 = new Borrowing(book.Id, book, 2, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-10));
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-5));
            context.Borrowings.AddRange(borrowing1, borrowing2);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetAverageReadRateForBookQuery>();
            var handler = new GetAverageReadRateForBookQueryHandler(repository, logger);

            var query = new GetAverageReadRateForBookQuery("12345");

            var result = await handler.Handle(query, CancellationToken.None);

            // totalBorrowDays = 5 + 5 = 10
            // totalReadPageCount = 100 * 2 = 200
            // expectedReadRate = 200 / 10 = 20
            Assert.Equal(20, result.ReadRate);
        }

    }
}
