using LibraryManagement.Common.Base;

namespace LibraryManagement.Common.Utils
{
    public interface IExceptionManager
    {
        IErrorResponse ConstructExceptionModel(Exception exception);
    }
}
