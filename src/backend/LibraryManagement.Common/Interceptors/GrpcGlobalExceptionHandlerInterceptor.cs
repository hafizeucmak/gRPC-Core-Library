using Grpc.Core;
using Grpc.Core.Interceptors;
using LibraryManagement.Common.Utils;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Common.Interceptors
{
    public class GrpcGlobalExceptionHandlerInterceptor : Interceptor
    {
        private readonly ILogger<GrpcGlobalExceptionHandlerInterceptor> _logger;
        private readonly IExceptionManager _exceptionManager;

        public GrpcGlobalExceptionHandlerInterceptor(ILogger<GrpcGlobalExceptionHandlerInterceptor> logger, IExceptionManager exceptionManager)
        {
            _logger = logger;
            _exceptionManager = exceptionManager;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await base.UnaryServerHandler(request, context, continuation);
            }
            catch (Exception ex)
            {
                var response = _exceptionManager.Handle(ex, out var grpcStatus, out _);

                var status = new Status(grpcStatus, response?.Message ?? "unknown error");

                var metadata = new Metadata();
                metadata.Add("error-code", response.Code.ToString() ?? string.Empty);
                metadata.Add("error-message", response.Message ?? string.Empty);
                metadata.Add("trace-id", Guid.NewGuid().ToString());


                throw new RpcException(status, metadata);

            }
        }
    }
}
