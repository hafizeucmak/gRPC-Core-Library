using Grpc.Core;
using LibraryManagement.Common.Base;

namespace LibraryManagement.Common.Utils
{
    public class GrpcExceptionManager : IExceptionManager
    {
        private readonly IDictionary<Type, StatusCode> _exceptionStatusCodePair;

        public GrpcExceptionManager(IDictionary<Type, StatusCode> exceptionCodePair)
        {
            _exceptionStatusCodePair = exceptionCodePair;
        }
        public IErrorResponse ConstructExceptionModel(Exception exception)
        {
            StatusCode statusCode = GetExceptionStatusCode(exception);

            // bool isServerException = code / 100 == (int)HttpStatusCode.InternalServerError / 100;
            string errorMessage = exception.Message; //isServerException ? "Unknown Error" : ;
            string innerException = exception?.InnerException?.Message ?? string.Empty;

            var errorResponse = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = errorMessage,
                InnerException = innerException,
            };

            return errorResponse;
        }

        private StatusCode GetExceptionStatusCode<TException>(TException exception)
         where TException : Exception
        {
            if (_exceptionStatusCodePair.TryGetValue(exception.GetType(), out var code))
            {
                return code;
            }

            return StatusCode.Unknown;
        }
    }
}
