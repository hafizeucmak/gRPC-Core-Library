using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Common.Filters
{
    public class TransactionManagerInterceptor<TDbContext> : Interceptor
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public TransactionManagerInterceptor(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {

            try
            {
                var response = await continuation(request, context);

                // var eventStore = (TEventStoreContext)context.GetHttpContext().RequestServices.GetService(typeof(TEventStoreContext));

                if (_dbContext.Database?.CurrentTransaction != null)
                {
                    _dbContext.SaveChanges();
                    _dbContext.Database.CurrentTransaction.Commit();
                }

                return response;
            }
            catch
            {
                if (_dbContext.Database?.CurrentTransaction != null)
                {
                    _dbContext.Database.RollbackTransaction();
                }

                throw;
            }
        }
    }
}
