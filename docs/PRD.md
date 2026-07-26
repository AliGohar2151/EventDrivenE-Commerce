# Product Requirements Document (PRD)

# Event-Driven E-Commerce Backend

**Project Name:** Event-Driven E-Commerce Backend
**Project Type:** Production-Style Backend Engineering Project
**Primary Language:** C#
**Framework:** ASP.NET Core
**Target Framework:** .NET 10
**Architecture:** Clean Architecture + Modular Design + Event-Driven Architecture
**Status:** Planning

---

## 1. Product Overview

The Event-Driven E-Commerce Backend is a production-focused backend system designed to demonstrate the development of a scalable, maintainable, and resilient e-commerce platform using modern .NET technologies and software engineering practices.

The system will provide the core backend capabilities required by a modern e-commerce platform, including user authentication, role-based authorization, product catalog management, shopping carts, inventory management, order processing, payment workflows, and notifications.

The project will use an event-driven architecture to decouple major business operations and allow asynchronous processing of tasks that do not need to execute synchronously within an HTTP request.

The system will demonstrate practical implementation of:

- ASP.NET Core Web API
- Clean Architecture
- Domain-Driven Design principles
- CQRS
- Entity Framework Core
- PostgreSQL
- Redis
- RabbitMQ
- Background Workers
- Event-Driven Architecture
- JWT Authentication
- Role-Based Access Control
- Permission-Based Authorization
- Retry and resilience patterns
- Idempotent event processing
- Dead-letter handling
- Structured logging
- Health checks
- OpenTelemetry
- Docker
- Automated testing
- CI/CD

The primary purpose of the project is to build a realistic backend system that can serve as a strong professional portfolio project and demonstrate the developer's ability to design and implement production-grade .NET backend systems.

---

# 2. Problem Statement

Traditional monolithic e-commerce applications often tightly couple order processing, inventory management, payment processing, and notifications.

This can create several problems:

- A failure in one component can affect unrelated functionality.
- Long-running operations can block HTTP requests.
- High traffic can overload databases and external services.
- Synchronous communication increases system coupling.
- Retry failures can create duplicate operations.
- Temporary service failures can cause unnecessary request failures.
- Background operations may lack observability and reliable error handling.

This project aims to address these challenges by introducing asynchronous event-driven workflows and clear separation of responsibilities between application modules.

For example:

```text
Customer places order
        |
        v
Order Created
        |
        v
OrderCreated Event
        |
        v
Message Broker
        |
        +------------------+
        |                  |
        v                  v
Inventory Consumer   Payment Consumer
        |                  |
        v                  v
Reserve Stock        Process Payment
        |                  |
        +--------+---------+
                 |
                 v
          Order State Update
                 |
                 v
        Notification Consumer
                 |
                 v
           Notify Customer
```

The system will be designed to handle failures, retries, duplicate events, and temporary infrastructure outages in a controlled and observable manner.

---

# 3. Product Goals

The primary goals of the project are:

## 3.1 Build a Production-Style .NET Backend

Create a backend application that demonstrates professional .NET development practices rather than a basic CRUD API.

## 3.2 Demonstrate Event-Driven Architecture

Use domain events and integration events to decouple business processes and support asynchronous workflows.

## 3.3 Demonstrate Scalable Backend Design

Design the system so that individual components can be scaled independently and future modules can be extracted into separate services if required.

## 3.4 Demonstrate Resilience

Implement:

- Retry policies
- Exponential backoff
- Jitter
- Circuit breakers where appropriate
- Idempotent event processing
- Dead-letter handling

## 3.5 Demonstrate Observability

Provide visibility into application behavior through:

- Structured logging
- Correlation IDs
- Health checks
- Metrics
- Distributed tracing

## 3.6 Demonstrate Testing

Provide automated tests covering:

- Domain logic
- Application use cases
- API endpoints
- Database interactions
- Event-driven workflows

## 3.7 Create a Strong Portfolio Project

The project should be suitable for:

- GitHub publication
- CV inclusion
- LinkedIn presentation
- Technical interviews
- Architecture discussions

