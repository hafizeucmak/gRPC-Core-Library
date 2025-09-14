using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace BorrowingGrpcService.Tests.Integration.Queries
{
    public class GetTopBorrowersWithinSpecifiedTimeframeQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>> _mockLogger;
        private readonly GetTopBorrowersWithinSpecifiedTimeframeQueryHandler _handler;

        public GetTopBorrowersWithinSpecifiedTimeframeQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _mockLogger = new Mock<ILogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>>();
            _handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        private User CreateUser(int id, string fullName, string firstName, string lastName, string email)
        {
            var user = new User(email, fullName, firstName, lastName, "1234567890");
            typeof(User).GetProperty("Id")!.SetValue(user, id);
            return user;
        }

        private Book CreateBook(int id, string title, string author, string publisher, string isbn)
        {
            var book = new Book(title, author, isbn, publisher, 2023, 100);
            typeof(Book).GetProperty("Id")!.SetValue(book, id);
            return book;
        }

        [Fact]
        public async Task Handle_ShouldReturnTopBorrowers_WhenBorrowingsExist()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                              .LogTo(Console.WriteLine, LogLevel.Information)
                              .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                              .Options;

            using var context = new BorrowingBaseDbContext(options);

            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");
            var book3 = CreateBook(3, "Book C", "Author", "Publisher", "333");
            context.Books.AddRange(book1, book2, book3);
            await context.SaveChangesAsync();

            var borrowing1 = new Borrowing(book1.Id, book1, user1.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing1, DateTime.Now.AddDays(-5));
            var borrowing2 = new Borrowing(book2.Id, book2, user1.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-4));
            var borrowing3 = new Borrowing(book3.Id, book3, user2.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing3, DateTime.Now.AddDays(-3));

            context.Borrowings.AddRange(borrowing1, borrowing2, borrowing3);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoBorrowingsExist()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.TopBorrowers);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyBorrowingsWithinDateRange()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var user = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            context.Users.Add(user);

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");
            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var borrowingInside = new Borrowing(book1.Id, book1, user.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowingInside, DateTime.Now.AddDays(-2));

            var borrowingOutside = new Borrowing(book2.Id, book2, user.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowingOutside, DateTime.Now.AddDays(-20));

            context.Borrowings.AddRange(borrowingInside, borrowingOutside);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result.TopBorrowers);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(1, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectOrder_WhenMultipleUsersBorrowed()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");
            context.Users.AddRange(user1, user2);

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");
            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, user1.Id, null),
                new Borrowing(book2.Id, book2, user1.Id, null),
                new Borrowing(book1.Id, book1, user2.Id, null)
            };

            foreach (var b in borrowings)
                typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(b, DateTime.Now.AddDays(-1));

            context.Borrowings.AddRange(borrowings);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
            Assert.Equal("Bob Johnson", result.TopBorrowers.Last().UserName);
            Assert.Equal(1, result.TopBorrowers.Last().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldIncludeBorrowingsOnStartAndEndDate()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var user = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            context.Users.Add(user);

            var book = CreateBook(1, "Book A", "Author", "Publisher", "111");
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var startDate = DateTime.Now.AddDays(-5).Date;
            var endDate = DateTime.Now.Date;

            var borrowingOnStart = new Borrowing(book.Id, book, user.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowingOnStart, startDate);

            var borrowingOnEnd = new Borrowing(book.Id, book, user.Id, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowingOnEnd, endDate);

            context.Borrowings.AddRange(borrowingOnStart, borrowingOnEnd);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(startDate, endDate);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result.TopBorrowers);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnMultipleUsersWithSameBorrowCount_CorrectOrder()
        {
            var options = new DbContextOptionsBuilder<BorrowingBaseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            using var context = new BorrowingBaseDbContext(options);

            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");
            context.Users.AddRange(user1, user2);

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");
            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, user1.Id, null),
                new Borrowing(book2.Id, book2, user2.Id, null)
            };

            foreach (var b in borrowings)
                typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(b, DateTime.Now.AddDays(-1));

            context.Borrowings.AddRange(borrowings);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var logger = new LoggerFactory().CreateLogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>();
            var handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(repository, logger);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Contains(result.TopBorrowers, x => x.UserName == "Alice Smith");
            Assert.Contains(result.TopBorrowers, x => x.UserName == "Bob Johnson");
            Assert.All(result.TopBorrowers, x => Assert.Equal(1, x.BorrowedBookCount));
        }
    }
}
