using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Common.ExceptionManagements
{
    public class AlreadyExistsException : ValidationException

    {
        public AlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
