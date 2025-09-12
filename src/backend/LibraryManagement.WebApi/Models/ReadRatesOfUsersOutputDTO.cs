namespace LibraryManagement.WebApi.Models
{
    public class ReadRatesOfUsersOutputDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public double ReadRate { get; set; }
    }
}
