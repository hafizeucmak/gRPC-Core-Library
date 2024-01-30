using LibraryManagement.Common.Configurations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace LibraryManagement.Common.Utils
{
    public class RabbitMQService : IRabbitMQService
    {
        RabbitMQConnectionOptions _rabbitMQConnectionOptions;
        ILogger<RabbitMQService> _logger;
        public RabbitMQService(RabbitMQConnectionOptions rabbitMQConnectionOptions, ILogger<RabbitMQService> logger)
        {
            _rabbitMQConnectionOptions = rabbitMQConnectionOptions;
            _logger = logger;
        }

        public IConnection GetConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMQConnectionOptions.HostName,
                    UserName = _rabbitMQConnectionOptions.UserName,
                    Password = _rabbitMQConnectionOptions.Password,
                };

                factory.AutomaticRecoveryEnabled = true;
                factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
                factory.TopologyRecoveryEnabled = true;

                return factory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError("//TODO:", ex);

                return GetConnection();
            }
        }

        public IModel GetModel(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}
