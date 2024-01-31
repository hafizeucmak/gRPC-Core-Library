using Grpc.Core;
using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.Extensions;
using LibraryManagement.Common.GenericRepositories;
using LibraryManagement.Common.RabbitMQEvents;
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

        public static void AddRabbitMQEventHub(this IServiceCollection services, ConfigurationOptions configurationOptions)
        {
            services.AddSingleton<RabbitMQConnectionOptions>(provider =>
            {
                return configurationOptions.RabbitMQConnectionOptions ?? throw new ArgumentNullException("RabbitMQ configurations not found.");
            });


            services.AddSingleton<IEventConnectionFactory>(config =>
            {
                return new EventConnectionFactory(configurationOptions.RabbitMQConnectionOptions ?? throw new ArgumentNullException("RabbitMQ configuration not found."));
            });

            services.AddSingleton<IEventCommandQueue, EventCommandQueue>();
            services.AddHostedService<EventHostedService>();
            services.AddScoped<IEventPublisherService, EventPublisherService>();
        }

        public static void ConfigureExceptionManager(this IServiceCollection services)
        {
            var exceptionManagerConfig = GetExceptionManagerConfig();
            services.AddSingleton<IExceptionManager>(manager => new GrpcExceptionManager(exceptionManagerConfig));
        }

        public static Dictionary<Type, StatusCode> GetExceptionManagerConfig()
        {
            Dictionary<Type, StatusCode> exceptionManagerConfig = new Dictionary<Type, StatusCode>();
            exceptionManagerConfig.Add(typeof(ResourceNotFoundException), StatusCode.NotFound);

            return exceptionManagerConfig;
        }

        #endregion
    }
}
