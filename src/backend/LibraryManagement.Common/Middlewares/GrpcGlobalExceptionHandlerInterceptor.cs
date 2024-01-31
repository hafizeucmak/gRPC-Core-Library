using Grpc.Core;
using Grpc.Core.Interceptors;
using LibraryManagement.Common.Utils;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Common.Middlewares
{
    public class GrpcGlobalExceptionHandlerInterceptor : Interceptor
    {
        private readonly ILogger<GrpcGlobalExceptionHandlerInterceptor> _logger;
        private readonly IExceptionManager _grpcExceptionManager;

        public GrpcGlobalExceptionHandlerInterceptor(ILogger<GrpcGlobalExceptionHandlerInterceptor> logger, IExceptionManager grpcExceptionManager)
        {
            _logger = logger;
            _grpcExceptionManager = grpcExceptionManager;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (Exception thrownException)
            {
                var errorResponse = _grpcExceptionManager.ConstructExceptionModel(thrownException);

                throw new RpcException(new Status(errorResponse.StatusCode ?? StatusCode.Unknown, errorResponse?.Message ?? errorResponse?.InnerException ?? string.Empty));
            }
        }
    }
}
