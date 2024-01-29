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
            //config.ForType<IQueryable<Carrier>, ListCarriersOutputDTO>()
            //    .Map(dest => dest.Carriers, src => src);
        }
    }
}
