### Library Management System

This repository contains the source code for a Library Management System, which consists of two main components: a Web API for managing books, users and borrowings, and three separate gRPC services for handling book-related and borrowing-related functionalities.

```
│   
└───App
│   └───LibraryManagement.WebApi
└───Common   
└───Services
│   └───LibraryManagement.BorrowingGrpcService
│   └───LibraryManagement.AssetsGrpcService
│   └───LibraryManagement.UserGrpcService
│   
│
└───Tests
```

##  REPOSITORY STRUCTURE

  └───## LibraryManagement.WebApi
        └──────Area
                └──────Controllers
                 └──────BorrowingsController
                 └──────UsersController
                 └──────AssetManagementController
                 └──────SeedsController

* BorrowingsController has 6 http get method for expected conditions
    * [getMostBorrowedBooks] => What are the most borrowed books? 
    * [getBookCopiesAvailability] => How many copies of a particular book are currently borrowed/available?
    * [getBorrowersAlsoBorrowedBooks] => What other books were borrowed by individuals that borrowed a particular book? 
    * [getTopBorrowersWithinSpecifiedTimeframe] => Which users borrowed the most books in a given time frame?
    * [getBorrowedBooksByUser] => What books has an individual borrowed in each time frame?
    * [getAverageReadRateForBook] => Roughly, what is the read rate (pages per day) for a particular book, assuming users start reading a book as soon as they borrow it and return it as soon as they are done reading it?



## HOW TO RUN

* The SeedsController's [HttpPost] method at `api/Seeds/Seed` is responsible for initiating a crucial operation. When invoked, this operation triggers the creation of a database and subsequently inserts seed data into it.

* The quantity of seed data can be adjusted by modifying the count parameter within the codebase. The root location of the seed service is situated within `LibraryManagement.BorrowGrpcService.Data.DataAccess.SeedData`.

## ACKNOWLEDMENTS 

* To prevent performance issues, I've integrated support for pagination features and implemented server-side filters. This enhancement utilizes an open-source library called DynamicQueryBuilder, which can be accessed on GitHub at the following link: `https://github.com/oplog/DynamicQueryBuilder`. For detailed usage instructions, please refer to the documentation available at: `https://oplog.github.io/DynamicQueryBuilder/dqb.html`.

* The `getMostBorrowedBooks` function now accepts a DynamicQueryOptions parameter for server-side data filtering. This implementation enables pagination and data filtering without the need to load all data into memory.

* PLEASE NOTE that the format for DynamicQueryOptions parameters is as follows: for pagination, use `&offset=0&count=10`, and for filtering, use `&o=Equals&p=foo&v=bar`. After referring to the documentation at https://oplog.github.io/DynamicQueryBuilder/dqb.html, make sure to use `&` instead of `?` as explained in the document.

## ADDITIONAL NOTES


In the project, I've adopted a microservices architecture approach, allowing flexibility for potential changes to the database in the future and enabling deployment on different servers. Each service within the architecture communicates through a RabbitMQ event-driven architecture. While the architecture is not fully implemented yet, you may notice some unimplemented methods in the project.

I've focused on implementing:

*  Custom exception management to handle errors gracefully within the application (is not fully implemented yet).
*  Transaction management to ensure data integrity, where any exceptions occurring after a transaction has been committed won't trigger event notifications to consumers.

* The separation of the Library Management System into asset, user, and borrowing services also facilitates event-driven communication between these components. This means that when significant events occur, such as the creation of a new book or copy, events are generated and propagated to other services, such as the Borrowing service. This event-driven approach ensures loose coupling between services and enables asynchronous communication, which enhances system responsiveness and scalability.

* Splitting the Library Management System into distinct services for managing assets (books and copies), users, and borrowings is a strategic decision aimed at improving system performance as user traffic grows in the future. This separation allows for more efficient resource allocation, better scalability, and improved handling of concurrent user requests, ultimately enhancing the overall performance of the system.