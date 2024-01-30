using RabbitMQ.Client;

namespace LibraryManagement.Common.Utils
{
    public interface IRabbitMQService
    {
        IConnection GetConnection();

        IModel GetModel(IConnection connection);
    }
}
