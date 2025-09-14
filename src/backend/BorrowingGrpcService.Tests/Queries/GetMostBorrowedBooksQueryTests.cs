using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;

namespace BorrowingGrpcService.Tests.Unit.Queries
{
    public class GetMostBorrowedBooksQueryTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetMostBorrowedBooksQuery>> _mockLogger;
        private readonly GetMostBorrowedBooksQueryHandler _handler;

        public GetMostBorrowedBooksQueryTests()
        {
            _mockRepository = new Mock<IGenericWriteRepository<BorrowingBaseDbContext>>();
            _mockLogger = new Mock<ILogger<GetMostBorrowedBooksQuery>>();
            _handler = new GetMostBorrowedBooksQueryHandler(_mockRepository.Object, _mockLogger.Object);
        }

        private Book CreateBook(int id, string title, string author, string publisher, string isbn)
        {
            var book = new Book(title, author, isbn, publisher, 2023, 200);
            var idProperty = book.GetType().GetProperty("Id");
            idProperty?.SetValue(book, id);
            return book;
        }

        [Fact]
        public async Task Handle_ShouldReturnMostBorrowedBooks_WhenBorrowingsExist()
        {
            var book1 = CreateBook(1, "Book One", "Author A", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "Author B", "Publisher B", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, 100, null),
                new Borrowing(book1.Id, book1, 101, null),
                new Borrowing(book2.Id, book2, 102, null)
            };

            _mockRepository
                .Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var queryOptions = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetMostBorrowedBooksQuery(queryOptions);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotEmpty(result.MostBorrowedBooks);
            Assert.Equal(2, result.MostBorrowedBooks.Count);

            var topBorrowed = result.MostBorrowedBooks.First();
            Assert.Equal("Book One", topBorrowed.Title);
            Assert.Equal(2, topBorrowed.BorrowedCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoBorrowingsExist()
        {
            var borrowings = new List<Borrowing>();

            _mockRepository
                .Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var queryOptions = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetMostBorrowedBooksQuery(queryOptions);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.MostBorrowedBooks);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredBooks_WhenDynamicQueryOptionsProvided()
        {
            var book1 = CreateBook(1, "Book One", "AuthorA", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "AuthorB", "Publisher B", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, 100, null),
                new Borrowing(book2.Id, book2, 101, null)
            };

            _mockRepository
                .Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var dynamicQueryOptions = new DynamicQueryOptions
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        PropertyName = "Author",
                        Operator = DynamicQueryBuilder.Models.Enums.FilterOperation.Equals,
                        LogicalOperator = DynamicQueryBuilder.Models.Enums.LogicalOperator.OrElse,
                        Value = "AuthorB",
                        CaseSensitive = true
                    }
                },
                SortOptions = new List<SortOption>
                {
                    new SortOption
                    {
                        PropertyName = "BorrowedCount",
                        SortingDirection = DynamicQueryBuilder.Models.Enums.SortingDirection.Desc
                    }
                },
                PaginationOption = new PaginationOption
                {
                    Offset = 0,
                    Count = 10
                }
            };

            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);
            var query = new GetMostBorrowedBooksQuery(queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Single(result.MostBorrowedBooks);
            var filteredBook = result.MostBorrowedBooks.First();
            Assert.Equal("Book Two", filteredBook.Title);
            Assert.Equal("AuthorB", filteredBook.Author);
            Assert.Equal(1, filteredBook.BorrowedCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnFirstPage_WhenPaginationApplied()
        {
            var books = new List<Book>();
            var borrowings = new List<Borrowing>();

            for (int i = 1; i <= 10; i++)
            {
                var book = CreateBook(i, $"Book {i}", "CommonAuthor", "CommonPublisher", $"{i:000}");
                books.Add(book);
                borrowings.Add(new Borrowing(book.Id, book, 100 + book.Id, null));
            }

            _mockRepository
                .Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(borrowings.BuildMockDbSet().Object);

            var dynamicQueryOptions = new DynamicQueryOptions
            {
                PaginationOption = new PaginationOption
                {
                    Offset = 0,
                    Count = 5
                }
            };

            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);
            var query = new GetMostBorrowedBooksQuery(queryOptionsJson);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(5, result.MostBorrowedBooks.Count);
        }
    }
}
