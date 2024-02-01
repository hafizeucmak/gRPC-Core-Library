namespace LibraryManagement.BorrowingGrpcService.Data.SeedData
{
    public class SeedServicesContainer : Dictionary<string, Type>
    {
        private readonly Type _interfaceType = typeof(ISeedService);

        public SeedServicesContainer() : base() { }

        public void RegisterSeedService(Type type)
        {
            if (!_interfaceType.IsAssignableFrom(type))
            {
                throw new Exception("Invalid seed service type.");
            }

            Add(type.Name.Replace("SeedService", string.Empty).ToLowerInvariant(), type);
        }
    }
}
