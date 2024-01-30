using System.Collections.Concurrent;

namespace LibraryManagement.Common.RabbitMQEvents
{
    public interface IEventCommandQueue
    {
        void Queue(RegisteredEventCommands registeredCommands);

        bool Dequeue(out RegisteredEventCommands registeredCommands);

        void Signal();

        Task WaitAsync();
    }

    public class EventCommandQueue : IEventCommandQueue
    {
        private readonly ConcurrentQueue<RegisteredEventCommands> _commands = new ConcurrentQueue<RegisteredEventCommands>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Queue(RegisteredEventCommands command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands.Enqueue(command);
        }

        public bool Dequeue(out RegisteredEventCommands command)
        {
            return _commands.TryDequeue(out command);
        }

        public void Signal()
        {
            _signal.Release();
        }

        public async Task WaitAsync()
        {
            await _signal.WaitAsync();
        }
    }
}
