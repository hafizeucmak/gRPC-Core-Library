using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;

namespace BorrowingGrpcService.Tests.Queries
{
    public class GetBookCopiesAvailabilityQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetMostBorrowedBooksQuery>> _mockLogger;
        private readonly GetBookCopiesAvailabilityQueryHandler _handler;

        public GetBookCopiesAvailabilityQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _mockLogger = new Mock<ILogger<GetMostBorrowedBooksQuery>>();

            _handler = new GetBookCopiesAvailabilityQueryHandler(
                _mockRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnAvailability_WhenBookExists()
        {
            var isbn = "123456";

            var book = new Book("Title", "Author", isbn, "Publisher", 2022, 300);

            var idProperty = book.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(book, 2);
            }

            var copy1 = new BookCopy(book.Id, book);
            var copy2 = new BookCopy(book.Id, book);
            copy1.UpdateStatusAsBorrowed();
            book.AddBookCopy(copy1);
            book.AddBookCopy(copy2);

            var books = new List<Book> { book };

            _mockRepository
                .Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(books.BuildMockDbSet().Object);

            var query = new GetBookCopiesAvailabilityQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.AvailableCopiesCount);
            Assert.Equal(1, result.BorrowedCopiesCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenBookDoesNotExist()
        {
            var isbn = "not_existing";

            var books = new List<Book>();
            _mockRepository
                .Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(books.BuildMockDbSet().Object);

            var query = new GetBookCopiesAvailabilityQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(0, result.AvailableCopiesCount);
            Assert.Equal(0, result.BorrowedCopiesCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroCounts_WhenBookHasNoCopies()
        {
            var isbn = "123456";
            var book = new Book("Title", "Author", isbn, "Publisher", 2022, 300);

            var idProperty = book.GetType().GetProperty("Id");
            idProperty?.SetValue(book, 1);

            var books = new List<Book> { book };
            _mockRepository
                .Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(books.BuildMockDbSet().Object);

            var query = new GetBookCopiesAvailabilityQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(0, result.AvailableCopiesCount);
            Assert.Equal(0, result.BorrowedCopiesCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllBorrowed_WhenAllCopiesAreBorrowed()
        {
            var isbn = "123456";
            var book = new Book("Title", "Author", isbn, "Publisher", 2022, 300);
            var idProperty = book.GetType().GetProperty("Id");
            idProperty?.SetValue(book, 1);

            var copy1 = new BookCopy(book.Id, book);
            var copy2 = new BookCopy(book.Id, book);
            copy1.UpdateStatusAsBorrowed();
            copy2.UpdateStatusAsBorrowed();
            book.AddBookCopy(copy1);
            book.AddBookCopy(copy2);

            var books = new List<Book> { book };
            _mockRepository
                .Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(books.BuildMockDbSet().Object);

            var query = new GetBookCopiesAvailabilityQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(0, result.AvailableCopiesCount);
            Assert.Equal(2, result.BorrowedCopiesCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllAvailable_WhenAllCopiesAreAvailable()
        {
            var isbn = "123456";
            var book = new Book("Title", "Author", isbn, "Publisher", 2022, 300);
            var idProperty = book.GetType().GetProperty("Id");
            idProperty?.SetValue(book, 1);

            var copy1 = new BookCopy(book.Id, book);
            var copy2 = new BookCopy(book.Id, book);
            book.AddBookCopy(copy1);
            book.AddBookCopy(copy2);

            var books = new List<Book> { book };
            _mockRepository
                .Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(books.BuildMockDbSet().Object);

            var query = new GetBookCopiesAvailabilityQuery(isbn);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.AvailableCopiesCount);
            Assert.Equal(0, result.BorrowedCopiesCount);
        }
    }
}
