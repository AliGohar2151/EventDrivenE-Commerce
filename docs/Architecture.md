# Architecture Document

# Event-Driven E-Commerce Backend

**Project Name:** Event-Driven E-Commerce Backend
**Architecture Version:** 1.0
**Status:** Planned
**Target Framework:** .NET 10
**Language:** C#
**Primary Architecture:** Clean Architecture + Modular Monolith + Event-Driven Architecture
**Messaging:** RabbitMQ
**Database:** PostgreSQL
**Caching:** Redis
**Containerization:** Docker

---

# 1. Architecture Overview

The Event-Driven E-Commerce Backend will initially be implemented as a **modular monolith** using Clean Architecture principles.

The system will have clear module boundaries and asynchronous communication through events.

The initial architecture is intentionally not a distributed microservices architecture.

Instead, the project will begin as:

```text
Modular Monolith
        +
Clean Architecture
        +
Domain-Driven Design Principles
        +
CQRS
        +
Event-Driven Architecture
```

This approach provides the benefits of strong modular boundaries without introducing unnecessary distributed-system complexity too early.

The architecture should allow individual modules to be extracted into independent microservices in the future if required.

---

# 2. High-Level System Architecture

```text
                         ┌──────────────────────┐
                         │      Client Apps     │
                         │ Web / Mobile / Tools │
                         └──────────┬───────────┘
                                    │
                                    │ HTTPS
                                    ▼
                         ┌──────────────────────┐
                         │    ASP.NET Core API  │
                         │                      │
                         │ Controllers           │
                         │ Middleware            │
                         │ Authentication        │
                         │ Authorization         │
                         └──────────┬───────────┘
                                    │
                                    ▼
                 ┌────────────────────────────────────┐
                 │        Application Layer            │
                 │                                    │
                 │ Commands                            │
                 │ Queries                             │
                 │ Handlers                            │
                 │ Validators                          │
                 │ Application Services                 │
                 └─────────────────┬──────────────────┘
                                   │
                                   ▼
                 ┌────────────────────────────────────┐
                 │           Domain Layer              │
                 │                                    │
                 │ Entities                            │
                 │ Value Objects                       │
                 │ Domain Events                       │
                 │ Business Rules                      │
                 └─────────────────┬──────────────────┘
                                   │
                                   ▼
                 ┌────────────────────────────────────┐
                 │        Infrastructure Layer        │
                 │                                    │
                 │ EF Core                             │
                 │ PostgreSQL                          │
                 │ Redis                               │
                 │ RabbitMQ                            │
                 │ Authentication Services             │
                 │ External Providers                  │
                 └────────────────────────────────────┘
```

---

# 3. Architectural Principles

The project must follow these principles:

## 3.1 Dependency Inversion

Dependencies must point inward.

```text
API
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure
 │
 ├── Application
 └── Domain
```

The Domain layer must not depend on Infrastructure or API.

The Application layer must not depend directly on Infrastructure implementations.

Infrastructure implements interfaces defined by inner layers.

---

## 3.2 Separation of Concerns

Each layer has a clearly defined responsibility.

```text
Domain
Business rules

Application
Use cases

Infrastructure
External systems

API
HTTP communication
```

Controllers must remain thin.

Business logic must not be placed inside controllers.

---

## 3.3 Explicit Business Logic

Business rules must be represented explicitly.

Avoid hiding important business behavior inside:

- Controllers
- EF Core configurations
- Middleware
- Generic repositories
- Infrastructure services

Business rules should live in domain entities, value objects, domain services, or application use cases depending on their responsibility.

---

## 3.4 Asynchronous Processing

Operations that do not require immediate synchronous completion should be processed asynchronously.

Examples:

- Notifications
- Payment processing
- Inventory workflows
- Order events
- Audit processing

---

# 4. Solution Structure

The solution will use the following structure:

```text
ECommerce
│
├── src
│   │
│   ├── ECommerce.Api
│   │
│   ├── ECommerce.Application
│   │
│   ├── ECommerce.Domain
│   │
│   ├── ECommerce.Infrastructure
│   │
│   └── ECommerce.Contracts
│
├── tests
│   │
│   ├── ECommerce.UnitTests
│   │
│   ├── ECommerce.IntegrationTests
│   │
│   └── ECommerce.ArchitectureTests
│
├── docker
│   │
│   └── docker-compose.yml
│
├── .github
│   │
│   └── workflows
│
├── docs
│
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── Directory.Packages.props
├── ECommerce.sln
└── README.md
```

---

# 5. Project Responsibilities

