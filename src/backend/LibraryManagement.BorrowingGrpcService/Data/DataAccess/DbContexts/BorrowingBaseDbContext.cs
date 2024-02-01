using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.Base;
using LibraryManagement.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts
{
    public class BorrowingBaseDbContext : DbContext
    {
        public BorrowingBaseDbContext(DbContextOptions<BorrowingBaseDbContext> options) : base(options)
        {
            if (options != null)
            {
                Database.AutoTransactionsEnabled = false;
                ChangeTracker.LazyLoadingEnabled = false;
                ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }

        public virtual DbSet<Borrowing> Borrowings { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<BookCopy> BookCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            var entityTypes = GetEntityTypes(modelBuilder).ToList();
            AddDefaultMaxLength(modelBuilder, entityTypes);
            AddDeletedAtQueryFilter(modelBuilder, entityTypes.Where(x => x.BaseType == null));
            AddDeletedAtIsNullFilterToNonClusteredIndexes(modelBuilder, entityTypes);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        private IEnumerable<IMutableEntityType> GetEntityTypes(ModelBuilder builder)
        {
            return builder.Model.GetEntityTypes().Where(x => typeof(IDomainEntity).IsAssignableFrom(x.ClrType));
        }

        private void AddDefaultMaxLength(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var entityTypeBuilder = builder.Entity(entityClrType);
                var properties = entityClrType.GetProperties();

                var stringProperties = properties.Where(x => x.PropertyType == typeof(string));

                foreach (var stringPropertyInfo in stringProperties)
                {
                    entityTypeBuilder
                        .Property(stringPropertyInfo.PropertyType, stringPropertyInfo.Name)
                        .HasMaxLength(DbContextConstants.DEFAULT_MAX_LENGTH_FOR_STRING);
                }

                var enumProperties = properties.Where(x => x.PropertyType.IsEnum);

                foreach (var enumPropertyInfo in enumProperties)
                {
                    entityTypeBuilder
                        .Property(enumPropertyInfo.PropertyType, enumPropertyInfo.Name)
                        .HasMaxLength(DbContextConstants.DEFAULT_MAX_LENGTH_FOR_STRING);
                }
            }
        }

        private void AddDeletedAtQueryFilter(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var parameter = Expression.Parameter(entityClrType, "p");
                var deletedAtProperty = entityClrType.GetProperty(nameof(DomainEntity.DeletedAt));
                var deletedAtNullExpression = Expression.Equal(Expression.Property(parameter, deletedAtProperty), Expression.Constant(null, typeof(DateTime?)));
                var filter = Expression.Lambda(deletedAtNullExpression, parameter);
                var entityTypeBuilder = builder.Entity(entityClrType);
                entityTypeBuilder.HasQueryFilter(filter);
            }
        }

        private void AddDeletedAtIsNullFilterToNonClusteredIndexes(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            string deletedAtIndexFilter = $"[{nameof(DomainEntity.DeletedAt)}] IS NULL";
            string deletedAtNotNullIndexFilter = $"[{nameof(DomainEntity.DeletedAt)}] IS NOT NULL";

            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var entityTypeBuilder = builder.Entity(entityClrType);

                var nonClusteredIndexesWithDeletedAt = entityTypeBuilder.Metadata.GetIndexes().Where(i =>
                    (
                        !i.IsClustered().HasValue ||
                            (i.IsClustered().HasValue && !i.IsClustered().Value)
                    )
                    && i.Properties.Any(p => p.Name == nameof(DomainEntity.DeletedAt)));

                foreach (var index in nonClusteredIndexesWithDeletedAt)
                {
                    var indexFilter = index.GetFilter();
                    if (!string.IsNullOrEmpty(indexFilter) && indexFilter.Contains(deletedAtNotNullIndexFilter))
                    {
                        indexFilter = indexFilter.Replace(deletedAtNotNullIndexFilter, deletedAtIndexFilter);
                    }
                    else
                    {
                        indexFilter = deletedAtIndexFilter;
                    }

                    index.SetFilter(indexFilter);
                }
            }
        }
    }
}
