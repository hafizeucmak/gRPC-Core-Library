using FluentValidation;
using LibraryManagement.Common.Constants;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using LibraryManagement.UserGrpcService.DataAccesses.DbContexts;
using LibraryManagement.UserGrpcService.Domains;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.UserGrpcService.Business.CQRS.Commands
{
    public class CreateUserCommand : IRequest<UserAddResponse>
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

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserAddResponse>
    {
        private readonly IGenericWriteRepository<UserBaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IGenericWriteRepository<UserBaseDbContext> genericWriteRepository, ILogger<CreateUserCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<UserAddResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var isUserAlreadyExists = await _genericWriteRepository.GetAll<User>().AnyAsync(x => x.Email.Equals(command.UserEmail), cancellationToken);

            if (isUserAlreadyExists)
            {
                throw new AlreadyExistsException($"{nameof(User)} with {nameof(command.UserEmail)} is equal to {command.UserEmail} already exists.");
            }

            var user = new User(command.UserEmail, command.UserName, command.FirstName, command.LastName, command.PhoneNumber);

            await _genericWriteRepository.AddAsync(user, cancellationToken);

            return new UserAddResponse();
        }
    }
}
