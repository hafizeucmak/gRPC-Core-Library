using LibraryManagement.BorrowingGrpcService;
using Mapster;

namespace LibraryManagement.WebApi.Models
{
    public class BorrowedBooksByUserDTO
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Isbn { get; set;  }

        public DateTime BorrowedDate { get; set; }
    }

    public class BorrowedBooksByUserDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<BorrowedBookDetail, BorrowedBooksByUserDTO>()
                .Map(dest => dest.Isbn, src => src.Isbn)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Author, src => src.Author)
                .Map(dest => dest.Publisher, src => src.Publisher)
                .Map(dest => dest.BorrowedDate, src => src.BorrowedDate.ToDateTime());
        }
    }
}
