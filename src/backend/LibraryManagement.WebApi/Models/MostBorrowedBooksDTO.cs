using LibraryManagement.BorrowingGrpcService;
using Mapster;

namespace LibraryManagement.WebApi.Models
{
    public class MostBorrowedBooksDTO
    {
        public string PublisherName { get; set; }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string Isbn { get; set; }

        public int BorrowedCount { get; set; }
    }

    public class MostBorrowedBooksDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<BorrowedBook, MostBorrowedBooksDTO>()
                .Map(dest => dest.PublisherName, src => src.Publisher)
                .Map(dest => dest.AuthorName, src => src.Author)
                .Map(dest => dest.Isbn, src => src.Isbn)
                .Map(dest => dest.BorrowedCount, src => src.BorrowedCount);
        }
    }
}
