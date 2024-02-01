﻿using LibraryManagement.WebApi.Extensions;
using LibraryManagement.WebApi.Middlewares;
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
            services.AddSwaggerGen();

            services.RegisterGrpcClient(Configuration);

            services.AddExceptionManager();

            services.RegisterGRPCServiceClients();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", _apiTitle);
                });

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