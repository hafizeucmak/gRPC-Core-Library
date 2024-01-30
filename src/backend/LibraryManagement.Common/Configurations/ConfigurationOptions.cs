namespace LibraryManagement.Common.Configurations
{
    public class ConfigurationOptions
    {
        public DbConnectionOptions? DbConnectionOptions { get; set; }

        public GrpcClientConnectionOptions? GrpcClientConnectionOptions { get; set; }

        public RabbitMQConnectionOptions? RabbitMQConnectionOptions { get; set;  }
    }
}
