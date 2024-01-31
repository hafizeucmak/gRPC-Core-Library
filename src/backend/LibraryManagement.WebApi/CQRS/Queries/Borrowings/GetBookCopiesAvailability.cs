using FluentValidation;
using LibraryManagement.BorrowingGrpcService;
using LibraryManagement.WebApi.GrpcClients.Borrows;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Borrowings
{
    public class GetBookCopiesAvailability : IRequest<BookCopiesAvailabilityDTO>
    {
        private readonly GetBookCopiesAvailabilityValidator _validator = new();

        public GetBookCopiesAvailability(string isbn)
        {
            Isbn = isbn;

            _validator.ValidateAndThrow(this);
        }

        public string Isbn { get; set; }
    }

    public class GetBookCopiesAvailabilityValidator : AbstractValidator<GetBookCopiesAvailability>
    {
        public GetBookCopiesAvailabilityValidator()
        {
            RuleFor(x => x.Isbn).NotEmpty();
        }
    }

    public class GetBookCopiesAvailabilityHandler : IRequestHandler<GetBookCopiesAvailability, BookCopiesAvailabilityDTO>
    {
        private readonly IBorrowingServiceClient _borrowingServiceClient;

        public GetBookCopiesAvailabilityHandler(IBorrowingServiceClient borrowingServiceClient)
        {
            _borrowingServiceClient = borrowingServiceClient;
        }

        public async Task<BookCopiesAvailabilityDTO> Handle(GetBookCopiesAvailability query, CancellationToken cancellationToken)
        {
            var result = await Task.Run(() => _borrowingServiceClient.GetBookCopiesAvailability(new BookCopiesAvailabilityRequest() { Isbn = query.Isbn }));

            return result.Adapt<BookCopiesAvailabilityDTO>();
        }
    }
}
