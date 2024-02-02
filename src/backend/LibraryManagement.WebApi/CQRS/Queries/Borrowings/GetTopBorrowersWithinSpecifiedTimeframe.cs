using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetTopBorrowersWithinSpecifiedTimeframe : IRequest<IEnumerable<TopBorrowersWithinTimeframeDTO>>
    {
        private readonly GetTopBorrowersWithinSpecifiedTimeframeValidator _validator = new();

        public GetTopBorrowersWithinSpecifiedTimeframe(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            
            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeValidator : AbstractValidator<GetTopBorrowersWithinSpecifiedTimeframe>
    {
        public GetTopBorrowersWithinSpecifiedTimeframeValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
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
                StartDate = query.StartDate.ToUniversalTime().ToTimestamp(),
                EndDate = query.EndDate.ToUniversalTime().ToTimestamp(),
            };

            var results = await _borrowingServiceClient.GetTopBorrowersWithinSpecifiedTimeframe(request);

            return results.TopBorrowers.Adapt<IEnumerable<TopBorrowersWithinTimeframeDTO>>();
        }
    }
}
