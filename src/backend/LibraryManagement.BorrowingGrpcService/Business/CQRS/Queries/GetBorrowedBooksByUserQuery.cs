using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using LibraryManagement.BorrowingGrpcService.DataAccesses.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetBorrowedBooksByUserQuery : IRequest<BorrowedBooksByUserResponse>
    {
        private readonly GetBorrowedBooksByUserQueryValidator _validator = new();
        public GetBorrowedBooksByUserQuery(DateTime startDate, DateTime endDate, string userEmail)
        {
            UserEmail = userEmail;
            StartDate = startDate;
            EndDate = endDate;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string UserEmail { get; set; }
    }

    public class GetBorrowedBooksByUserQueryValidator : AbstractValidator<GetBorrowedBooksByUserQuery>
    {
        public GetBorrowedBooksByUserQueryValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.UserEmail).NotEmpty();
        }
    }

    public class GetBorrowedBooksByUserQueryHandler : IRequestHandler<GetBorrowedBooksByUserQuery, BorrowedBooksByUserResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetBorrowedBooksByUserQuery> _logger;

        public GetBorrowedBooksByUserQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                               ILogger<GetBorrowedBooksByUserQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<BorrowedBooksByUserResponse> Handle(GetBorrowedBooksByUserQuery query, CancellationToken cancellationToken)
        {
            var user = await _genericWriteRepository.GetAll<User>().FirstOrDefaultAsync(x => x.Email.Equals(query.UserEmail), cancellationToken);

            if (user == null)
            {
                throw new ResourceNotFoundException($"{nameof(user)} not found with {nameof(query.UserEmail)} is equal to {query.UserEmail}");
            }
            var borrowings = await _genericWriteRepository.GetAll<Borrowing>()
                                                     .Where(x => x.BorrowDate.Date >= query.StartDate.Date
                                                              && x.BorrowDate <= query.EndDate.Date
                                                              && x.UserId.Equals(user.Id))
                                                     .Select(x => new BorrowedBookDetail()
                                                     {
                                                         Title = x.Book.Title,
                                                         Author = x.Book.Author,
                                                         Publisher = x.Book.Publisher,
                                                         PageCount = x.Book.PageCount,
                                                         BorrowedDate = x.BorrowDate.ToTimestamp(),
                                                     }).ToListAsync(cancellationToken);
            //TODO: mapster
            return new BorrowedBooksByUserResponse
            {
                BorrowedBooks = { borrowings }
            };
        }
    }
}