﻿using FluentValidation;
using LibraryManagement.Common.Base;
using LibraryManagement.Domain.Books;
using System.Text;

namespace LibraryManagement.Domain.Copies
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

            _validator.ValidateAndThrow(this);
        }

        public int BookId { get; private set; }

        public string CopyNumber { get; private set; }

        public DateTime AcquisitionDate { get; set; }

        public virtual Book Book { get; private set; }

        protected string GenerateCopyNumber()
        {
            int copyId = this.Id;
            string isbn = this.Book.ISBN;

            StringBuilder copyNumberBuilder = new();

            copyNumberBuilder.Append(isbn);
            copyNumberBuilder.Append('-');
            copyNumberBuilder.Append(copyId);

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