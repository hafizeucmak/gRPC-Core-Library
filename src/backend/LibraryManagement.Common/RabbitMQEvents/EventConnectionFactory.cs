using LibraryManagement.Common.Configurations;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Runtime.InteropServices;

namespace LibraryManagement.Common.RabbitMQEvents
{
    public class EventConnectionFactory : IEventConnectionFactory
    {
        ILogger<EventConnectionFactory> _logger;
        public EventConnectionFactory(RabbitMQConnectionOptions rabbitMQConnectionOptions, [Optional] ILogger<EventConnectionFactory> logger)
        {
            RabbitMQConnectionOptions = rabbitMQConnectionOptions;
            _logger = logger;
        }

        public RabbitMQConnectionOptions RabbitMQConnectionOptions { get; set; }

        public IConnection GetConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = RabbitMQConnectionOptions.HostName,
                    UserName = RabbitMQConnectionOptions.UserName,
                    Port = RabbitMQConnectionOptions.Port,
                    Password = RabbitMQConnectionOptions.Password,
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
