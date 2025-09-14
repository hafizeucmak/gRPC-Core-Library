using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using MockQueryable.Moq;
using Moq;

namespace BorrowingGrpcService.Tests.Unit.Queries
{
    public class GetTopBorrowersWithinSpecifiedTimeframeQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly GetTopBorrowersWithinSpecifiedTimeframeQueryHandler _handler;

        public GetTopBorrowersWithinSpecifiedTimeframeQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _handler = new GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(
                _mockRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<GetTopBorrowersWithinSpecifiedTimeframeQuery>>());
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
            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");
            var book3 = CreateBook(3, "Book C", "Author", "Publisher", "333");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, user1.Id, null),
                new Borrowing(book2.Id, book2, user1.Id, null),
                new Borrowing(book3.Id, book3, user2.Id, null)
            };

            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[0], user1);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[1], user1);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[2], user2);

            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[0], DateTime.Now.AddDays(-5));
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[1], DateTime.Now.AddDays(-4));
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[2], DateTime.Now.AddDays(-3));

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoBorrowingsExist()
        {
            var borrowings = new List<Borrowing>();

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.TopBorrowers);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyBorrowingsWithinDateRange()
        {
            var user = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, user.Id, null),
                new Borrowing(book2.Id, book2, user.Id, null)
            };

            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[0], user);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[1], user);

            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[0], DateTime.Now.AddDays(-2));
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[1], DateTime.Now.AddDays(-20));

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result.TopBorrowers);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(1, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectOrder_WhenMultipleUsersBorrowed()
        {
            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, user1.Id, null),
                new Borrowing(book2.Id, book2, user1.Id, null),
                new Borrowing(book1.Id, book1, user2.Id, null)
            };

            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[0], user1);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[1], user1);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[2], user2);

            foreach (var b in borrowings)
                typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(b, DateTime.Now.AddDays(-1));

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Equal("Alice Smith", result.TopBorrowers.First().UserName);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
            Assert.Equal("Bob Johnson", result.TopBorrowers.Last().UserName);
            Assert.Equal(1, result.TopBorrowers.Last().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldIncludeBorrowingsOnStartAndEndDate()
        {
            var user = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var book = CreateBook(1, "Book A", "Author", "Publisher", "111");

            var startDate = DateTime.Now.AddDays(-5).Date;
            var endDate = DateTime.Now.Date;

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book.Id, book, user.Id, null),
                new Borrowing(book.Id, book, user.Id, null)
            };

            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[0], user);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowings[1], user);

            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[0], startDate);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowings[1], endDate);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(startDate, endDate);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result.TopBorrowers);
            Assert.Equal(2, result.TopBorrowers.First().BorrowedBookCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnMultipleUsersWithSameBorrowCount_CorrectOrder()
        {
            var user1 = CreateUser(1, "Alice Smith", "Alice", "Smith", "alice@test.com");
            var user2 = CreateUser(2, "Bob Johnson", "Bob", "Johnson", "bob@test.com");

            var book1 = CreateBook(1, "Book A", "Author", "Publisher", "111");
            var book2 = CreateBook(2, "Book B", "Author", "Publisher", "222");

            var borrowing1 = new Borrowing(book1.Id, book1, user1.Id, null);
            var borrowing2 = new Borrowing(book2.Id, book2, user2.Id, null);

            typeof(Borrowing).GetProperty("User")!.SetValue(borrowing1, user1);
            typeof(Borrowing).GetProperty("User")!.SetValue(borrowing2, user2);

            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing1, DateTime.Now.AddDays(-5));
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-5));

            var borrowings = new List<Borrowing> { borrowing1, borrowing2 };

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var query = new GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime.Now.AddDays(-10), DateTime.Now);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.TopBorrowers.Count);
            Assert.Contains(result.TopBorrowers, x => x.UserName == "Alice Smith");
            Assert.Contains(result.TopBorrowers, x => x.UserName == "Bob Johnson");
            Assert.All(result.TopBorrowers, x => Assert.Equal(1, x.BorrowedBookCount));
        }
    }
}
