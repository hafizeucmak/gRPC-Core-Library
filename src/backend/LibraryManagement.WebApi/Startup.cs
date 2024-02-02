using LibraryManagement.Common.Configurations.Swagger;
using LibraryManagement.WebApi.Extensions;
using LibraryManagement.WebApi.Middlewares;
using LibraryManagement.WebApi.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.Json.Serialization;

namespace LibraryManagement.WebApi
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        public readonly string _apiTitle = "LibraryManagementAPI";
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
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(ConfigureSwaggerGenerator);


            services.RegisterGrpcClient(Configuration);

            services.AddExceptionManager();

            services.RegisterGRPCServiceClients();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddMapster();
        }

        void ConfigureSwaggerGenerator(SwaggerGenOptions options)
        {
            options.SupportNonNullableReferenceTypes();
            options.OperationFilter<ResolveDynamicQueryEndpoints>("dqb");
            options.CustomSchemaIds(modelType => new SwashbuckleSchemaHelper().GetSchemaId(modelType));
            options.SchemaFilter<RequireValueTypePropertiesSchemaFilter>(true);
        }

        private void ConfigureSwaggerUI(SwaggerUIOptions options)
        {
            options.DocExpansion(DocExpansion.None);
            options.DisplayRequestDuration();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", _apiTitle);
            options.InjectJavascript("https://code.jquery.com/jquery-3.6.0.min.js");
            options.InjectJavascript("../js/swagger-seed-dropdown-sorting.js");
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(ConfigureSwaggerUI);
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}