using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.DataAccesses.DbContexts
{
    public class LibraryWriteDbContext : LibraryBaseContext
    {
        public LibraryWriteDbContext(
          DbContextOptions<LibraryWriteDbContext> options)
          : base(options)
        {
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
