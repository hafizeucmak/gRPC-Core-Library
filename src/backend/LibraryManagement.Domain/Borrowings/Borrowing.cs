using FluentValidation;
using LibraryManagement.Common.Base;
using LibraryManagement.Common.Constants;

namespace LibraryManagement.Domain.Borrowings
{
    public class Borrowing : DomainEntity
    {
        private readonly BorrowingValidator _validator = new();
        public Borrowing(int bookId, int userId)
        {
            BookId = bookId;
            UserId = userId;
            BorrowDate = DateTime.Now;
            ExpectedReturnDate = DateTime.Now.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay);

            _validator.ValidateAndThrow(this);
        }

        public int BookId { get; private set; }

        public int UserId { get; private set; }

        public DateTime BorrowDate { get; private set; }

        public DateTime? ReturnDate { get; private set; }

        public DateTime ExpectedReturnDate { get; private set; }

        public class BorrowingValidator : AbstractValidator<Borrowing>
        {
            public BorrowingValidator()
            {
                RuleFor(x => x.BookId).NotEmpty();
                RuleFor(x => x.UserId).NotEmpty();
            }
        }
    }
}
