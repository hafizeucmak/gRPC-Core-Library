# 📚 Library Management System (gRPC + CQRS + Microservices)

## 🚀 Overview
This project is a **microservice-oriented library management system** built with **gRPC** for inter-service communication and **ASP.NET Core WebAPI** as the API gateway.  
The system is designed with a **CQRS pattern**, **clean architecture principles**, and focuses on **scalability, performance, and maintainability**.  
It was developed as part of an **assessment project** to demonstrate microservice best practices.

## 🏗️ Architecture

### API Gateway (WebAPI Layer)
- Communicates with gRPC services.
- Implements **CQRS** with separate **Commands** and **Queries**.
- Includes **Exception Handling Middleware** for structured error responses.
- Read and write repositories are separated → enables **future read replicas** for analytics/performance.
- **Customized Logging** with **Serilog** for structured and queryable logs.

### Microservices
- **Assets Service** – manages books and copies.
- **Borrowings Service** – handles borrow/return operations.
- **Users Service** – manages user profiles.
- Services are developed in a **microservice style** within the same solution for demonstration purposes. Each service would typically be a separate repository with its own deployments.

### Common Layer
- Shared contracts, exception manager, and utilities.
- Facilitates consistency across services and avoids duplicate code.

### Domain Layer
- Entities enforce invariants using **FluentValidation**.
- Entities encapsulate behavior (e.g., `Book.UpdateStatusAsBorrowed()`).
- Fields that should not be mutated externally are marked as **private setters**.
- Validation rules are defined **inside the entity**, ensuring domain consistency.

### Infrastructure Layer
- Entity Framework Core with **clustered/non-clustered indexes**.
- Each service owns its own database → ensures **bounded context** and **data ownership**.
- Database configurations include **constraints and indexes** for performance optimization.

### Event-Driven Architecture
- Implemented with **RabbitMQ**.
- Example:
  - When a book is created in the **Assets Service**, a `BookCreatedEvent` is published.
  - The **Borrowings Service** consumes this event to maintain consistency in its local database.
- Event consumer implementation was planned but not completed due to time constraints.

### Transaction Management Middleware
- Wraps requests in a **transaction scope**.
- On failure → transaction rollback.
- Rolled-back transactions **do not trigger events**, ensuring **eventual consistency**.

---

## 🗂️ Repository Structure
```
LibraryManagement/
├── LibraryManagement.WebApi/ # API Gateway, CQRS, Middleware
├── LibraryManagement.Common/ # Shared utilities, Exception Manager
├── LibraryManagement.AssetsGRPCService/
│ ├── Domains/ # Entities, Domain Logic
│ ├── Data/ # EF Core DbContext, Configurations
│ └── Services/ # gRPC Services & Handlers
├── LibraryManagement.BorrowingsGRPCService/
│ ├── Domains/
│ ├── Data/
│ └── Services/
├── LibraryManagement.UsersGRPCService/
│ ├── Domains/
│ ├── Data/
│ └── Services/
└── Tests/ Unit and Integration Tests
```


---

## ✨ Features
- gRPC-based inter-service communication.
- WebAPI layer with **CQRS** and repository segregation (**read/write separation**).
- Structured **exception handling** (interceptors in gRPC, middleware in WebAPI).
- **Serilog logging** with structured, queryable logs.
- Mapster for lightweight and efficient object mapping.
- FluentValidation for **domain invariants**.
- RabbitMQ for **event-driven communication**.
- Database optimizations with **indexes, constraints, and query shaping**.
- Clustered indexes on frequently queried fields.
- Shared **Exception Manager** ensures consistent error structures across services.

---

## ⚡ Performance Considerations
- **Efficient Queries**
  - Queries only project necessary fields (no over-fetching).
  - Critical fields indexed with clustered/non-clustered indexes.
  - Example: calculating **Top Borrowers** with optimized grouping, filtering, and ordering.

- **Pagination & Filtering**
  - Used **DQB (Dynamic Query Builder)** library for DB-level filtering & pagination.
  - Avoided in-memory queries to reduce performance bottlenecks.

---

## 🔮 Future Enhancements
- **Caching**
  - Introduce caching for non-frequently changing data (e.g., user details).
- **Containerization & Deployment**
  - Dockerize each service and configure deployment pipelines.
  - CI/CD pipelines to build, test, and deploy services automatically.
- **Security**
  - Add **JWT-based authentication** and **authorization policies**.
  - Enforce role-based access for APIs and gRPC calls.
- **Event Consumers**
  - Implement consumers for published RabbitMQ events to complete **event-driven workflows**.

---

## 🛠️ Tech Stack
- **.NET 8**, **ASP.NET Core WebAPI**
- **gRPC** for RPC communication
- **Entity Framework Core** with SQL Server
- **RabbitMQ** for event-driven messaging
- **Serilog** for structured logging
- **Mapster** for object mapping
- **FluentValidation** for validation
- **CQRS pattern** with read/write separation
- **xUnit** unit, functional and integration tests

---

## 🏃 How to Run

1. **Select Multiple Projects**  
   Run all gRPC services along with the Web API.  

2. **Configure Local Database**  
   Update your local database credentials in `appsettings.json`.
   
3. Open link http://localhost:5078/swagger/index.html?urls.primaryName=v2
  
4. **Seed Database**  
   Execute the `POST /api/Seeds/Seed` endpoint in `SeedsController` to create the database and insert initial seed data.  

## ACKNOWLEDMENTS 

* To prevent performance issues, I've integrated support for pagination features and implemented server-side filters. This enhancement utilizes an open-source library called DynamicQueryBuilder, which can be accessed on GitHub at the following link: `https://github.com/oplog/DynamicQueryBuilder`. For detailed usage instructions, please refer to the documentation available at: `https://oplog.github.io/DynamicQueryBuilder/dqb.html`.

* The `getMostBorrowedBooks` function now accepts a DynamicQueryOptions parameter for server-side data filtering. This implementation enables pagination and data filtering without the need to load all data into memory.

* PLEASE NOTE that the format for DynamicQueryOptions parameters is as follows: for pagination, use `&offset=0&count=10`, and for filtering, use `&o=Equals&p=foo&v=bar`. After referring to the documentation at https://oplog.github.io/DynamicQueryBuilder/dqb.html, make sure to use `&` instead of `?` as explained in the document.

