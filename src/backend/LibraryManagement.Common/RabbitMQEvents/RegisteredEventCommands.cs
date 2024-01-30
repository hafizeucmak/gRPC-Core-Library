namespace LibraryManagement.Common.RabbitMQEvents
{
    public class RegisteredEventCommands : List<IQueueEventCommand>
    {
        public virtual void RegisteredEventCommand(IQueueEventCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            Add(command);
        }
    }
}
