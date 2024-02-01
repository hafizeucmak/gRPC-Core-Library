using LibraryManagement.Common.SeedManagements.Enums;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData
{
    public interface ISeedInitializer
    {
        Task Seed(SeedServiceTypes seedService);
    }
}