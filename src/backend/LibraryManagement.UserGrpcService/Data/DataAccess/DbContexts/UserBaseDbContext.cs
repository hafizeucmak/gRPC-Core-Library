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
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
