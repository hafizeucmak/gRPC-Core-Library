using FluentValidation;
using LibraryManagement.Common.Base;
using LibraryManagement.Domain.Entities.Copies;

namespace LibraryManagement.Domain.Entities.Books
{
    public class Book : DomainEntity
    {
        private readonly BookValidator _validator = new();
        private readonly HashSet<BookCopy> _bookCopies = new();

        public Book(string title,
                       string author,
                       string isbn,
                       string publisher,
                       int publicationYear)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            Publisher = publisher;
            PublicationYear = publicationYear;

            _validator.ValidateAndThrow(this);
        }

        protected Book() { }

        public string Title { get; private set; }

        public string Author { get; private set; }

        public string ISBN { get; private set; }

        public string Publisher { get; private set; }

        public int PublicationYear { get; private set; }

        public IReadOnlyCollection<BookCopy> BookCopies => _bookCopies;

        public class BookValidator : AbstractValidator<Book>
        {
            public BookValidator()
            {
                RuleFor(c => c.Title).NotEmpty();
                RuleFor(c => c.Author).NotEmpty();
                RuleFor(c => c.ISBN).NotEmpty();
                RuleFor(c => c.Publisher).NotEmpty();
                RuleFor(c => c.PublicationYear).NotEmpty().ExclusiveBetween(1000, DateTime.Today.Year);
            }
        }
    }
}