## 5.1 ECommerce.Domain

The Domain project contains the core business model.

It must have no dependency on:

- ASP.NET Core
- EF Core
- PostgreSQL
- Redis
- RabbitMQ
- Infrastructure
- API

Expected structure:

```text
ECommerce.Domain
│
├── Common
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   └── Result.cs
│
├── Users
│   ├── User.cs
│   ├── Role.cs
│   └── Permission.cs
│
├── Catalog
│   ├── Product.cs
│   ├── Category.cs
│   └── ProductVariant.cs
│
├── Inventory
│   ├── InventoryItem.cs
│   └── StockReservation.cs
│
├── Cart
│   ├── Cart.cs
│   └── CartItem.cs
│
├── Orders
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── OrderStatus.cs
│
├── Payments
│   ├── Payment.cs
│   └── PaymentStatus.cs
│
├── Events
│   ├── DomainEvent.cs
│   └── ...
│
└── Exceptions
```

The exact structure may evolve as implementation progresses.

---

# 6. ECommerce.Application

The Application project contains application use cases.

It coordinates business operations without knowing implementation details of external systems.

Expected structure:

```text
ECommerce.Application
│
├── Abstractions
│   ├── Persistence
│   ├── Caching
│   ├── Messaging
│   ├── Authentication
│   └── Notifications
│
├── Behaviors
│   ├── ValidationBehavior.cs
│   ├── LoggingBehavior.cs
│   └── TransactionBehavior.cs
│
├── Features
│   │
│   ├── Authentication
│   │
│   ├── Users
│   │
│   ├── Products
│   │
│   ├── Categories
│   │
│   ├── Cart
│   │
│   ├── Inventory
│   │
│   ├── Orders
│   │
│   └── Payments
│
├── DTOs
│
├── Validators
│
└── DependencyInjection.cs
```

CQRS operations should be organized by feature.

Example:

```text
Features
└── Orders
    │
    ├── Commands
    │   ├── CreateOrder
    │   └── CancelOrder
    │
    └── Queries
        ├── GetOrder
        └── GetOrders
```

---

# 7. ECommerce.Infrastructure

Infrastructure contains implementations for external dependencies.

Expected structure:

```text
ECommerce.Infrastructure
│
├── Persistence
│   ├── ECommerceDbContext.cs
│   ├── Configurations
│   ├── Migrations
│   └── Repositories
│
├── Messaging
│   ├── RabbitMqConnection.cs
│   ├── EventPublisher.cs
│   ├── Consumers
│   └── Retry
│
├── Caching
│   └── RedisCacheService.cs
│
├── Authentication
│   ├── JwtTokenService.cs
│   └── RefreshTokenService.cs
│
├── Payments
│   └── MockPaymentProvider.cs
│
├── Notifications
│   └── MockNotificationProvider.cs
│
├── Observability
│
└── DependencyInjection.cs
```

Infrastructure must implement abstractions defined by the Application layer.

---

# 8. ECommerce.Api

The API project is responsible for HTTP communication.

Expected structure:

```text
ECommerce.Api
│
├── Controllers
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── CategoriesController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   └── UsersController.cs
│
├── Middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── CorrelationIdMiddleware.cs
│
├── Extensions
│
├── Filters
│
├── Health
│
├── OpenApi
│
└── Program.cs
```

Controllers should:

- Validate HTTP input through application validation.
- Dispatch commands or queries.
- Return appropriate HTTP responses.

Controllers must not contain domain business logic.

---

# 9. ECommerce.Contracts

Contracts define messages exchanged between modules and external messaging systems.

Example:

```text
ECommerce.Contracts
│
├── Orders
│   ├── OrderCreated.cs
│   └── OrderCancelled.cs
│
├── Payments
│   ├── PaymentRequested.cs
│   ├── PaymentSucceeded.cs
│   └── PaymentFailed.cs
│
├── Inventory
│   ├── InventoryReservationRequested.cs
│   ├── InventoryReserved.cs
│   └── InventoryReservationFailed.cs
│
└── Notifications
```

Integration events must be stable and versionable.

Internal domain events must not automatically become public integration events.

---

# 10. Module Boundaries

The initial system will contain these logical modules:

```text
Authentication & Identity
Catalog
Inventory
Cart
Orders
Payments
Notifications
```

Each module should own its business logic.

Example:

```text
Orders
   │
   ├── Order Entity
   ├── Order Rules
   ├── Order Commands
   └── Order Queries

Inventory
   │
   ├── Inventory Entity
   ├── Reservation Rules
   └── Inventory Commands
```

