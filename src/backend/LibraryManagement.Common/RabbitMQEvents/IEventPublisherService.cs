namespace LibraryManagement.Common.RabbitMQEvents
{
    public interface IEventPublisherService
    {
        void PublishEvent<TEvent>(TEvent @event, string queueName);
    }
}
