namespace LibraryManagement.WebApi.Models
{
    public class BookDetailsDTO
    {
        public string? Title { get; set; }

        public string? AuthorName { get; set; }

        public string? Publisher { get; set; }

        public int? PublicationYear { get; set; }

        public string? Isbn { get; set; }
    }
}
