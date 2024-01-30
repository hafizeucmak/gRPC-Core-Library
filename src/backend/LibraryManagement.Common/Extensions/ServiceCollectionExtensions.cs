using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.Extensions;
using LibraryManagement.Common.GenericRepositories;
using LibraryManagement.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Registration of Repositories
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));
        }

        #endregion

        #region Registration of DbContexts

        public static void AddDbContext<TDbContext>(this IServiceCollection services, ConfigurationOptions configurationOptions)
         where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(options =>
            {
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(StringBuilderUtils.BuildConnectionString(configurationOptions), sqlOptions =>
                {
                    sqlOptions.CommandTimeout(120);
                });
            });
        }

        #endregion
    }
}
