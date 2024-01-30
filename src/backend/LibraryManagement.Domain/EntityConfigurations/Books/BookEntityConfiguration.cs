using LibraryManagement.Common.Constants;
using LibraryManagement.Domain.Entities.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Domain.EntityConfigurations.Books
{
    public class BookEntityConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> book)
        {
            book.HasKey(x => x.Id).IsClustered(false);

            book.HasIndex(x => new { x.ISBN, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            book.HasIndex(x => new { x.Title, x.ISBN, x.DeletedAt })
               .IsUnique()
               .IsClustered(false);

            book.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_TITLE);

            book.Property(x => x.Author)
              .IsRequired()
              .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_AUTHOR);

            book.Property(x => x.Publisher)
              .IsRequired()
              .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_PUBLISHER);

            book.HasMany(x => x.BookCopies)
             .WithOne(w => w.Book)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();
        }
    }
}
