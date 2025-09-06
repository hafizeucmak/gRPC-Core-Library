using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibraryManagement.Common.Interceptors
{
    public class GrpcRequestResponseLoggingInterceptor : Interceptor
    {
        private readonly ILogger<GrpcRequestResponseLoggingInterceptor> _logger;

        public GrpcRequestResponseLoggingInterceptor(
            ILogger<GrpcRequestResponseLoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });

            TResponse? response = default;
            ExceptionDetails? exceptionDetails = null;
            bool success = false;

            try
            {
                response = await continuation(request, context);
                success = true;
                return response;
            }
            catch (Exception ex)
            {
                exceptionDetails = new ExceptionDetails
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                };

                throw;
            }
            finally
            {
                var responseJson = response != null
                    ? JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true })
                    : null;

                var log = new GrpcRequestResponseLog
                {
                    Method = context.Method,
                    Request = request,
                    Response = response,
                    Success = success,
                    Exception = exceptionDetails,
                };

                _logger.LogInformation("Request/Response Log: {@Log}", log);
            }
        }
    }

    public class GrpcRequestResponseLog
    {
        public string Method { get; set; } = string.Empty;
        public object Request { get; set; } = string.Empty;
        public object? Response { get; set; }
        public bool Success { get; set; }
        public ExceptionDetails? Exception { get; set; }
    }

    public class ExceptionDetails
    {
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
    }
}
