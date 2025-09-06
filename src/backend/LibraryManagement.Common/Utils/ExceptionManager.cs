using Grpc.Core;
using LibraryManagement.Common.Base;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement.Common.Utils
{
    public class ExceptionManager : IExceptionManager
    {
        private readonly IDictionary<Type, StatusCode> _exceptionConfig;

        public ExceptionManager(IDictionary<Type, StatusCode> exceptionConfig)
        {
            _exceptionConfig = exceptionConfig;
        }

        public ErrorResponse Handle(Exception ex, out StatusCode grpcStatus, out int httpStatus)
        {
            if (_exceptionConfig.TryGetValue(ex.GetType(), out var grpcCode))
            {
                grpcStatus = grpcCode;

                httpStatus = grpcCode switch
                {
                    StatusCode.NotFound => StatusCodes.Status404NotFound,
                    StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
                    StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                    StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                    StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                };

                return new ErrorResponse
                {
                    StatusCode = grpcCode,
                    Code = httpStatus,
                    ErrorCode = grpcCode.ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }

            grpcStatus = StatusCode.Unknown;
            httpStatus = StatusCodes.Status500InternalServerError;

            return new ErrorResponse
            {
                StatusCode = grpcStatus,
                Code = httpStatus,
                ErrorCode = "InternalError",
                Message = ex.Message,
                InnerException = ex.InnerException?.Message
            };
        }
    }
}
