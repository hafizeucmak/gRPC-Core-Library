namespace LibraryManagement.WebApi.Models
{
    public class BorrowBookOutputDTO
    {
        public DateTime ExpectedReturnDate { get; set; }

        public string BookTitle { get; set; }

        public string UserFullname { get; set; }
    }
}
