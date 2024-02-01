using FluentValidation;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetAverageReadRateForBook : IRequest<ReadRateForBookOutputDTO>
    {
        private readonly GetAverageReadRateForBookValidator _validator = new();

        public GetAverageReadRateForBook(DateTime startDate, DateTime endDate, string isbn)
        {
            StartDate = startDate;
            EndDate = endDate;
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Isbn { get; set; }
    }

    public class GetAverageReadRateForBookValidator : AbstractValidator<GetAverageReadRateForBook>
    {
        public GetAverageReadRateForBookValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.Isbn).NotEmpty().NotNull();
        }
    }

    public class GetAverageReadRateForBookHandler : IRequestHandler<GetAverageReadRateForBook, ReadRateForBookOutputDTO>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetAverageReadRateForBookHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<ReadRateForBookOutputDTO> Handle(GetAverageReadRateForBook query, CancellationToken cancellationToken)
        {
            var request = new ReadRateRequest { Isbn = query.Isbn };
            var result = await _borrowingServiceClient.GetAverageReadRateForBook(request);

            return result.Adapt<ReadRateForBookOutputDTO>();
        }
    }
}
