using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Data.SeedData;
using LibraryManagement.BorrowingGrpcService.Data.SeedData.DbHandler;
using LibraryManagement.BorrowingGrpcService.Services;
using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.Extensions;
using LibraryManagement.Common.Extensions.Reflection;
using LibraryManagement.Common.Filters;
using LibraryManagement.Common.Middlewares;
using LibraryManagement.Common.RabbitMQEvents;
using LibraryManagement.Common.SeedManagements.Enums;
using System.Reflection;

namespace LibraryManagement.BorrowingGrpcService
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        private string _seedServiceSuffix = "SeedService";
        private string _seedServicesNamespace = "LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedServices";
        public Startup(ILogger<Startup> logger, IConfiguration configuration, IHostEnvironment env)
        {
            Logger = logger;
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public ILogger<Startup> Logger { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var configurationOptions = Configuration.GetSection("AppSettings").Get<ConfigurationOptions>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddGrpc();

            services.AddDbContext<BorrowingBaseDbContext>(configurationOptions);

            services.AddGrpc(c =>
            {
                c.Interceptors.Add<TransactionManagerInterceptor<BorrowingBaseDbContext>>();
                c.Interceptors.Add<GrpcGlobalExceptionHandlerInterceptor>();
            });

            services.AddRepositories();

            services.AddRabbitMQEventHub(configurationOptions);

            AddQueueLogEventHandlers(services);

            ConfigureSeedServices(services, _seedServicesNamespace);

            services.AddExceptionManager();
            services.AddScoped<RecreateDbHandler>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BorrowingService>();
            });
        }
        public void AddQueueLogEventHandlers(IServiceCollection services)
        {
            services.AddScoped<RegisteredEventCommands>();

            // services.AddTransient<IRequestHandler<QueueEventCommand<BookCreatedEvent>>, QueueEventCommandHandler<BookCreatedEvent>>();
        }

        public void ConfigureSeedServices(IServiceCollection services, string servicesNamespace)
        {
            var seederTypes = ReflectionExtensions.GetTypesFromAssembly(typeof(BaseSeedService), typeof(ISeedService), servicesNamespace);
            CheckSeedServiceTypesEnumValues(services, seederTypes);

            services.AddScoped<ISeedInitializer, SeedInitializer>();
            //TODO: think about it auto migration also
            // services.AddScoped<RecreateDBHandler>();

            AddSeedServiceContainer(services, seederTypes);
            RegisterSeedServices(services, seederTypes);
        }

        private void CheckSeedServiceTypesEnumValues(IServiceCollection serviceCollection, IEnumerable<Type> seederTypes)
        {
            if (seederTypes.Any())
            {
                foreach (var seeder in seederTypes)
                {
                    string seederName = seeder.Name.Trim().Replace(_seedServiceSuffix, string.Empty);
                    if (!Enum.IsDefined(typeof(SeedServiceTypes), seederName))
                    {
                        throw new InvalidOperationException($"{nameof(SeedServiceTypes)} enum is not defined for {seederName}.");
                    }
                }
            }
        }

        private void RegisterSeedServices(IServiceCollection serviceCollection, IEnumerable<Type> seederTypes)
        {
            if (seederTypes.Any())
            {
                foreach (var seedService in seederTypes)
                {
                    serviceCollection.AddScoped(seedService);
                }
            }
        }

        private void AddSeedServiceContainer(IServiceCollection serviceCollection, IEnumerable<Type> seederTypes)
        {
            var seedServicesContainer = new SeedServicesContainer();

            foreach (var type in seederTypes)
            {
                seedServicesContainer.RegisterSeedService(type);
            }

            serviceCollection.AddSingleton(seedServicesContainer);
        }
    }
}
