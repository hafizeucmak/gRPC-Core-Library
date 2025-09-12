using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;

namespace BorrowingGrpcService.Tests.Queries
{
    public class GetBorrowersAlsoBorrowedBooksQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetBorrowersAlsoBorrowedBooksQuery>> _mockLogger;
        private readonly GetBorrowersAlsoBorrowedBooksQueryHandler _handler;

        public GetBorrowersAlsoBorrowedBooksQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _mockLogger = new Mock<ILogger<GetBorrowersAlsoBorrowedBooksQuery>>();
            _handler = new GetBorrowersAlsoBorrowedBooksQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        private Book CreateBook(int id, string title, string author, string publisher, string isbn)
        {
            var book = new Book(title, author, isbn, publisher, 2023, 200);
            book.GetType().GetProperty("Id")!.SetValue(book, id);
            return book;
        }

        [Fact]
        public async Task Handle_ShouldReturnAlsoBorrowedBooks_WhenOtherBooksBorrowedBySameUsers()
        {
            var book1 = CreateBook(1, "Book One", "Author A", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "Author B", "Publisher B", "222");
            var book3 = CreateBook(3, "Book Three", "Author C", "Publisher C", "333");

            var borrowings = new List<Borrowing>
            {
                // User 1 borrowed Book One and Book Two
                new Borrowing(book1.Id, book1, 1, null),
                new Borrowing(book2.Id, book2, 1, null),
                // User 2 borrowed Book One and Book Three
                new Borrowing(book1.Id, book1, 2, null),
                new Borrowing(book3.Id, book3, 2, null),
                // User 3 borrowed Book Three only
                new Borrowing(book3.Id, book3, 3, null)
            };

            var books = new List<Book> { book1, book2, book3 };
            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(books.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(borrowings.BuildMockDbSet().Object);

            var dynamicQueryOptions = new DynamicQueryOptions();
            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);

            var query = new GetBorrowersAlsoBorrowedBooksQuery("111", queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.AlsoBorrowedBooks.Count);

            var titles = result.AlsoBorrowedBooks.Select(x => x.Title).ToList();
            Assert.Contains("Book Two", titles);   // borrowed by User 1
            Assert.Contains("Book Three", titles); // borrowed by User 2
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoOtherBooksBorrowed()
        {
            var book1 = CreateBook(1, "Book One", "Author A", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "Author B", "Publisher B", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, 1, null),
                new Borrowing(book2.Id, book2, 2, null)
            };

            var books = new List<Book> { book1, book2 };

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(books.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(borrowings.BuildMockDbSet().Object);

            var queryOptionsJson = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetBorrowersAlsoBorrowedBooksQuery("111", queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.AlsoBorrowedBooks);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBookDoesNotExist()
        {
            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(new List<Book>().BuildMockDbSet().Object);

            var queryOptionsJson = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetBorrowersAlsoBorrowedBooksQuery("nonexistent-isbn", queryOptionsJson);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldApplyFiltersCorrectly()
        {
            var book1 = CreateBook(1, "Book One", "Author A", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "Author B", "Publisher B", "222");
            var book3 = CreateBook(3, "Book Three", "Author C", "Publisher C", "333");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, 1, null),
                new Borrowing(book2.Id, book2, 1, null),
                new Borrowing(book3.Id, book3, 2, null)
            };
            var books = new List<Book> { book1, book2, book3 };

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(books.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(borrowings.BuildMockDbSet().Object);

            var dynamicQueryOptions = new DynamicQueryOptions
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        PropertyName = "Author",
                        Operator = DynamicQueryBuilder.Models.Enums.FilterOperation.Equals,
                        Value = "Author B",
                        LogicalOperator = DynamicQueryBuilder.Models.Enums.LogicalOperator.OrElse,
                        CaseSensitive = true
                    }
                }
            };

            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);
            var query = new GetBorrowersAlsoBorrowedBooksQuery("111", queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result.AlsoBorrowedBooks);
            Assert.Equal("Book Two", result.AlsoBorrowedBooks.First().Title);
        }

        [Fact]
        public async Task Handle_ShouldReturnPagedAlsoBorrowedBooks_WhenPaginationApplied()
        {
            var bookMain = CreateBook(1, "Book Main", "Author A", "Publisher A", "111");
            var otherBooks = new List<Book>
            {
                CreateBook(2, "Book 2", "Author B", "Publisher B", "222"),
                CreateBook(3, "Book 3", "Author C", "Publisher C", "333"),
                CreateBook(4, "Book 4", "Author D", "Publisher D", "444"),
                CreateBook(5, "Book 5", "Author E", "Publisher E", "555")
            };

            var borrowings = new List<Borrowing>
            {
                new Borrowing(bookMain.Id, bookMain, 1, null)
            };

            borrowings.AddRange(otherBooks.Select((b, index) => new Borrowing(b.Id, b, 1, null)));

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(otherBooks.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Book>(It.IsAny<string>(), It.IsAny<string>()))
                  .Returns(new List<Book> { bookMain }.BuildMockDbSet().Object);

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(borrowings.BuildMockDbSet().Object);

            var dynamicQueryOptions = new DynamicQueryOptions
            {
                PaginationOption = new PaginationOption
                {
                    Offset = 0,
                    Count = 2
                }
            };

            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);
            var query = new GetBorrowersAlsoBorrowedBooksQuery("111", queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.AlsoBorrowedBooks.Count);

            var titles = result.AlsoBorrowedBooks.Select(x => x.Title).ToList();
            Assert.Contains("Book 2", titles);
            Assert.Contains("Book 3", titles);
        }
    }
}
