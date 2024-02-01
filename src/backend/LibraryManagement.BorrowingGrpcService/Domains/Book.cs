using FluentValidation;
using LibraryManagement.BorrowingGrpcService.Domains.Enums;
using LibraryManagement.Common.Base;

namespace LibraryManagement.BorrowingGrpcService.Domains
{
    public class Book : DomainEntity
    {
        private readonly BookValidator _validator = new();
        private readonly HashSet<BookCopy> _bookCopies = new();

        public Book(string title,
                       string author,
                       string isbn,
                       string publisher,
                       int publicationYear,
                       int pageCount)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            Publisher = publisher;
            PublicationYear = publicationYear;
            Status = AssetStatus.Available;
            PageCount = pageCount;

            _validator.ValidateAndThrow(this);
        }

        protected Book() { }

        public string Title { get; private set; }

        public string Author { get; private set; }

        public string ISBN { get; private set; }

        public string Publisher { get; private set; }

        public int PublicationYear { get; private set; }

        public int PageCount { get; private set; }

        public AssetStatus  Status { get; set; }

        public IReadOnlyCollection<BookCopy> BookCopies => _bookCopies;

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

        public class BookValidator : AbstractValidator<Book>
        {
            public BookValidator()
            {
                RuleFor(c => c.Title).NotEmpty();
                RuleFor(c => c.Author).NotEmpty();
                RuleFor(c => c.ISBN).NotEmpty();
                RuleFor(c => c.Publisher).NotEmpty();
                RuleFor(c => c.PageCount).GreaterThan(0);
                RuleFor(c => c.PublicationYear).NotEmpty().ExclusiveBetween(1000, DateTime.Today.Year);
            }
        }
    }
}
