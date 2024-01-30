using LibraryManagement.Domain.Entities.Borrowings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Domain.EntityConfigurations
{
    public class BorrowingEntityConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> borrowing)
        {
            borrowing.HasKey(x => x.Id).IsClustered(false);

            borrowing.HasIndex(x => new { x.Id, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);
        }
    }
}
