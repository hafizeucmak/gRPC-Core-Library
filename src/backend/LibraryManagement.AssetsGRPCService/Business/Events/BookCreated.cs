using LibraryManagement.Common.RabbitMQEvents;

namespace LibraryManagement.AssetsGRPCService.Business.Events
{
    public class BookCreatedEvent : Event
    {
        public BookCreatedEvent(string title, string author)
        {
            Title = title;
            Author = author;
        }

        public string Title { get; set; }

        public string Author { get; set; }
    }
}
