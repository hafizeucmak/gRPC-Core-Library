﻿using LibraryManagement.BorrowingGrpcService.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.BorrowingGrpcService.Data.DataAccess.EntityConfigurations
{
    public class BorrowingEntityConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> borrowing)
        {
            borrowing.HasKey(x => x.Id).IsClustered(false);

            borrowing.HasIndex(x => new { x.Id, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            borrowing.HasQueryFilter(w => w.DeletedAt == null);

            borrowing.HasIndex(x => new { x.BookId, x.DeletedAt })
                .IsClustered(false);

            borrowing.HasIndex(x => new { x.BookCopyId, x.DeletedAt })
                .IsClustered(false);

            borrowing.HasOne(x => x.Book)
                  .WithMany()
                  .HasForeignKey(x => x.BookId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);

            borrowing.HasOne(x => x.User)
                  .WithMany()
                  .HasForeignKey(x => x.UserId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Restrict);

            borrowing.HasOne(x => x.BookCopy)
              .WithMany()
              .HasForeignKey(x => x.BookCopyId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
