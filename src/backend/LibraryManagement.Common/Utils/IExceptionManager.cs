using Grpc.Core;
using LibraryManagement.Common.Base;

namespace LibraryManagement.Common.Utils
{
    public interface IExceptionManager
    {
        ErrorResponse Handle(Exception ex, out StatusCode grpcStatus, out int httpStatus);
    }
}
