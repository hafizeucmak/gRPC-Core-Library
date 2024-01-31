using FluentValidation;
using LibraryManagement.UserGrpcService;
using LibraryManagement.WebApi.GrpcClients.Users;
using LibraryManagement.WebApi.Models;
using Mapster;
using MediatR;

namespace LibraryManagement.WebApi.CQRS.Queries.Users
{
    public class GetUserByEmail : IRequest<UserDetailsDTO>
    {
        private readonly GetUserByEmailValidator _validator = new();
        public GetUserByEmail(string email)
        {
            Email = email;

            _validator.ValidateAndThrow(this);
        }

        public string Email { get; set; }
    }

    public class GetUserByEmailValidator : AbstractValidator<GetUserByEmail>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmail, UserDetailsDTO>
    {
        private readonly IUserServiceClient _userServiceClient;

        public GetUserByEmailHandler(IUserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
        }

        public async Task<UserDetailsDTO> Handle(GetUserByEmail query, CancellationToken cancellationToken)
        {
            var result = await _userServiceClient.GetUserByEmailAsync(new UserByEmailRequest() { Email = query.Email });

            return result.Adapt<UserDetailsDTO>();
        }
    }
}
