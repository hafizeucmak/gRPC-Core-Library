using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Copies;
using LibraryManagement.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.DataAccesses.DbContexts
{
    public abstract class LibraryBaseContext : DbContext
    {
        public LibraryBaseContext(DbContextOptions<LibraryWriteDbContext> options) : base(options) {
            Database.AutoTransactionsEnabled = false;
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<BookCopy> BookCopies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
