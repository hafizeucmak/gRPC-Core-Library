using FluentValidation;
using LibraryManagement.Common.Base;
using LibraryManagement.Common.Constants;

namespace LibraryManagement.BorrowingGrpcService.Domains
{
    public class Borrowing : DomainEntity
    {
        private readonly BorrowingValidator _validator = new();
        public Borrowing(int bookId, Book book,int userId, int? bookCopyId)
        {
            BookId = bookId;
            UserId = userId;
            BookCopyId = bookCopyId;
            BorrowDate = DateTime.Now;
            Book = book;
            ExpectedReturnDate = DateTime.Now.AddDays(LibraryManagementConstants.BorrowingsExpectedReturnDay);

            _validator.ValidateAndThrow(this);
        }

        protected Borrowing() { }

        public int BookId { get; private set; }

        public int? BookCopyId { get; private set; }

        public int UserId { get; private set; }

        public DateTime BorrowDate { get; private set; }

        public DateTime? ReturnDate { get; private set; }

        public DateTime ExpectedReturnDate { get; private set; }

        public virtual Book Book { get; private set; }

        public virtual User User { get; private set; }

        public virtual BookCopy BookCopy { get; private set; }

        public void Returned(DateTime returnDate)
        {
            ReturnDate = returnDate;
            Update();
        }

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
