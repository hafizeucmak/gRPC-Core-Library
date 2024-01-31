using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetTopBorrowersWithinSpecifiedTimeframe : IRequest<IEnumerable<TopBorrowersWithinTimeframeDTO>>
    {
        private readonly GetTopBorrowersWithinSpecifiedTimeframeValidator _validator = new();

        public GetTopBorrowersWithinSpecifiedTimeframe(DateTime startDate, DateTime endDate, int expectedTopBorrowerCount)
        {
            StartDate = startDate;
            EndDate = endDate;
            ExpectedTopBorrowerCount = expectedTopBorrowerCount;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ExpectedTopBorrowerCount { get; set; }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeValidator : AbstractValidator<GetTopBorrowersWithinSpecifiedTimeframe>
    {
        public GetTopBorrowersWithinSpecifiedTimeframeValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.ExpectedTopBorrowerCount).GreaterThan(0);
        }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeHandler : IRequestHandler<GetTopBorrowersWithinSpecifiedTimeframe, IEnumerable<TopBorrowersWithinTimeframeDTO>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetTopBorrowersWithinSpecifiedTimeframeHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<IEnumerable<TopBorrowersWithinTimeframeDTO>> Handle(GetTopBorrowersWithinSpecifiedTimeframe query, CancellationToken cancellationToken)
        {
            var request = new TopBorrowersRequest()
            {
                StartDate = query.StartDate.ToTimestamp(),
                EndDate = query.EndDate.ToTimestamp(),
                ExpectedTopBorrowerCount = query.ExpectedTopBorrowerCount
            };

            var results = await _borrowingServiceClient.GetTopBorrowersWithinSpecifiedTimeframe(request);

            //TODO: map
            return results.TopBorrowers.Select(x => new TopBorrowersWithinTimeframeDTO
            {
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                BorrowedBookCount = x.BorrowedBookCount,
            });
        }
    }
}
