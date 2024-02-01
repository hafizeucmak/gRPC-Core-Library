namespace LibraryManagement.BorrowingGrpcService.Data.DataAccess.SeedData.Utils
{
    public static class SeedUtils
    {
        public static string FormatSeedServiceName(string fullSeedServiceName)
        {
            return fullSeedServiceName.ToLowerInvariant().Trim().Replace("seedservice", string.Empty);
        }
    }
}