---

# 4. Target Users

The system has multiple user types.

## 4.1 Customer

A customer can:

- Register an account
- Log in
- Manage their profile
- Browse products
- Search products
- Filter products
- Add products to a cart
- Update cart quantities
- Remove products from a cart
- Place orders
- View order history
- View order details
- Track order status

## 4.2 Administrator

An administrator can:

- Manage users
- Manage roles
- Manage permissions
- Manage products
- Manage categories
- Manage inventory
- View all orders
- Manage order statuses
- View system activity

## 4.3 Store Manager

A store manager can:

- Manage products
- Manage categories
- View inventory
- Update inventory
- View orders
- Process orders

The exact permissions will be controlled through the authorization system rather than hard-coded role checks wherever practical.

---

# 5. Core Functional Requirements

## 5.1 Authentication

The system must support:

- User registration
- User login
- Secure password hashing
- JWT access tokens
- Refresh tokens
- Token expiration
- Token rotation
- Logout / refresh token revocation
- Account activation where required

Authentication must not expose sensitive information such as passwords or password hashes through API responses.

---

## 5.2 Authorization

The system must implement role-based and permission-based authorization.

Example:

```text
Role
 |
 +-- Permissions
       |
       +-- Product.Read
       +-- Product.Create
       +-- Product.Update
       +-- Product.Delete
       +-- Order.Read
       +-- Order.Update
```

The system should avoid scattering authorization logic throughout controllers.

Authorization rules should be centralized and reusable.

---

## 5.3 User Management

Administrators must be able to:

- View users
- Search users
- Activate/deactivate users
- Assign roles
- Remove roles
- View user activity where appropriate

Customers must be able to:

- View their profile
- Update their profile
- Change their password

---

## 5.4 Product Catalog

The system must support:

- Product creation
- Product updates
- Product deletion
- Product retrieval
- Product listing
- Product categories
- Product variants
- Product pricing
- Product availability
- Product search
- Product filtering
- Product sorting
- Pagination

The API should support efficient querying for large product collections.

---

## 5.5 Category Management

The system must support:

- Create category
- Update category
- Delete category
- View category
- List categories

Categories may support hierarchical relationships where appropriate.

---

## 5.6 Inventory Management

The inventory system must support:

- Product stock tracking
- Stock reservation
- Stock release
- Stock increase
- Stock decrease
- Low-stock detection

Inventory operations must protect against race conditions and overselling.

The system should use concurrency control where appropriate.

---

## 5.7 Shopping Cart

Customers must be able to:

- Create a cart
- View their cart
- Add products
- Remove products
- Update quantities
- Clear the cart

Redis should be used for cart storage or caching where appropriate.

Cart operations must validate:

- Product existence
- Product availability
- Quantity limits

---

## 5.8 Order Management

Customers must be able to:

- Create orders
- View their orders
- View order details
- View order status

Administrators and authorized staff must be able to:

- View all orders
- Update order status
- Process orders
- Cancel orders where permitted

Orders must maintain a clear lifecycle.

Example:

```text
Pending
   |
   v
Payment Processing
   |
   +----> Payment Failed
   |             |
   |             v
   |         Cancelled
   |
   v
Paid
   |
   v
Inventory Reserved
   |
   v
Processing
   |
   v
Shipped
   |
   v
Delivered
```

The system must enforce valid state transitions.

---

## 5.9 Payment Workflow

The initial implementation will use a payment abstraction rather than integrating directly with a real payment provider.

The system must support:

- Payment initiation
- Payment success
- Payment failure
- Payment status tracking
- Payment idempotency

A mock payment provider may be used for development and testing.

The architecture must allow a real payment provider to be introduced later without changing core business logic.

---

## 5.10 Event-Driven Processing

The system must publish and consume integration events for important business workflows.

Initial events may include:

- `OrderCreated`
- `PaymentRequested`
- `PaymentSucceeded`
- `PaymentFailed`
- `InventoryReservationRequested`
- `InventoryReserved`
- `InventoryReservationFailed`
- `OrderCancelled`
- `OrderShipped`
- `OrderDelivered`

