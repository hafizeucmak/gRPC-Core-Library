namespace LibraryManagement.BorrowingGrpcService.Data.SeedData
{
    public interface ISeedService
    {
        string Name { get; }

        string Description { get; }

        Task Execute();
    }
}
