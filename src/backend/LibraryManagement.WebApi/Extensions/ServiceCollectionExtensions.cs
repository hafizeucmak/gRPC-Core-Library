using Grpc.Core;
using Grpc.Net.Client.Configuration;
using LibraryManagement.AssetsGRPCService;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.Common.Configurations;
using LibraryManagement.Common.Utils;
using LibraryManagement.UserGrpcService;
using LibraryManagement.WebApi.GrpcClients.Assets;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.GrpcClients.Users;
using LibraryManagement.WebApi.Models;
using Mapster;
using System.Reflection;

namespace LibraryManagement.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string AppSettings = "AppSettings";

        public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(nameof(AppSettings));
            services.Configure<ConfigurationOptions>(appSettings);
        }

        public static void RegisterGrpcClient(this IServiceCollection services, IConfiguration configuration)
        {
            var retryMethodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            };

            var grpcClientOptions = configuration.GetSection($"{nameof(AppSettings)}:{nameof(GrpcClientConnectionOptions)}").Get<GrpcClientConnectionOptions>();

            string? borrowGrpcServiceClientUrl = grpcClientOptions?.BorrowGRPCServiceClientUrl;
            string? assetManagementGRPCServiceClientUrl = grpcClientOptions?.AssetManagementGRPCServiceClientUrl;
            string? userGRPCServiceClientUrl = grpcClientOptions?.UserGRPCServiceClientUrl;

            if (string.IsNullOrEmpty(borrowGrpcServiceClientUrl))
            {
                throw new ArgumentException("//TODO: handle.");
            }

            ConfigureGrpcClient<BorrowGRPCService.BorrowGRPCServiceClient>(services, borrowGrpcServiceClientUrl, retryMethodConfig);


            if (string.IsNullOrEmpty(assetManagementGRPCServiceClientUrl))
            {
                throw new ArgumentException("//TODO: handle.");
            }

            ConfigureGrpcClient<AssetManagementGRPCService.AssetManagementGRPCServiceClient>(services, assetManagementGRPCServiceClientUrl, retryMethodConfig);

            if (string.IsNullOrEmpty(userGRPCServiceClientUrl))
            {
                throw new ArgumentException("//TODO: handle.");
            }

            ConfigureGrpcClient<UserGRPCService.UserGRPCServiceClient>(services, userGRPCServiceClientUrl, retryMethodConfig);
        }

        public static void AddExceptionManager(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionManager, ExceptionManager>();
        }

        public static void RegisterGRPCServiceClients(this IServiceCollection services)
        {
            services.AddScoped<IBorrowingServiceClient, BorrowingServiceClient>();
            services.AddScoped<IUserServiceClient, UserServiceClient>();
            services.AddScoped<IAssetManagementServiceClient, AssetManagementServiceClient>();
        }
        public static void AddMapster(this IServiceCollection services)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            Assembly applicationAssembly = typeof(MostBorrowedBooksDTO).Assembly;
            Assembly applicationAssembly2 = typeof(BorrowedBook).Assembly;
            typeAdapterConfig.Scan(applicationAssembly);
            typeAdapterConfig.Scan(applicationAssembly2);
        }

        private static void ConfigureGrpcClient<TClient>(IServiceCollection services, string clientUrl, MethodConfig retryMethodConfig)
        where TClient : ClientBase<TClient>
        {
            services.AddGrpcClient<TClient>(clientOptions =>
            {
                clientOptions.Address = new Uri(clientUrl);
                clientOptions.ChannelOptionsActions.Add(opt => opt.ServiceConfig = new ServiceConfig { MethodConfigs = { retryMethodConfig } });
            });
        }
    }
}
