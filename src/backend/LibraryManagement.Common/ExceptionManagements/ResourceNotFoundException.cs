using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Common.ExceptionManagements
{
    public class ResourceNotFoundException
       : ValidationException

    {
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }
}