Modules must communicate through explicit interfaces or events.

They should not directly manipulate each other's internal entities.

---

# 11. Database Architecture

PostgreSQL will be the primary relational database.

Initially, the modular monolith will use a single PostgreSQL database.

Logical ownership will still be separated by module.

Example:

```text
PostgreSQL
│
├── Identity Tables
│
├── Catalog Tables
│
├── Inventory Tables
│
├── Cart Tables
│
├── Order Tables
│
└── Payment Tables
```

Where appropriate, database schemas may be used to reinforce module boundaries.

For example:

```text
identity.users
catalog.products
inventory.items
orders.orders
payments.payments
```

The exact schema strategy will be finalized during database implementation.

The project must avoid unnecessary cross-module database coupling.

---

# 12. Entity Framework Core

EF Core will be used for persistence.

The DbContext must be located in Infrastructure.

Entity configuration should use separate configuration classes.

Example:

```text
Persistence
│
├── ECommerceDbContext.cs
│
└── Configurations
    ├── UserConfiguration.cs
    ├── ProductConfiguration.cs
    ├── OrderConfiguration.cs
    └── ...
```

Business logic must not depend on EF Core-specific APIs.

EF Core tracking and querying decisions should be made deliberately.

Read-only queries should use `AsNoTracking()` where appropriate.

---

# 13. CQRS Architecture

The application will use CQRS to separate write and read operations.

```text
HTTP Request
     │
     ├───────────────┐
     │               │
     ▼               ▼
Command           Query
     │               │
     ▼               ▼
Command Handler   Query Handler
     │               │
     ▼               ▼
Domain Logic      Read Model
     │               │
     ▼               ▼
Database          Database
```

Commands change system state.

Queries retrieve data and must not modify system state.

CQRS does not require separate databases.

The initial implementation will use the same PostgreSQL database for both reads and writes.

---

# 14. Messaging Architecture

RabbitMQ will be used as the message broker.

The system will use asynchronous integration events.

Example:

```text
Order Module
      │
      │ Publish
      ▼
RabbitMQ Exchange
      │
      ├─────────────┐
      │             │
      ▼             ▼
Inventory Queue   Payment Queue
      │             │
      ▼             ▼
Inventory        Payment
Consumer         Consumer
```

Consumers must acknowledge messages only after successful processing.

Messages that fail processing will enter a retry workflow.

After retry exhaustion, messages will be routed to dead-letter handling.

---

# 15. RabbitMQ Topology

The initial RabbitMQ design will use topic-based routing.

Conceptually:

```text
Exchange
    │
    ├── order.created
    ├── order.cancelled
    ├── payment.requested
    ├── payment.succeeded
    ├── payment.failed
    ├── inventory.reserved
    └── inventory.failed
```

Example:

```text
OrderCreated
      │
      ▼
ecommerce.events
      │
      ├── inventory.order-created
      │
      ├── payment.order-created
      │
      └── notification.order-created
```

The final exchange and routing-key naming convention will be standardized before messaging implementation.

---

# 16. Event Processing Flow

A typical order workflow:

```text
Customer
   │
   │ POST /orders
   ▼
API
   │
   ▼
CreateOrderCommand
   │
   ▼
OrderCommandHandler
   │
   ▼
Create Order
   │
   ▼
Save Order
   │
   ▼
Publish OrderCreated
   │
   ▼
RabbitMQ
   │
   ├─────────────────────┐
   │                     │
   ▼                     ▼
Inventory Consumer    Payment Consumer
   │                     │
   ▼                     ▼
Reserve Stock         Process Payment
   │                     │
   ▼                     ▼
InventoryReserved     PaymentSucceeded
   │                     │
   └──────────┬──────────┘
              ▼
       Update Order State
              │
              ▼
      Notification Event
              │
              ▼
      Notification Consumer
```

The workflow must be designed to tolerate partial failures.

For example:

```text
Payment Succeeded
        +
Inventory Reservation Failed
        |
        v
Compensation Workflow
        |
        v
Refund / Cancel Order
```

The exact saga or orchestration strategy will be determined during implementation.

---

# 17. Redis Architecture

Redis will be used for:

- Shopping cart data
- Frequently accessed cache data
- Potential distributed locks where justified
- Potential idempotency records

Example:

```text
API
 │
 ▼
Application
 │
 ▼
Redis
 │
 ├── Cart
 ├── Cache
 └── Idempotency
```

Redis must not become the primary source of truth for critical transactional data unless explicitly justified.

PostgreSQL remains the source of truth for persistent business data.

