using LibraryManagement.BorrowingGrpcService;
using Mapster;

namespace LibraryManagement.WebApi.Models
{
    public class TopBorrowersWithinTimeframeDTO
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public int BorrowedBookCount { get; set; }
    }

    public class TopBorrowersWithinTimeframeDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<TopBorrowerDetail, TopBorrowersWithinTimeframeDTO>()
                .Map(dest => dest.UserName, src => src.UserName)
                .Map(dest => dest.UserEmail, src => src.UserEmail)
                .Map(dest => dest.BorrowedBookCount, src => src.BorrowedBookCount);
        }
    }
}
