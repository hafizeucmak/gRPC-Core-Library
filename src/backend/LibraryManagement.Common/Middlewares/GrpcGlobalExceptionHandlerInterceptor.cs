using Grpc.Core.Interceptors;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Common.Middlewares
{
    public class GrpcGlobalExceptionHandlerInterceptor : Interceptor
    {
        private readonly ILogger<GrpcGlobalExceptionHandlerInterceptor> _logger;

        public GrpcGlobalExceptionHandlerInterceptor(ILogger<GrpcGlobalExceptionHandlerInterceptor> logger)
        {
            _logger = logger;
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
                //TODO: change this disgusting model name
                var responseVm = new ResponseViewModel
                {
                    Code = 34, //thrownException.Code,
                    Message = thrownException.Message
                };

                //TODO: implement generic exception handler you need to get and catch  all exception types and convert to meaningfull ones
                throw new RpcException(new Status(StatusCode.NotFound, $"ShoppingCart with UserName=anan canım is not found."));

            }
        }
    }
}
