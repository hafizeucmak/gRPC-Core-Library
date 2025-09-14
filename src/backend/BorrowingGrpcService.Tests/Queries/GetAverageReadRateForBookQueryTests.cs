using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
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
            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Book>().BuildMockDbSet().Object);

            var query = new GetAverageReadRateForBookQuery("nonexistent-isbn");

            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenBookNeverBorrowed()
        {
            var book = new Book("Never Borrowed", "Author", "00001", "Publisher", 2023, 150);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Book> { book }.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Borrowing>().BuildMockDbSet().Object);

            var query = new GetAverageReadRateForBookQuery("00001");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.ReadRate);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenBorrowDaysAreZero()
        {
            var book = new Book("Instant Return", "Author", "00002", "Publisher", 2023, 50);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            var borrowing = new Borrowing(book.Id, book, 1, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing, DateTime.Now);
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing, DateTime.Now);

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Book> { book }.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Borrowing> { borrowing }.BuildMockDbSet().Object);

            var query = new GetAverageReadRateForBookQuery("00002");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.ReadRate);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectReadRate_WhenBookExistsAndBorrowed()
        {
            var book = new Book("Test Book", "Author", "12345", "Publisher", 2022, 100);
            typeof(Book).GetProperty("Id")!.SetValue(book, 1);

            var borrowing1 = new Borrowing(book.Id, book, 1, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing1, DateTime.Now.AddDays(-5));
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing1, DateTime.Now);

            var borrowing2 = new Borrowing(book.Id, book, 2, null);
            typeof(Borrowing).GetProperty("BorrowDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-10));
            typeof(Borrowing).GetProperty("ReturnDate")!.SetValue(borrowing2, DateTime.Now.AddDays(-5));

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Book> { book }.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<Borrowing> { borrowing1, borrowing2 }.BuildMockDbSet().Object);

            var query = new GetAverageReadRateForBookQuery("12345");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(20, result.ReadRate);
        }
    }
}