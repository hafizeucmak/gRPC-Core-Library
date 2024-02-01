using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedServices
{
    public class BorrowingStatisticSeedService : BaseSeedService
    {
        public BorrowingStatisticSeedService(BorrowingBaseDbContext dbContext) : base(dbContext)
        {
        }

        public override string Name => throw new NotImplementedException();

        public override string Description => throw new NotImplementedException();
        public async override Task Execute()
        {

            var books = AssetsGenerator.GenerateBooks(50);
            await _dbContext.Books.AddRangeAsync(books);
            await _dbContext.SaveChangesAsync();


            var bookCopies = AssetsGenerator.GenerateCopiesOfBooks(books, 50);
            await _dbContext.BookCopies.AddRangeAsync(bookCopies);
            await _dbContext.SaveChangesAsync();
        }
    }
}
