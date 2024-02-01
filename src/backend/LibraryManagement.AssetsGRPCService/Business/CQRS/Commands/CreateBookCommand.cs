using FluentValidation;
using LibraryManagement.AssetsGRPCService.Business.Constants.EventConstants;
using LibraryManagement.AssetsGRPCService.Business.Events;
using LibraryManagement.AssetsGRPCService.Data.DataAccess.DbContexts;
using LibraryManagement.AssetsGRPCService.Domains;
using LibraryManagement.Common.Constants;
using LibraryManagement.Common.ExceptionManagements;
using LibraryManagement.Common.GenericRepositories;
using LibraryManagement.Common.RabbitMQEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.AssetsGRPCService.Business.CQRS.Commands
{
    public class CreateBookCommand : IRequest<BookAddResponse>
    {
        private readonly CreateBookCommandValidator _validator = new();
        public CreateBookCommand(string title,
                                 string authorName,
                                 string isbn,
                                 string publisherName,
                                 int publicationYear,
                                 int pageCount)
        {
            Title = title;
            AuthorName = authorName;
            Isbn = isbn;
            PublisherName = publisherName;
            PublicationYear = publicationYear;
            PageCount = pageCount;

            _validator.ValidateAndThrow(this);
        }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public string Isbn { get; set; }

        public string PublisherName { get; set; }

        public int PublicationYear { get; set; }

        public int PageCount { get; set; }
    }

    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_TITLE);
            RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_AUTHOR);
            RuleFor(x => x.Isbn).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_ISBN);
            RuleFor(x => x.PublisherName).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_BOOK_PUBLISHER);
            RuleFor(c => c.PublicationYear).NotEmpty().ExclusiveBetween(1000, DateTime.Today.Year);
            RuleFor(c => c.PageCount).NotEmpty().GreaterThan(0);
        }
    }


    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookAddResponse>
    {
        private readonly IGenericWriteRepository<AssetBaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateBookCommandHandler> _logger;
        private readonly RegisteredEventCommands _registeredEventCommands;

        public CreateBookCommandHandler(IGenericWriteRepository<AssetBaseDbContext> genericWriteRepository,
                                        ILogger<CreateBookCommandHandler> logger,
                                        RegisteredEventCommands registeredEventCommands)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
            _registeredEventCommands = registeredEventCommands;
        }

        public async Task<BookAddResponse> Handle(CreateBookCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var isUserAlreadyExists = await _genericWriteRepository.GetAll<Book>().AnyAsync(x => x.ISBN.Equals(command.Isbn), cancellationToken);

            if (isUserAlreadyExists)
            {
                throw new AlreadyExistsException($"{nameof(Book)} with {nameof(command.Isbn)} is equal to {command.Isbn} already exists.");
            }

            var book = new Book(command.Title, command.AuthorName, command.Isbn, command.PublisherName, command.PublicationYear, command.PageCount);

            _registeredEventCommands.RegisteredEventCommand(new QueueEventCommand<BookCreatedEvent>(EventConstants.BookCreatedQueueName));

            await _genericWriteRepository.AddAsync(book, cancellationToken);

            return new BookAddResponse();
        }
    }
}
