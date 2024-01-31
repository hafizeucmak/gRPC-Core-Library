using LibraryManagement.BorrowingGrpcService;
using Mapster;

namespace LibraryManagement.WebApi.Models
{
    public class MostBorrowedBooksDTO
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string Isbn { get; set; }

        public int PageCount { get; set; }
    }

    public class MostBorrowedBooksDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.ForType<BorrowedBook, MostBorrowedBooksDTO>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.AuthorName, src => src.Author)
                .Map(dest => dest.Isbn, src => src.Isbn)
                .Map(dest => dest.PageCount, src => src.Page);
        }
    }
}
