namespace LibraryManagement.WebApi.Models
{
    public class TopBorrowersWithinTimeframeDTO
    {
        public string UserName  { get; set; }

        public string UserEmail  { get; set; }

        public int BorrowedBookCount { get; set; }
    }
}
