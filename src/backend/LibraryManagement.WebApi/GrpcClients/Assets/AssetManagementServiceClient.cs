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

        public async Task DeleteBookRecordAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBookInfoAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddBookCopyAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBookCopyAsync()
        {
            throw new NotImplementedException();
        }

     
        public async Task UpdateBookCopyAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BookByISBNResponse> GetBookByIsbnAsync(BookByISBNRequest request)
        {
            return await _assetManagementGRPCServiceClient.GetBookByISBNAsync(request);
        }
    }
}
