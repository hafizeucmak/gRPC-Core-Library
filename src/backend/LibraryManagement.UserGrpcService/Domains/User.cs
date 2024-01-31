using FluentValidation;
using LibraryManagement.Common.Base;

namespace LibraryManagement.UserGrpcService.Domains
{
    public class User : DomainEntity
    {
        private readonly UserValidator _validator = new();
        public User(string email,
                    string userName,
                    string firstName,
                    string lastName,
                    string phoneNumber)
        {
            Email = email;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{firstName} {lastName}";
            PhoneNumber = phoneNumber;

            _validator.ValidateAndThrow(this);
        }

        protected User() { }

        public string Email { get; private set; }

        public string UserName { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName { get; private set; }

        public string PhoneNumber { get; private set; }

        public class UserValidator : AbstractValidator<User>
        {
            public UserValidator()
            {
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.FirstName).NotEmpty();
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.PhoneNumber).NotEmpty()
                                           .WithMessage("Phone number is required.")
                                           .Matches(@"^\+(?:[0-9] ?){6,14}[0-9]$")
                                           .WithMessage("Invalid phone number format.");
            }
        }
    }
}
