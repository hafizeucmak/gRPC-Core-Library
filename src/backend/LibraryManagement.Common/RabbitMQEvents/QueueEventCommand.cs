using MediatR;

namespace LibraryManagement.Common.RabbitMQEvents
{
    public interface IQueueEventCommand : IRequest { }

    public class QueueEventCommand<TEvent> : IQueueEventCommand where TEvent : Event
    {
        public QueueEventCommand(string queueName)
        {
            QueueName = queueName;
        }

        public string QueueName { get; set; }
    }

    public class QueueEventCommandHandler<TEvent> : IRequestHandler<QueueEventCommand<TEvent>>
        where TEvent : Event
    {
        private IEventPublisherService _eventPublisher;

        public QueueEventCommandHandler(IEventPublisherService eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(QueueEventCommand<TEvent> command, CancellationToken cancellationToken)
        {
           // var eventInstance = Activator.CreateInstance(typeof(TEvent));

            var a = new { Name = "", elma = "" };

            await Task.Run(() =>
            {
                _eventPublisher.PublishEvent(a, command.QueueName);
            });
        }
    }
}
