using LibraryManagement.Common.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace LibraryManagement.Common.RabbitMQEvents
{
    public class EventPublisherService :IEventPublisherService
    {
        private readonly IEventConnectionFactory _eventConnectionFactory;
        private readonly ILogger<EventPublisherService> _logger;
        public EventPublisherService(IEventConnectionFactory eventConnectionFactory, ILogger<EventPublisherService> logger)
        {
            _eventConnectionFactory = eventConnectionFactory;
            _logger = logger;
        }

        public void PublishEvent<TEvent>(TEvent @event, string queueName)
        {
            try
            {
                using (var connection = _eventConnectionFactory.GetConnection())
                {
                    using (var channel = _eventConnectionFactory.GetModel(connection))
                    {

                        channel.QueueDeclare(queue: queueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false, null);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.Expiration = RabbitMQConstants.MessageTTL;

                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during publishing events. Exception : {ex.InnerException?.Message}", ex);
                throw new Exception("An error occurred during publishing events.", ex);
            }
        }
    }
}
