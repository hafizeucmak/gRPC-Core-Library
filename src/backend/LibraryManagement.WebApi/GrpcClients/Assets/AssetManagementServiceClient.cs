using LibraryManagement.AssetsGRPCService;

namespace LibraryManagement.WebApi.GrpcClients.Assets
{
    public class AssetManagementServiceClient : IAssetManagementServiceClient
    {
        private readonly AssetManagementGRPCService.AssetManagementGRPCServiceClient _assetManagementGRPCServiceClient;

        public AssetManagementServiceClient(AssetManagementGRPCService.AssetManagementGRPCServiceClient assetManagementServiceClient)
        {
            _assetManagementGRPCServiceClient = assetManagementServiceClient;
        }

        public async Task AddBookRecordAsync(BookAddRequest bookRequest)
        {
            await _assetManagementGRPCServiceClient.AddBookRecordAsync(bookRequest);
        }

        public Task DeleteBookRecordAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateBookInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task AddBookCopyAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteBookCopyAsync()
        {
            throw new NotImplementedException();
        }

     
        public Task UpdateBookCopyAsync()
        {
            throw new NotImplementedException();
        }
    }
}
