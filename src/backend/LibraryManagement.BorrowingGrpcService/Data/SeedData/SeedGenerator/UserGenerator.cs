using Bogus;
using LibraryManagement.BorrowingGrpcService.Domains;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.SeedGenerator
{
    public class UserGenerator
    {
        public static List<User> GenerateUser(int amountToGenerate)
        {
            var userFaker = new Faker<User>().CustomInstantiator(f =>
            {
                var book = new User(f.Person.Email, f.Person.UserName, f.Person.FirstName, f.Person.LastName, f.Person.Phone);
                return book;

            });

            return userFaker.Generate(amountToGenerate);
        }
    }
}
