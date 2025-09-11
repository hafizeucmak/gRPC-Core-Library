using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Newtonsoft.Json;

namespace BorrowingGrpcService.Tests.Queries
{
    public class GetMostBorrowedBooksQueryHandlerTests
    {
        private readonly Mock<IGenericWriteRepository<BorrowingBaseDbContext>> _mockRepository;
        private readonly Mock<ILogger<GetMostBorrowedBooksQuery>> _mockLogger;
        private readonly GetMostBorrowedBooksQueryHandler _handler;

        public GetMostBorrowedBooksQueryHandlerTests()
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
            // Arrange
            var book1 = CreateBook(1, "Book One", "AuthorA", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "AuthorB", "Publisher B", "222");

            var borrowings = new List<Borrowing>
            {
                new Borrowing(book1.Id, book1, 100, null),
                new Borrowing(book2.Id, book2, 101, null)
            };

            _mockRepository.Setup(r => r.GetAll<Borrowing>(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(borrowings.BuildMockDbSet().Object);

            // Proper DynamicQueryOptions object
            var dynamicQueryOptions = new DynamicQueryOptions
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        PropertyName = "Author",
                        Operator =  DynamicQueryBuilder.Models.Enums.FilterOperation.Equals,
                        LogicalOperator = DynamicQueryBuilder.Models.Enums.LogicalOperator.OrElse,
                        Value = "AuthorB",
                        CaseSensitive = false
                    }
                },
                //SortOptions = new List<SortOption>
                //{
                //    new SortOption
                //    {
                //         PropertyName = "BorrowedCount",
                //        SortingDirection =  DynamicQueryBuilder.Models.Enums.SortingDirection.Desc
                //    }
                //},
                PaginationOption = new PaginationOption
                {
                    Offset = 0,
                    Count = 10
                }
            };

            var queryOptionsJson = JsonConvert.SerializeObject(dynamicQueryOptions);
            var query = new GetMostBorrowedBooksQuery(queryOptionsJson);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result.MostBorrowedBooks);
            var filteredBook = result.MostBorrowedBooks.First();
            Assert.Equal("Book Two", filteredBook.Title);
            Assert.Equal("Author B", filteredBook.Author);
            Assert.Equal(1, filteredBook.BorrowedCount);
        }




    }

}