Events will be transported through RabbitMQ.

Event consumers must be designed to handle:

- Duplicate events
- Failed processing
- Retries
- Temporary infrastructure failures
- Poison messages

---

## 5.11 Notification System

The system should support asynchronous notifications for events such as:

- Order confirmation
- Payment confirmation
- Payment failure
- Order shipment
- Order delivery
- Order cancellation

The initial implementation may use a mock or console-based notification provider.

The notification provider must be abstracted so that email or other providers can be added later.

---

# 6. Reliability Requirements

The system must be designed with failure handling as a first-class concern.

## 6.1 Retry

Transient failures must support controlled retries.

Retry behavior should use:

- Exponential backoff
- Jitter
- Maximum retry attempts

The system must avoid immediate infinite retry loops.

---

## 6.2 Idempotency

Operations that may be delivered more than once must be idempotent.

For example:

```text
OrderCreated Event
        |
        v
Consumer receives event
        |
        v
Process successfully
        |
        v
Event delivered again
        |
        v
Consumer detects duplicate
        |
        v
No duplicate side effect
```

Idempotency must be considered for:

- Event consumers
- Payment operations
- Inventory reservations
- Order creation workflows

---

## 6.3 Dead-Letter Handling

Messages that cannot be successfully processed after configured retries must be moved to a dead-letter queue or equivalent dead-letter storage.

The system should preserve enough information to investigate failures.

Dead-letter information should include where practical:

- Event ID
- Event type
- Original message
- Failure reason
- Retry count
- Timestamp
- Correlation ID

---

## 6.4 Graceful Shutdown

The application must attempt to shut down gracefully.

Background consumers should:

- Stop accepting new work
- Complete or safely abandon in-flight work
- Acknowledge messages only after successful processing
- Respect cancellation tokens

---

# 7. Performance Requirements

The system should:

- Use asynchronous I/O
- Avoid unnecessary database queries
- Support pagination
- Use appropriate database indexes
- Use caching where beneficial
- Avoid blocking calls
- Avoid unnecessary allocations in performance-sensitive paths

Performance optimization should be evidence-driven through benchmarks or profiling rather than premature optimization.

---

# 8. Observability Requirements

The system must provide:

## Logging

Use structured logging.

Logs should include relevant contextual information such as:

- Correlation ID
- Request ID
- User ID where appropriate
- Order ID
- Event ID
- Event type

Sensitive information must never be logged.

## Health Checks

Health checks should cover important dependencies such as:

- Database
- Redis
- RabbitMQ

## Metrics

The system should expose metrics for:

- HTTP requests
- Request duration
- Error rates
- Orders created
- Events published
- Events consumed
- Failed event processing
- Retry counts
- Dead-lettered messages

## Distributed Tracing

OpenTelemetry should be used where appropriate to trace operations across:

```text
HTTP Request
    |
    v
Application
    |
    v
Database
    |
    v
RabbitMQ
    |
    v
Consumer
```

---

# 9. Security Requirements

The system must:

- Hash passwords securely
- Protect authentication endpoints
- Validate input
- Validate authorization
- Prevent unauthorized resource access
- Avoid exposing internal exceptions
- Avoid exposing sensitive configuration
- Store secrets outside source control
- Use environment variables or secure secret management
- Validate JWT tokens correctly
- Apply appropriate rate limiting where required

The API must use a centralized exception-handling mechanism.

Internal stack traces must not be returned to clients in production environments.

---

# 10. API Requirements

The API should follow RESTful principles where appropriate.

The API must provide:

- Consistent HTTP status codes
- Consistent response formats
- Validation error responses
- Pagination metadata
- Centralized exception handling
- OpenAPI documentation

Example response format:

```json
{
  "success": true,
  "data": {},
  "message": null,
  "errors": []
}
```

Error responses should provide useful client-facing information without exposing internal implementation details.

---

# 11. Technology Requirements

The initial technology stack will include:

### Backend

- C#
- .NET 10
- ASP.NET Core Web API

