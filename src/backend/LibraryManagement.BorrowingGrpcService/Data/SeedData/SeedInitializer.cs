using LibraryManagement.BorrowingGrpcService.Data.DataAccess.SeedData.Utils;
using LibraryManagement.Common.SeedManagements.Enums;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData
{
    public sealed class SeedInitializer : ISeedInitializer
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Seed(SeedServiceTypes seedService)
        {
            string seederName = SeedUtils.FormatSeedServiceName(seedService.ToString());
            if (string.IsNullOrWhiteSpace(seederName))
            {
                throw new ArgumentNullException(nameof(seedService));
            }

            var services = _serviceProvider.GetService(typeof(SeedServicesContainer)) as Dictionary<string, Type>;
            if (services == null || !services.ContainsKey(seederName))
            {
                throw new InvalidOperationException($"Invalid seed service: {seederName}");
            }

            var seederType = services[seederName];

            var seederService = _serviceProvider.GetService(seederType) as ISeedService;
            if (seederService != null)
            {
                await seederService.Execute();
            }
        }
    }
}