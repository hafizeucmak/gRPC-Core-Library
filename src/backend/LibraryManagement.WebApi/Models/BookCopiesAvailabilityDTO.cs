using System;

namespace LibraryManagement.WebApi.Models
{
    public class BookCopiesAvailabilityDTO
    {
        public int BorrowedCopiesCount { get; set; }

        public int AvailableCopiesCount { get; set; }
    }
}
