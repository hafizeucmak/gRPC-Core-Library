using LibraryManagement.Common.Constants;
using LibraryManagement.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Domain.EntityConfigurations.Users
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> user)
        {
            user.HasKey(x => x.Id).IsClustered(false);

            user.HasIndex(x => new { x.Email, x.Id, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            user.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);

            user.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);

            user.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);

            user.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_FULLNAMES);

            user.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_EMAILS);
        }
    }
}
