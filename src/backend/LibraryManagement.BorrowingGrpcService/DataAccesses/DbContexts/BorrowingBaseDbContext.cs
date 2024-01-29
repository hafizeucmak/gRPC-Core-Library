using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Borrowings;
using LibraryManagement.Domain.Copies;
using LibraryManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.DataAccesses.DbContexts
{
    public class BorrowingBaseDbContext : DbContext
    {
        public BorrowingBaseDbContext(DbContextOptions<BorrowingBaseDbContext> options) : base(options)
        {
            if (options != null)
            {
                Database.AutoTransactionsEnabled = false;
                ChangeTracker.LazyLoadingEnabled = false;
                ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }

        public virtual DbSet<Borrowing> Borrowings { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Book>  Books { get; set; }

        public virtual DbSet<BookCopy>  BookCopy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
