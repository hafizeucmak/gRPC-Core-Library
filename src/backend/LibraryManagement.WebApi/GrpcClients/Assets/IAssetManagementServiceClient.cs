using LibraryManagement.AssetsGRPCService;

namespace LibraryManagement.WebApi.GrpcClients.Assets
{
    public interface IAssetManagementServiceClient
    {
        Task AddBookRecordAsync(BookAddRequest bookRequest);

        Task AddBookCopyAsync(BookCopyAddRequest request);

        Task<BookByISBNResponse> GetBookByIsbnAsync(BookByISBNRequest request);
    }
}
