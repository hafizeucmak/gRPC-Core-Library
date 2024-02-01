using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData
{
    public abstract class BaseSeedService : ISeedService
    {
        protected readonly BorrowingBaseDbContext _dbContext;

        protected BaseSeedService(BorrowingBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract Task Execute();
    }
}
