using Bogus;
using LibraryManagement.BorrowingGrpcService.Domains;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator
{
    public class AssetsGenerator
    {
        public static List<Book> GenerateBooks(int amountToGenerate, int startIndex = 1)
        {
            var bookFaker = new Faker<Book>().CustomInstantiator(f =>
           {
               var book = new Book(f.Lorem.Word(), f.Person.FullName, f.Commerce.Ean8(), f.Company.CompanyName(), f.Random.Number(2000, 2023), f.Random.Number(1, 50000));


               return book;
           }).RuleFor(p => p.ISBN, (faker, p) => $"ISBN-{startIndex++}");

            return bookFaker.Generate(amountToGenerate);
        }

        public static List<BookCopy> GenerateCopiesOfBooks(List<Book> books, int amountToGenerateEachBook = 0)
        {
            var bookCopies = new List<BookCopy>();

            foreach (var book in books)
            {
                var bookCopyFaker = new Faker<BookCopy>().CustomInstantiator(f =>
                {
                    return new BookCopy(book.Id, book);
                });

                var copies = amountToGenerateEachBook != 0 ? bookCopyFaker.Generate(amountToGenerateEachBook) : bookCopyFaker.GenerateBetween(3, 100);
                bookCopies.AddRange(copies);
            }

            return bookCopies;
        }
    }
}