---

# 18. Background Workers

Long-running and asynchronous operations will be processed through background consumers.

Conceptually:

```text
RabbitMQ
    │
    ▼
Background Consumer
    │
    ▼
Deserialize Message
    │
    ▼
Validate Message
    │
    ▼
Execute Handler
    │
    ├── Success
    │      │
    │      ▼
    │   Acknowledge
    │
    └── Failure
           │
           ▼
        Retry
           │
           ▼
     Dead Letter
```

Consumers must support cancellation and graceful shutdown.

---

# 19. Retry Architecture

Retry handling must distinguish between transient and permanent failures.

Example:

```text
Message
   │
   ▼
Consumer
   │
   ├── Success ──────────> ACK
   │
   └── Failure
          │
          ▼
   Is Retryable?
       │
    ┌──┴──┐
    │     │
   Yes    No
    │     │
    ▼     ▼
 Retry   Dead Letter
    │
    ▼
Max Attempts?
    │
    ├── No ──> Retry
    │
    └── Yes ─> Dead Letter
```

Retry policies should use:

- Exponential backoff
- Jitter
- Maximum retry count

The system must avoid tight retry loops.

---

# 20. Idempotency Architecture

Event consumers must assume that messages may be delivered more than once.

Example:

```text
Event ID: 123
       │
       ▼
Consumer
       │
       ▼
Check Idempotency Store
       │
       ├── Already Processed
       │       │
       │       ▼
       │      ACK
       │
       └── Not Processed
               │
               ▼
         Process Event
               │
               ▼
         Record Event ID
               │
               ▼
              ACK
```

The idempotency mechanism must be designed carefully to avoid race conditions.

Where possible, idempotency records and business state changes should be coordinated transactionally.

---

# 21. Dead-Letter Architecture

Messages that cannot be processed successfully after retries will be moved to dead-letter handling.

```text
Main Queue
     │
     ▼
Consumer
     │
     ▼
Failure
     │
     ▼
Retry Queue
     │
     ▼
Consumer
     │
     ▼
Maximum Retries
     │
     ▼
Dead Letter Queue
```

Dead-letter messages must retain enough metadata for debugging and replay.

Future versions may implement a dead-letter management API.

---

# 22. Authentication Architecture

Authentication flow:

```text
Client
   │
   │ Login
   ▼
Auth API
   │
   ▼
Validate Credentials
   │
   ▼
Generate Access Token
   │
   ▼
Generate Refresh Token
   │
   ▼
Client
```

Access tokens will be short-lived.

Refresh tokens will be stored securely and support revocation and rotation.

The API will validate JWT tokens through ASP.NET Core authentication middleware.

---

# 23. Authorization Architecture

Authorization will use:

```text
User
 │
 ▼
Roles
 │
 ▼
Permissions
```

Example:

```text
User
 └── Admin
      ├── Product.Read
      ├── Product.Create
      ├── Product.Update
      ├── Product.Delete
      └── Order.Read
```

Permission policies should be defined centrally.

Controllers should use authorization policies rather than implementing manual permission checks repeatedly.

---

# 24. API Request Flow

A typical API request will follow:

```text
HTTP Request
      │
      ▼
Correlation ID Middleware
      │
      ▼
Exception Handling Middleware
      │
      ▼
Authentication
      │
      ▼
Authorization
      │
      ▼
Controller
      │
      ▼
Command / Query
      │
      ▼
Pipeline Behaviors
      │
      ├── Validation
      ├── Logging
      └── Transaction
      │
      ▼
Handler
      │
      ▼
Domain
      │
      ▼
Persistence / External Services
      │
      ▼
Response
```

---

# 25. Exception Handling

All unhandled application exceptions must be processed by centralized exception handling.

The API must return consistent error responses.

Example:

```json
{
  "success": false,
  "data": null,
  "message": "An error occurred while processing the request.",
  "errors": []
}
```

Known business errors should return meaningful client-facing messages.

Unexpected errors must not expose:

- Stack traces
- Database details
- Internal implementation details
- Secrets
- Sensitive information

Errors must be logged with correlation information.

---

# 26. Observability Architecture

Observability will use three primary pillars:

```text
Logs
Metrics
Traces
```

Conceptually:

```text
Application
    │
    ├───────────────┐
    │               │
    ▼               ▼
 OpenTelemetry   Structured Logs
    │               │
    ▼               ▼
 Metrics         Log Storage
    │
    ▼
 Traces
```

Every important operation should be traceable using correlation and trace IDs.

Example:

