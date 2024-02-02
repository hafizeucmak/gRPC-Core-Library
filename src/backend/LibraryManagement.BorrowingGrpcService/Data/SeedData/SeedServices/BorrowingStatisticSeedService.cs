using Bogus;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator;
using LibraryManagement.BorrowingGrpcService.Domains.Enums;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedServices
{
    public class BorrowingStatisticSeedService : BaseSeedService
    {
        private static Faker _faker = new Faker();
        private readonly int _userCount = 10;
        private readonly int _bookCount = 5;
        private readonly int _bookCopyCount = 5;

        public BorrowingStatisticSeedService(BorrowingBaseDbContext dbContext) : base(dbContext)
        {
        }

        public override string Name => throw new NotImplementedException();

        public override string Description => throw new NotImplementedException();
        public async override Task Execute()
        {
            await GenerateReturnedBorrowings();
            await GenerateUnReturnedBorrowings();
        }

        public async Task GenerateReturnedBorrowings()
        {
            var users = UserGenerator.GenerateUser(_userCount);
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var books = AssetsGenerator.GenerateBooks(_faker.Random.Number(1, _bookCount));
            await _dbContext.Books.AddRangeAsync(books);
            await _dbContext.SaveChangesAsync();

            var bookCopies = AssetsGenerator.GenerateCopiesOfBooks(books, _faker.Random.Number(1, _bookCopyCount));
            await _dbContext.BookCopies.AddRangeAsync(bookCopies);
            await _dbContext.SaveChangesAsync();

            var bookBorrowings = BorrowingsGenerator.GenerateBookBorrowings(books, users, true);
            await _dbContext.Borrowings.AddRangeAsync(bookBorrowings);

            foreach (var book in books)
            {
                if (book.Status != AssetStatus.Available)
                    book.UpdateStatusAsAvailable();
            }

            _dbContext.Books.UpdateRange(books);
            await _dbContext.SaveChangesAsync();

            var bookCopyBorrowings = BorrowingsGenerator.GenerateBookCopyBorrowings(bookCopies, users, true);
            await _dbContext.Borrowings.AddRangeAsync(bookCopyBorrowings);

            foreach (var bookCopy in bookCopies)
            {
                if (bookCopy.Status != AssetStatus.Available)
                    bookCopy.UpdateStatusAsAvailable();
            }

            _dbContext.BookCopies.UpdateRange(bookCopies);
            await _dbContext.SaveChangesAsync();
        }

        public async Task GenerateUnReturnedBorrowings()
        {
            var users = UserGenerator.GenerateUser(_userCount);
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var books = AssetsGenerator.GenerateBooks(_faker.Random.Number(1, _bookCount), _dbContext.Books.Count() + 1);
            await _dbContext.Books.AddRangeAsync(books);
            await _dbContext.SaveChangesAsync();

            var bookCopies = AssetsGenerator.GenerateCopiesOfBooks(books, _faker.Random.Number(1, _bookCopyCount));
            await _dbContext.BookCopies.AddRangeAsync(bookCopies);
            await _dbContext.SaveChangesAsync();

            var bookBorrowings = BorrowingsGenerator.GenerateBookBorrowings(books, users, true);
            await _dbContext.Borrowings.AddRangeAsync(bookBorrowings);

            foreach (var book in books)
            {
                if (book.Status != AssetStatus.Borrowed)
                    book.UpdateStatusAsBorrowed();
            }

            _dbContext.Books.UpdateRange(books);
            await _dbContext.SaveChangesAsync();

            var bookCopyBorrowings = BorrowingsGenerator.GenerateBookCopyBorrowings(bookCopies, users, true);
            await _dbContext.Borrowings.AddRangeAsync(bookCopyBorrowings);

            foreach (var bookCopy in bookCopies)
            {
                if (bookCopy.Status != AssetStatus.Borrowed)
                    bookCopy.UpdateStatusAsBorrowed();
            }

            _dbContext.BookCopies.UpdateRange(bookCopies);
            await _dbContext.SaveChangesAsync();
        }
    }
}
