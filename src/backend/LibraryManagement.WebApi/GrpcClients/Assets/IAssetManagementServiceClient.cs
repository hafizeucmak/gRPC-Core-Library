using LibraryManagement.AssetsGRPCService;

namespace LibraryManagement.WebApi.GrpcClients.Assets
{
    public interface IAssetManagementServiceClient
    {
        Task AddBookRecordAsync(BookAddRequest bookRequest);

        Task UpdateBookInfoAsync();

        Task DeleteBookRecordAsync();

        Task AddBookCopyAsync();

        Task UpdateBookCopyAsync();

        Task DeleteBookCopyAsync();

        Task<BookByISBNResponse> GetBookByIsbnAsync(BookByISBNRequest request);

    }
}