### Data

- PostgreSQL
- Entity Framework Core

### Caching

- Redis

### Messaging

- RabbitMQ

### Architecture

- Clean Architecture
- Modular Design
- CQRS
- Event-Driven Architecture

### Authentication

- JWT
- Refresh Tokens
- RBAC
- Permission-Based Authorization

### Resilience

- Polly / Microsoft resilience APIs where appropriate

### Logging

- Microsoft.Extensions.Logging
- Serilog where beneficial

### Observability

- OpenTelemetry

### Testing

- xUnit
- FluentAssertions
- Integration testing
- Testcontainers where appropriate

### Containerization

- Docker
- Docker Compose

### CI/CD

- GitHub Actions

---

# 12. Non-Functional Requirements

The system should be:

### Maintainable

Code must be organized according to clear architectural boundaries.

### Testable

Business logic should be testable without requiring external infrastructure whenever possible.

### Scalable

The architecture should allow individual modules to evolve independently.

### Observable

Failures and important business events should be visible through logs, metrics, and traces.

### Resilient

Temporary failures should not unnecessarily cause cascading failures.

### Secure

Security must be considered at every layer.

### Documented

The project must include clear documentation explaining:

- Architecture
- Setup
- Configuration
- Running locally
- Running tests
- API usage
- Event flows
- Architectural decisions

---

# 13. Out of Scope

The following features are intentionally excluded from the initial scope:

- Real payment processing
- Real shipping provider integration
- Real email delivery
- Frontend application
- Mobile application
- Recommendation engine
- AI-powered product recommendations
- Multi-vendor marketplace functionality
- International tax calculation
- Complex warehouse management
- Real-time customer support
- Full production cloud deployment

These may be added later if they provide meaningful architectural value.

---

# 14. Definition of Done

The project will be considered complete when:

- Authentication is implemented.
- Authorization is implemented.
- Product catalog is functional.
- Categories are functional.
- Inventory management is functional.
- Shopping cart is functional.
- Order management is functional.
- Payment workflow abstraction is implemented.
- RabbitMQ event-driven workflows are functional.
- Background consumers are implemented.
- Retry policies are implemented.
- Idempotency is implemented.
- Dead-letter handling is implemented.
- Redis is integrated where appropriate.
- PostgreSQL and EF Core are integrated.
- Structured logging is implemented.
- Health checks are implemented.
- Metrics are implemented.
- OpenTelemetry tracing is implemented.
- Unit tests are implemented.
- Integration tests are implemented.
- Docker setup is available.
- CI/CD pipeline is configured.
- API documentation is available.
- Architecture documentation is complete.
- README documentation is complete.

---

# 15. Portfolio Success Criteria

The project should demonstrate that the developer can:

1. Design a modern .NET backend architecture.
2. Build APIs using ASP.NET Core.
3. Apply Clean Architecture principles.
4. Implement secure authentication and authorization.
5. Work with relational databases using EF Core.
6. Use Redis for caching and high-speed data access.
7. Implement asynchronous messaging with RabbitMQ.
8. Build reliable event-driven workflows.
9. Handle retries and transient failures.
10. Design idempotent operations.
11. Implement dead-letter handling.
12. Build background workers.
13. Implement observability.
14. Write unit and integration tests.
15. Containerize applications with Docker.
16. Build CI/CD pipelines.
17. Explain architectural trade-offs during technical interviews.

---

# 16. Project Philosophy

This project is not intended to be the fastest way to build an e-commerce API.

The primary objective is to demonstrate strong backend engineering fundamentals and practical knowledge of modern .NET architecture.

Every major technology should have a clear reason for being included.

The project should prioritize:

```text
Correctness
    >
Maintainability
    >
Reliability
    >
Observability
    >
Performance
    >
Complexity
```

Complexity should only be introduced when it provides meaningful architectural or learning value.

The system should start as a well-structured modular backend and evolve incrementally toward more advanced event-driven capabilities.

The final implementation should be something the developer can confidently explain, defend, test, and maintain in a professional software engineering environment.
