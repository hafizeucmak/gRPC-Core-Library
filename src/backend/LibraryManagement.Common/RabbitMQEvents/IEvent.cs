namespace LibraryManagement.Common.RabbitMQEvents
{
    public abstract class Event
    {
        public Event()
        {
            EventTimeStamp = DateTime.UtcNow;
        }

        public DateTime EventTimeStamp { get; set; }
    }
}
