using Grpc.Core;
using LibraryManagement.Common.Utils;
using System.Text.Json;

namespace LibraryManagement.WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionManager _exceptionManager;

        public ExceptionHandlerMiddleware(RequestDelegate next, IExceptionManager exceptionManager)
        {
            _next = next;
            _exceptionManager = exceptionManager;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (RpcException rpcEx)
            {
                await HandleRpcException(httpContext, rpcEx, logger);
            }
            catch (Exception ex)
            {
                await HandleGenericException(httpContext, ex, logger);
            }
        }

        private async Task HandleRpcException(HttpContext httpContext, RpcException rpcEx, ILogger logger)
        {
            var httpStatus = rpcEx.StatusCode switch
            {
                StatusCode.NotFound => 404,
                StatusCode.AlreadyExists => 409,
                StatusCode.InvalidArgument => 400,
                StatusCode.Unauthenticated => 401,
                StatusCode.PermissionDenied => 403,
                _ => 500
            };

            var response = new
            {
                Success = false,
                Code = httpStatus,
                ErrorCode = rpcEx.StatusCode.ToString(),
                Message = rpcEx.Status.Detail,
                TraceId = httpContext.TraceIdentifier
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = httpStatus;

            logger.LogError("RPC Exception: {@Response}", response);

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                })
            );
        }

        private async Task HandleGenericException(HttpContext httpContext, Exception ex, ILogger logger)
        {
            var errorResponse = _exceptionManager.Handle(ex, out var grpcStatus, out var httpStatus);

            var response = new
            {
                Success = false,
                Code = errorResponse.Code,      
                ErrorCode = errorResponse.ErrorCode, 
                Message = errorResponse.Message,
                InnerException = errorResponse.InnerException,
                TraceId = httpContext.TraceIdentifier
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = httpStatus;

            logger.LogError("Exception: {@Response}", response);

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                })
            );
        }
    }
}