```text
Trace ID: abc123

HTTP Request
    ↓
Create Order
    ↓
Database Transaction
    ↓
Publish Event
    ↓
RabbitMQ
    ↓
Inventory Consumer
    ↓
Payment Consumer
```

---

# 27. Docker Architecture

Local development will use Docker Compose.

Expected infrastructure:

```text
Docker Compose
│
├── PostgreSQL
├── Redis
├── RabbitMQ
└── ECommerce API
```

Conceptually:

```text
                 ┌───────────────┐
                 │ ECommerce API │
                 └───────┬───────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
        ▼                ▼                ▼
   PostgreSQL         Redis            RabbitMQ
```

The application must support environment-specific configuration.

---

# 28. Testing Architecture

Testing will be divided into:

```text
Unit Tests
    │
    ▼
Business Logic

Integration Tests
    │
    ▼
Database + Infrastructure

Architecture Tests
    │
    ▼
Dependency Rules
```

Unit tests should not require external infrastructure.

Integration tests may use:

- PostgreSQL
- Redis
- RabbitMQ

Testcontainers may be used to provide isolated infrastructure during testing.

---

# 29. CI/CD Architecture

GitHub Actions will be used for CI/CD.

The pipeline should eventually perform:

```text
Push / Pull Request
        │
        ▼
Restore
        │
        ▼
Build
        │
        ▼
Format Check
        │
        ▼
Unit Tests
        │
        ▼
Integration Tests
        │
        ▼
Architecture Tests
        │
        ▼
Docker Build
        │
        ▼
Security Checks
        │
        ▼
Publish / Deploy
```

Deployment will initially be optional.

The primary objective is to demonstrate automated quality checks and reproducible builds.

---

# 30. Future Microservice Extraction

The architecture must allow future extraction of modules.

Potential future services:

```text
Identity Service
Catalog Service
Inventory Service
Order Service
Payment Service
Notification Service
```

Possible future architecture:

```text
                    API Gateway
                        │
          ┌─────────────┼─────────────┐
          │             │             │
          ▼             ▼             ▼
      Catalog       Order         Identity
       Service      Service        Service
          │             │             │
          └─────────────┼─────────────┘
                        │
                    RabbitMQ
                        │
          ┌─────────────┼─────────────┐
          ▼             ▼             ▼
      Inventory      Payment      Notification
       Service       Service        Service
```

This is a future architectural direction, not part of the initial implementation.

The initial system must not introduce microservices solely for the sake of complexity.

---

# 31. Architectural Decision Principles

When making implementation decisions, the project should follow these principles:

1. Prefer simplicity over unnecessary abstraction.
2. Introduce complexity only when it solves a real problem.
3. Keep domain logic independent from frameworks.
4. Keep infrastructure concerns outside the domain.
5. Prefer explicit dependencies.
6. Prefer asynchronous processing for non-critical synchronous workflows.
7. Design event consumers to be idempotent.
8. Treat external systems as unreliable.
9. Make failures observable.
10. Make important operations testable.
11. Avoid premature optimization.
12. Measure performance before optimizing.
13. Preserve clear module boundaries.
14. Prefer composition over inheritance where appropriate.
15. Use abstractions only when they provide meaningful value.

---

# 32. Initial Architectural Scope

The first implementation will contain:

```text
ASP.NET Core API
        │
        ▼
Clean Architecture
        │
        ├── Domain
        ├── Application
        └── Infrastructure
        │
        ▼
PostgreSQL
        │
        ▼
Redis
        │
        ▼
RabbitMQ
        │
        ▼
Background Consumers
```

The architecture will evolve incrementally.

The project must not attempt to implement every advanced feature at the beginning.

Each phase must produce a working and testable system.

---

# 33. Final Architecture Vision

The final system should demonstrate the following engineering progression:

```text
ASP.NET Core
      │
      ▼
Clean Architecture
      │
      ▼
Modular Design
      │
      ▼
CQRS
      │
      ▼
PostgreSQL
      │
      ▼
Redis
      │
      ▼
RabbitMQ
      │
      ▼
Event-Driven Workflows
      │
      ▼
Background Processing
      │
      ▼
Retry + Idempotency
      │
      ▼
Dead-Letter Handling
      │
      ▼
Observability
      │
      ▼
Testing
      │
      ▼
Docker
      │
      ▼
CI/CD
```

The architecture should remain understandable throughout the project.

The goal is not to demonstrate the maximum number of technologies.

The goal is to demonstrate the ability to make sound architectural decisions and build a reliable, maintainable, observable, and testable .NET backend system.
