using LibraryManagement.UserGrpcService.Domains;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.UserGrpcService.Data.DataAccess.DbContexts
{
    public class UserBaseDbContext : DbContext
    {
        public UserBaseDbContext(DbContextOptions<UserBaseDbContext> options) : base(options)
        {
            if (options != null)
            {
                Database.AutoTransactionsEnabled = false;
                ChangeTracker.LazyLoadingEnabled = false;
                ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }
        public virtual DbSet<User> Users { get; set; }

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
