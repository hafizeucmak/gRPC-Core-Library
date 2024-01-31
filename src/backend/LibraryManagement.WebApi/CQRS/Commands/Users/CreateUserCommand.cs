using FluentValidation;
using LibraryManagement.Common.Constants;
using LibraryManagement.UserGrpcService;
using LibraryManagement.WebApi.GrpcClients.Users;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Commands.Users
{
    public class CreateUserCommand : IRequest
    {
        private readonly CreateUserCommandValidator _validator = new();
        public CreateUserCommand(string userEmail,
                                 string userName,
                                 string firstName,
                                 string lastName,
                                 string phoneNumber)
        {

            UserEmail = userEmail;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;

            _validator.ValidateAndThrow(this);
        }

        public string UserEmail { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.UserEmail).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);
            RuleFor(c => c.LastName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PERSON_NAMES);
            RuleFor(c => c.PhoneNumber).NotEmpty();
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserServiceClient userServiceClient, ILogger<CreateUserCommandHandler> logger)
        {
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            //TODO: check user if exists with email address than throw exception

            var userRecord = new UserAddRequest
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                UserEmail = command.UserEmail,
                PhoneNumber = command.PhoneNumber
            };

            await _userServiceClient.AddUserAsync(userRecord);
        }
    }
}
