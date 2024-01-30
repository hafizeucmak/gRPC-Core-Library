using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.Extensions;
using LibraryManagement.Common.Filters;
using LibraryManagement.Common.Middlewares;
using LibraryManagement.Common.RabbitMQEvents;
using LibraryManagement.UserGrpcService.DataAccesses.DbContexts;
using LibraryManagement.UserGrpcService.Services;
using System.Reflection;

namespace LibraryManagement.UserGrpcService
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
            services.AddRepositories();
            
            services.AddDbContext<UserBaseDbContext>(configurationOptions);

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddGrpc(c =>
            {
                c.Interceptors.Add<TransactionManagerInterceptor<UserBaseDbContext>>();
                c.Interceptors.Add<GrpcGlobalExceptionHandlerInterceptor>();
            });

          //  services.AddScoped<IRequestHandler<CreateBookCommand>, CreateBookCommandHandler>();

            services.AddRepositories();

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
                endpoints.MapGrpcService<UserService>();

            });
        }

        public static void AddQueueLogEventHandlers(IServiceCollection services)
        {
            services.AddScoped<RegisteredEventCommands>();

         //   services.AddTransient<IRequestHandler<QueueEventCommand<BookCreatedEvent>>, QueueEventCommandHandler<BookCreatedEvent>>();
        }
    }
}
