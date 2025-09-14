using DynamicQueryBuilder.Models;
using LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BorrowingGrpcService.Tests.Integration.Queries
{
    public class GetMostBorrowedBooksQueryTests
    {
        private Book CreateBook(int id, string title, string author, string publisher, string isbn)
        {
            var book = new Book(title, author, isbn, publisher, 2023, 200);
            var idProp = typeof(Book).GetProperty("Id");
            idProp?.SetValue(book, id);
            return book;
        }

        private Borrowing CreateBorrowing(int bookId, Book book, int userId)
        {
            return new Borrowing(bookId, book, userId, null);
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
        public async Task Handle_ShouldReturnMostBorrowedBooks_WhenBorrowingsExist()
        {
            using var context = CreateDbContext();

            var book1 = CreateBook(1, "Book One", "Author A", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "Author B", "Publisher B", "222");

            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var borrowings = new List<Borrowing>
            {
                CreateBorrowing(book1.Id, book1, 100),
                CreateBorrowing(book1.Id, book1, 101),
                CreateBorrowing(book2.Id, book2, 102)
            };

            context.Borrowings.AddRange(borrowings);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetMostBorrowedBooksQueryHandler(repository, new LoggerFactory().CreateLogger<GetMostBorrowedBooksQuery>());

            var queryOptions = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetMostBorrowedBooksQuery(queryOptions);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.MostBorrowedBooks.Count);
            Assert.Equal("Book One", result.MostBorrowedBooks.First().Title);
            Assert.Equal(2, result.MostBorrowedBooks.First().BorrowedCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoBorrowingsExist()
        {
            using var context = CreateDbContext();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetMostBorrowedBooksQueryHandler(repository, new LoggerFactory().CreateLogger<GetMostBorrowedBooksQuery>());

            var queryOptions = JsonConvert.SerializeObject(new DynamicQueryOptions());
            var query = new GetMostBorrowedBooksQuery(queryOptions);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result.MostBorrowedBooks);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredBooks_WhenDynamicQueryOptionsProvided()
        {
            using var context = CreateDbContext();

            var book1 = CreateBook(1, "Book One", "AuthorA", "Publisher A", "111");
            var book2 = CreateBook(2, "Book Two", "AuthorB", "Publisher B", "222");

            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var borrowings = new List<Borrowing>
            {
                CreateBorrowing(book1.Id, book1, 100),
                CreateBorrowing(book2.Id, book2, 101)
            };

            context.Borrowings.AddRange(borrowings);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetMostBorrowedBooksQueryHandler(repository, new LoggerFactory().CreateLogger<GetMostBorrowedBooksQuery>());

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

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result.MostBorrowedBooks);
            var filteredBook = result.MostBorrowedBooks.First();
            Assert.Equal("Book Two", filteredBook.Title);
            Assert.Equal("AuthorB", filteredBook.Author);
            Assert.Equal(1, filteredBook.BorrowedCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnFirstPage_WhenPaginationApplied()
        {
            using var context = CreateDbContext();

            var books = new List<Book>();
            for (int i = 1; i <= 10; i++)
            {
                var book = CreateBook(i, $"Book {i}", "CommonAuthor", "CommonPublisher", $"{i:000}");
                books.Add(book);
            }

            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            var borrowings = books.Select(book =>
                CreateBorrowing(book.Id, book, 100 + book.Id)).ToList();

            context.Borrowings.AddRange(borrowings);
            await context.SaveChangesAsync();

            var repository = new GenericWriteRepository<BorrowingBaseDbContext>(context);
            var handler = new GetMostBorrowedBooksQueryHandler(repository, new LoggerFactory().CreateLogger<GetMostBorrowedBooksQuery>());

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

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(5, result.MostBorrowedBooks.Count);
        }
    }
}
