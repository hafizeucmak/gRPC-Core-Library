using Grpc.Core;
using LibraryManagement.Common.Base;
using LibraryManagement.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LibraryManagement.WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
                                    IExceptionManager exceptionManager,
                                    ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (RpcException rcpException)
            {
                var errorResponse = new ErrorResponse
                {
                    //TODO: resolve code issue
                    StatusCode = rcpException.StatusCode,
                    Message = rcpException.Status.Detail,
                    InnerException = rcpException.InnerException?.Message ?? string.Empty,
                    ExceptionContent = rcpException.ToString(),
                };

                //TODO: resolve status code issue
                httpContext.Response.StatusCode = 45;

                httpContext.Response.ContentType = "application/json";
                string exceptionMessageBody = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

                await httpContext.Response.WriteAsync(exceptionMessageBody);

                var logException = new LogExceptionDetails();
                logException.StackTrace = rcpException.StackTrace;
                logException.InnerException = rcpException.InnerException?.Message;
                logException.ExceptionMessage = rcpException.Message;
                //TODO: resolve code issue
                logException.ResultCode = 45;

                logger.LogError("Exception Log Details: {@Log}", logException);

            }
            catch (Exception thrownException)
            {
                var errorResponse = exceptionManager.ConstructExceptionModel(thrownException);

                string exceptionMessageBody = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

                httpContext.Response.StatusCode = errorResponse.Code;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(exceptionMessageBody);

                var logException = new LogExceptionDetails();
                logException.StackTrace = thrownException.StackTrace;
                logException.InnerException = thrownException.InnerException?.Message;
                logException.ExceptionMessage = thrownException.Message;
                logException.ResultCode = httpContext.Response.StatusCode;

                logger.LogError("Exception Log Details: {@Log}", logException);
            }
        }
    }
}
