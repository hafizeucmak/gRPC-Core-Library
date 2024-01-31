using FluentValidation;
using LibraryManagement.BorrowingGrpcService.DataAccesses.DbContexts;
using LibraryManagement.BorrowingGrpcService.Domains;
using LibraryManagement.Common.GenericRepositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Business.CQRS.Queries
{
    public class GetTopBorrowersWithinSpecifiedTimeframeQuery : IRequest<TopBorrowersResponse>
    {
        private readonly GetTopBorrowersWithinSpecifiedTimeframeQueryValidator _validator = new();
        public GetTopBorrowersWithinSpecifiedTimeframeQuery(DateTime startDate, DateTime endDate, int expectedTopBorrowerCount)
        {
            ExpectedTopBorrowerCount = expectedTopBorrowerCount;
            StartDate = startDate;
            EndDate = endDate;

            _validator.ValidateAndThrow(this);
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ExpectedTopBorrowerCount { get; set; }
    }

    public class GetTopBorrowersWithinSpecifiedTimeframeQueryValidator : AbstractValidator<GetTopBorrowersWithinSpecifiedTimeframeQuery>
    {
        public GetTopBorrowersWithinSpecifiedTimeframeQueryValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.ExpectedTopBorrowerCount).GreaterThan(0);
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
            //Belirli bir zaman aralığında en çok kitap ödünç alan kullanıcılar kimlerdir ?
            //TODO: mape bir bak

            var topBorrowerIds = await _genericWriteRepository.GetAll<Borrowing>()
                                                      .Where(x => x.BorrowDate.Date >= query.StartDate.Date && x.BorrowDate <= query.EndDate.Date)
                                                      .GroupBy(x => x.UserId)
                                                      .Select(x => new { UserId = x.Key, BorrowCount = x.Count() })
                                                      .OrderByDescending(x => x.BorrowCount)
                                                      .Select(x => x.UserId)
                                                      .Take(query.ExpectedTopBorrowerCount)
                                                      .ToListAsync(cancellationToken); //TODO check tolistsiz

            var topBorrowers = _genericWriteRepository.GetAll<User>()
                                               .Where(x => topBorrowerIds.Contains(x.Id))
                                               .Select(x => new TopBorrowerDetail
                                               {
                                                   UserEmail = x.Email,
                                                   UserName = x.FullName
                                               });

            return topBorrowers.Adapt<TopBorrowersResponse>();
        }
    }
}
