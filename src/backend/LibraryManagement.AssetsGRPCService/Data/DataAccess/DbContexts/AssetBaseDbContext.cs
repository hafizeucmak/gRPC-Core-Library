using LibraryManagement.AssetsGRPCService.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LibraryManagement.AssetsGRPCService.Data.DataAccess.DbContexts
{
    public class AssetBaseDbContext : DbContext
    {
        public AssetBaseDbContext(DbContextOptions<AssetBaseDbContext> options) : base(options)
        {
            if (options == null)
            {
                //TODO: exaption handle
                throw new ArgumentNullException(nameof(options));
            }

            Database.AutoTransactionsEnabled = false;
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;

        }
        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<BookCopy> BookCopies { get; set; }

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
