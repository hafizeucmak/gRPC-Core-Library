using Grpc.Core;
using Grpc.Core.Interceptors;
using LibraryManagement.Common.RabbitMQEvents;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Common.Filters
{
    public class TransactionManagerInterceptor<TDbContext> : Interceptor
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly IEventCommandQueue _eventCommandQueue;
        private readonly RegisteredEventCommands _registeredEventCommands;

        public TransactionManagerInterceptor(TDbContext dbContext, IEventCommandQueue eventCommandQueue, RegisteredEventCommands registeredEventCommands)
        {
            _dbContext = dbContext;
            _eventCommandQueue = eventCommandQueue;
            _registeredEventCommands = registeredEventCommands;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {

            try
            {
                var response = await continuation(request, context);

      

                if (_dbContext.Database?.CurrentTransaction != null)
                {
                    _dbContext.SaveChanges();
                    _dbContext.Database.CurrentTransaction.Commit();
                }

                if (_registeredEventCommands.Any())
                {
                    _eventCommandQueue.Queue(_registeredEventCommands);
                    _eventCommandQueue.Signal();
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
