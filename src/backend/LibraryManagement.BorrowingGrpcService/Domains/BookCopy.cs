using FluentValidation;
using Grpc.Core;
using LibraryManagement.BorrowingGrpcService.Domains.Enums;
using LibraryManagement.Common.Base;
using System.Text;

namespace LibraryManagement.BorrowingGrpcService.Domains
{
    public class BookCopy : DomainEntity
    {
        private readonly BookCopyValidator _validator = new();

        public BookCopy(int bookId, Book book)
        {
            BookId = bookId;
            Book = book;
            AcquisitionDate = DateTime.UtcNow;
            CopyNumber = GenerateCopyNumber();
            Status = AssetStatus.Available;

            _validator.ValidateAndThrow(this);
        }

        protected BookCopy() { }

        public int BookId { get; private set; }

        public string CopyNumber { get; private set; }

        public DateTime AcquisitionDate { get; set; }

        public virtual Book Book { get; private set; }

        public AssetStatus Status { get; private set; }

        public void UpdateStatusAsBorrowed()
        {
            if (Status == AssetStatus.Borrowed)
            {
                throw new InvalidOperationException("Status already borrowed");
            }

            this.Status = AssetStatus.Borrowed;
            Update();
        }

        public void UpdateStatusAsAvailable()
        {
            if (Status == AssetStatus.Available)
            {
                throw new InvalidOperationException("Status already available");
            }

            this.Status = AssetStatus.Available;
            Update();
        }

        protected string GenerateCopyNumber()
        {
            string isbn = Book.ISBN;

            StringBuilder copyNumberBuilder = new();

            copyNumberBuilder.Append(isbn);
            copyNumberBuilder.Append('-');
            copyNumberBuilder.Append(Guid.NewGuid());

            string copyNumber = copyNumberBuilder.ToString();

            return copyNumber;
        }

        public class BookCopyValidator : AbstractValidator<BookCopy>
        {
            public BookCopyValidator()
            {
                RuleFor(c => c.BookId).NotEmpty();
                RuleFor(c => c.CopyNumber).NotEmpty();
            }
        }
    }
}
