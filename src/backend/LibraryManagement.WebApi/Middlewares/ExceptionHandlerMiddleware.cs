using Grpc.Core;
using LibraryManagement.Common.Base;
using LibraryManagement.Common.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static LibraryManagement.Common.Utils.ExceptionManager;

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
            //TODO:
            catch (RpcException rcpEx)
            { 
                var errorResponse = new ErrorResponse
                {
                    Code = 45,
                    Message = rcpEx.Status.Detail,
                    InnerException = rcpEx.InnerException?.Message ?? string.Empty,
                    ExceptionContent = rcpEx.ToString(),
                };

                httpContext.Response.StatusCode = 45;
                httpContext.Response.ContentType = "application/json";
                string exceptionMessageBody = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });

                await httpContext.Response.WriteAsync(exceptionMessageBody);
                await httpContext.Response.WriteAsync(exceptionMessageBody);

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
