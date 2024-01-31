using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.BorrowingGrpcService.Domains.Enums;
using LibraryManagement.Common.Constants;
using LibraryManagement.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.BorrowingGrpcService.DataAccesses.EntityConfigurations
{
    public class BookCopyEntityConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> bookCopy)
        {
            bookCopy.HasKey(w => w.Id)
                    .IsClustered(false);

            bookCopy.HasIndex(w => new { w.BookId, w.CopyNumber, w.DeletedAt })
                    .IsUnique()
                    .IsClustered(false);

            bookCopy.HasIndex(w => w.BookId)
                    .IsClustered(false);

            bookCopy.HasOne(x => x.Book)
                    .WithMany()
                    .HasForeignKey(x => x.BookId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

            bookCopy.Property(x => x.CopyNumber)
                    .IsRequired()
                    .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_COPY_NUMBER);

            bookCopy.Property(c => c.Status)
                    .HasConversion(
                        EnumConverter<AssetStatus>.EnumToString,
                        EnumConverter<AssetStatus>.StringToEnum)
                    .IsRequired();
        }
    }
}
