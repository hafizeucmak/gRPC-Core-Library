namespace LibraryManagement.Common.Configurations
{
    public class GrpcClientConnectionOptions
    {
        public string? UserGRPCServiceClientUrl { get; set; }

        public string? AssetManagementGRPCServiceClientUrl { get; set; }

        public string? BorrowGRPCServiceClientUrl { get; set; }
    }
}
