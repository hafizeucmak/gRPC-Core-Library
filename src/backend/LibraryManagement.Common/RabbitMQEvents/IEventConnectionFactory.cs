using RabbitMQ.Client;

namespace LibraryManagement.Common.RabbitMQEvents
{
    public interface IEventConnectionFactory
    {
        IConnection GetConnection();

        IModel GetModel(IConnection connection);
    }
}
