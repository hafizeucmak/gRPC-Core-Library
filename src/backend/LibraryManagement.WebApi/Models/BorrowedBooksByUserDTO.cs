namespace LibraryManagement.WebApi.Models
{
    public class BorrowedBooksByUserDTO
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Isbn { get; set;  }

        public int PageCount { get; set; }

        public DateTime BorrowedDate { get; set; }
    }
}
