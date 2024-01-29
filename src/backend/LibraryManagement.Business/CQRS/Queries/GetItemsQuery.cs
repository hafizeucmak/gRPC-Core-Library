using MediatR;

namespace LibraryManagement.Business.CQRS.Queries
{
    public class GetItemsQuery : IRequest
    {
        public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery>
        {
            public GetItemsQueryHandler()
            {
            }

            public async Task Handle(GetItemsQuery query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException("salak");
            }
        }
    }
}
