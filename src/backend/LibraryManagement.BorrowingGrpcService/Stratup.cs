using LibraryManagement.BorrowingGrpcService.DataAccesses.DbContexts;
using LibraryManagement.BorrowingGrpcService.Services;
using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.Extensions;
using LibraryManagement.Common.Filters;
using LibraryManagement.Common.Middlewares;
using LibraryManagement.Common.RabbitMQEvents;
using MediatR;

namespace LibraryManagement.BorrowingGrpcService
{
    public class Startup
    {
        private readonly IHostEnvironment _env;

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

            services.Con

            services.AddRabbitMQEventHub(configurationOptions);

            AddQueueLogEventHandlers(services);
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
        public static void AddQueueLogEventHandlers(IServiceCollection services)
        {
            services.AddScoped<RegisteredEventCommands>();

           // services.AddTransient<IRequestHandler<QueueEventCommand<BookCreatedEvent>>, QueueEventCommandHandler<BookCreatedEvent>>();
        }
    }
}
