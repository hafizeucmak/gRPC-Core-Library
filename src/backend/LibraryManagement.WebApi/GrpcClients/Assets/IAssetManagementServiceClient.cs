namespace LibraryManagement.WebApi.GTaskClients.Assets
{
    public interface IAssetManagementServiceClient
    {
        Task AddBookRecordAsync();

        Task UpdateBookInfoAsync();

        Task DeleteBookRecordAsync();

        Task AddBookCopyAsync();

        Task UpdateBookCopyAsync();

        Task DeleteBookCopyAsync();
    }
}
