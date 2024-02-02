using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetTopBorrowersWithinSpecifiedTimeframeQuery : IRequest<TopBorrowersResponse>
    {
        private readonly GetTopBorrowersWithinSpecifiedTimeframeQueryValidator _validator = new();
        public GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeQueryValidator : AbstractValidator<GetTopBorrowersWithinSpecifiedTimeframeQuery>
    {
        public GetTopBorrowersWithinSpecifiedTimeframeQueryValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
        }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeQueryHandler : IRequestHandler<GetTopBorrowersWithinSpecifiedTimeframeQuery, TopBorrowersResponse>
    {
        private readonly IGenericWriteRepository<BorrowingBaseDbContext> _genericWriteRepository;
        private readonly ILogger<GetTopBorrowersWithinSpecifiedTimeframeQuery> _logger;

        public GetTopBorrowersWithinSpecifiedTimeframeQueryHandler(IGenericWriteRepository<BorrowingBaseDbContext> genericWriteRepository,
                                               ILogger<GetTopBorrowersWithinSpecifiedTimeframeQuery> logger)
        {
            _logger = logger;
            _genericWriteRepository = genericWriteRepository;
        }

        public async Task<TopBorrowersResponse> Handle(GetTopBorrowersWithinSpecifiedTimeframeQuery query, CancellationToken cancellationToken)
        {
            var topBorrowersQuery = await Task.Run(() => _genericWriteRepository.GetAll<Borrowing>()
                                                      .Where(x => x.BorrowDate.Date >= query.StartDate.Date
                                                                && x.BorrowDate <= query.EndDate.Date
                                                                && x.BookCopyId  == null)
                                                      .GroupBy(x => new { x.UserId, x.User.FullName, x.User.Email })
                                                      .Select(x => new { 
                                                          UserId = x.Key.UserId, 
                                                          UserName = x.Key.FullName, 
                                                          UserEmail = x.Key.Email, 
                                                          BorrowCount = x.Count() })
                                                      .Where(x => x.BorrowCount > 0)
                                                      .OrderByDescending(x => x.BorrowCount)
                                                      .Select(x => new TopBorrowerDetail
                                                      {
                                                          UserEmail = x.UserEmail,
                                                          UserName = x.UserName,
                                                          BorrowedBookCount = x.BorrowCount
                                                      }));

            return new TopBorrowersResponse() { TopBorrowers = { topBorrowersQuery } };
        }
    }
}
