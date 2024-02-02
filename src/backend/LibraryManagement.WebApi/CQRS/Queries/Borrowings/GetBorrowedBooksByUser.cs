using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetBorrowedBooksByUser : IRequest<IEnumerable<BorrowedBooksByUserDTO>>
    {
        private readonly GetBorrowedBooksByUserValidator _validator = new();

        public GetBorrowedBooksByUser(DateTime startDate, DateTime endDate, string userEmail)
        {
            StartDate = startDate;
            EndDate = endDate;
            UserEmail = userEmail;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UserEmail { get; set; }
    }

    public class GetBorrowedBooksByUserValidator : AbstractValidator<GetBorrowedBooksByUser>
    {
        public GetBorrowedBooksByUserValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.UserEmail).NotEmpty().NotNull();
        }
    }

    public class GetBorrowedBooksByUserHandler : IRequestHandler<GetBorrowedBooksByUser, IEnumerable<BorrowedBooksByUserDTO>>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetBorrowedBooksByUserHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<IEnumerable<BorrowedBooksByUserDTO>> Handle(GetBorrowedBooksByUser query, CancellationToken cancellationToken)
        {
            var request = new BorrowedBooksByUserRequest()
            {
                UserEmail = query.UserEmail,
                StartDate = query.StartDate.ToUniversalTime().ToTimestamp(),
                EndDate = query.EndDate.ToUniversalTime().ToTimestamp()
            };

            var results = await _borrowingServiceClient.GetBorrowedBooksByUser(request);
            return results.BorrowedBooks.Adapt<IEnumerable<BorrowedBooksByUserDTO>>();
        }
    }
}
