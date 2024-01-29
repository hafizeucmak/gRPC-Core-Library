using LibraryManagement.AssetsGRPCService;
using LibraryManagement.WebApi.GTaskClients.Assets;

namespace LibraryManagement.WebApi.GrpcClients.Assets
{
    public class AssetManagementServiceClient : IAssetManagementServiceClient
    {
        private readonly AssetManagementGRPCService.AssetManagementGRPCServiceClient _assetManagementGRPCServiceClient;

        public AssetManagementServiceClient(AssetManagementGRPCService.AssetManagementGRPCServiceClient assetManagementServiceClient)
        {
            _assetManagementGRPCServiceClient = assetManagementServiceClient;
        }

        public async Task AddBookCopyAsync()
        {
           var result = await _assetManagementGRPCServiceClient.AddBookRecordAsync(new BookAddRequest());
        }

        public async Task AddBookRecordAsync()
        {
            var req = new BookAddRequest { Name = "hafili" };
            var result = await _assetManagementGRPCServiceClient.AddBookRecordAsync(req);
        }

        public Task DeleteBookCopyAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteBookRecordAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookCopyAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookInfoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
