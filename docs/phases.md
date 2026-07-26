# Project Phases

# Event-Driven E-Commerce Backend

**Project:** Event-Driven E-Commerce Backend
**Target Framework:** .NET 10
**Language:** C#
**Architecture:** Clean Architecture + Modular Monolith + Event-Driven Architecture
**Status:** In Progress
**Current Phase:** Phase 3 — Authentication & Authorization
**Version:** 1.0

---

# 1. Purpose

This document defines the implementation roadmap for the Event-Driven E-Commerce Backend.

The project will be developed incrementally.

Each phase must:

- Have a clearly defined objective.
- Build on previous phases.
- Produce working code.
- Include appropriate tests.
- Have a clear Definition of Done.

The AI or developer must not skip phases unless explicitly approved.

---

# 2. Development Strategy

The project will follow this progression:

```text
Phase 0
Project Foundation
        │
        ▼
Phase 1
Domain Foundation
        │
        ▼
Phase 2
Database & Persistence
        │
        ▼
Phase 3
Authentication & Authorization
        │
        ▼
Phase 4
Product Catalog
        │
        ▼
Phase 5
Inventory Management
        │
        ▼
Phase 6
Shopping Cart
        │
        ▼
Phase 7
Order Management
        │
        ▼
Phase 8
Event-Driven Messaging
        │
        ▼
Phase 9
Payment Workflow
        │
        ▼
Phase 10
Reliable Event Processing
        │
        ▼
Phase 11
Notifications
        │
        ▼
Phase 12
Observability
        │
        ▼
Phase 13
Testing & Quality
        │
        ▼
Phase 14
Docker & Local Infrastructure
        │
        ▼
Phase 15
CI/CD
        │
        ▼
Phase 16
Production Hardening
        │
        ▼
Phase 17
Final Documentation
```

---

# 3. Phase Rules

Before starting a phase:

1. Read `PRD.md`.
2. Read `Architecture.md`.
3. Read `rules.md`.
4. Read the current `memory.md`.
5. Confirm the current phase.
6. Review dependencies from previous phases.

During a phase:

- Only implement tasks belonging to the active phase.
- Keep changes focused.
- Write tests alongside functionality.
- Update documentation when architectural decisions change.
- Keep `memory.md` updated.

After completing a phase:

1. Run the build.
2. Run all relevant tests.
3. Verify architecture boundaries.
4. Review the Definition of Done.
5. Update `memory.md`.
6. Mark the phase complete.
7. Move to the next phase.

---

# 4. Phase 0 — Project Foundation

## Objective

Create the initial .NET solution and establish the project's architectural foundation.

## Tasks

- Install or verify .NET 10 SDK.
- Create solution.
- Create API project.
- Create Domain project.
- Create Application project.
- Create Infrastructure project.
- Create Contracts project.
- Create Unit Tests project.
- Create Integration Tests project.
- Create Architecture Tests project.
- Configure project references.
- Configure nullable reference types.
- Configure implicit usings.
- Configure `.editorconfig`.
- Configure centralized package management.
- Configure `Directory.Build.props`.
- Configure solution build.
- Configure initial dependency injection structure.
- Add initial health endpoint.
- Add basic API configuration.

## Expected Structure

```text
src/
├── ECommerce.Api
├── ECommerce.Application
├── ECommerce.Domain
├── ECommerce.Infrastructure
└── ECommerce.Contracts

tests/
├── ECommerce.UnitTests
├── ECommerce.IntegrationTests
└── ECommerce.ArchitectureTests
```

## Definition of Done

- Solution builds successfully.
- All projects compile.
- Project references follow architecture rules.
- API starts successfully.
- Health endpoint works.
- Test projects execute successfully.
- No architectural dependency violations exist.

---

# 5. Phase 1 — Domain Foundation

## Objective

Create the core domain primitives and establish domain modeling conventions.

## Tasks

Implement:

- Base Entity.
- Aggregate Root.
- Domain Event abstraction.
- Domain event collection.
- Result pattern if required.
- Domain exception conventions.
- Strongly typed IDs where justified.
- Basic value object conventions.

Establish:

```text
Entity
    │
    ▼
AggregateRoot
    │
    ▼
Domain Events
```

## Definition of Done

- Domain project has no infrastructure dependencies.
- Base domain abstractions exist.
- Domain event mechanism works.
- Unit tests cover core domain primitives.
- Architecture tests verify domain independence.

---

# 6. Phase 2 — Database & Persistence

## Objective

Establish PostgreSQL and Entity Framework Core persistence.

## Tasks

- Add EF Core.
- Configure PostgreSQL.
- Create DbContext.
- Configure database connection.
- Create entity configurations.
- Configure migrations.
- Create initial database migration.
- Configure database dependency injection.
- Add development database setup.
- Add database health check.

## Initial Data Model

At this stage, only create foundational entities required for the current implementation.

Do not create the entire final database schema prematurely.

Potential initial entities:

```text
User
Role
Permission
Product
Category
```

Additional entities will be introduced in their respective phases.

## Definition of Done

- PostgreSQL connection works.
- EF Core DbContext works.
- Migration can be created.
- Database can be created from migrations.
- Health check reports database status.
- Integration tests can connect to PostgreSQL.
- No domain-to-EF Core dependency exists.

---

# 7. Phase 3 — Authentication & Authorization

## Objective

Implement secure identity, authentication, and permission-based authorization.

## Tasks

Implement:

- User registration.
- Password hashing.
- Login.
- JWT access tokens.
- Refresh tokens.
- Refresh token rotation.
- Refresh token revocation.
- Logout.
- User activation/deactivation.
- Role management.
- Permission management.
- Role-permission relationships.
- User-role relationships.
- Authorization policies.

## Authorization Model

```text
User
 │
 └── Roles
       │
       └── Permissions
```

Example:

```text
Admin
├── User.Read
├── User.Update
├── Product.Create
├── Product.Update
├── Product.Delete
└── Order.Read
```

## Definition of Done

- User can register.
- User can log in.
- JWT authentication works.
- Refresh tokens work.
- Refresh tokens can be revoked.
- Protected endpoints reject unauthenticated requests.
- Permission-based authorization works.
- Role assignment works.
- Security-sensitive data is never exposed.
- Authentication tests pass.

---

# 8. Phase 4 — Product Catalog

## Objective

Implement product and category management.

## Tasks

Implement:

- Product entity.
- Category entity.
- Product variants.
- Product SKU.
- Product pricing.
- Product status.
- Product creation.
- Product update.
- Product deletion.
- Product retrieval.
- Product listing.
- Pagination.
- Sorting.
- Filtering.
- Search.
- Category management.

## Query Requirements

Product queries should support:

```text
Search
Filter
Sort
Pagination
```

Example:

```text
GET /api/products
    ?search=phone
    &category=electronics
    &page=1
    &pageSize=20
    &sort=price
```

## Definition of Done

- Products can be created.
- Products can be updated.
- Products can be retrieved.
- Products can be listed.
- Pagination works.
- Filtering works.
- Search works.
- Categories work.
- Authorization rules are enforced.
- Product tests pass.
- API integration tests pass.

---

# 9. Phase 5 — Inventory Management

## Objective

Build inventory management and stock reservation capabilities.

## Tasks

Implement:

- Inventory item.
- Stock quantity.
- Available quantity.
- Reserved quantity.
- Stock adjustment.
- Stock reservation.
- Stock release.
- Low-stock detection.
- Concurrency protection.

## Important Business Rule

The system must prevent overselling.

Example:

```text
Available Stock = 10

Customer A requests 7
        │
        ▼
Reserve 7
        │
        ▼
Available = 3

Customer B requests 5
        │
        ▼
Reservation rejected
```

Concurrency must be handled correctly.

## Definition of Done

- Stock can be added.
- Stock can be adjusted.
- Stock can be reserved.
- Stock can be released.
- Overselling is prevented.
- Concurrent reservation tests pass.
- Inventory business rules are tested.

---

# 10. Phase 6 — Shopping Cart

## Objective

Implement customer shopping carts using Redis where appropriate.

## Tasks

Implement:

- Create cart.
- Get cart.
- Add item.
- Update quantity.
- Remove item.
- Clear cart.
- Validate product availability.
- Validate quantity.

Redis should be used for cart storage.

## Definition of Done

- Customer can create a cart.
- Customer can view cart.
- Customer can add products.
- Customer can update quantities.
- Customer can remove products.
- Customer can clear cart.
- Unauthorized users cannot access another customer's cart.
- Redis integration works.
- Cart tests pass.

---

# 11. Phase 7 — Order Management

## Objective

Implement the core order lifecycle.

## Tasks

Implement:

- Order entity.
- Order item.
- Order status.
- Order creation.
- Order retrieval.
- Order history.
- Order cancellation.
- Order status transitions.

Initial order states:

```text
Pending
    │
    ▼
PaymentProcessing
    │
    ├── PaymentFailed ──> Cancelled
    │
    ▼
Paid
    │
    ▼
Processing
    │
    ▼
Shipped
    │
    ▼
Delivered
```

The system must enforce valid transitions.

## Definition of Done

- Customer can create order.
- Customer can view own orders.
- Customer can view order details.
- Order lifecycle works.
- Invalid state transitions are rejected.
- Order business rules are tested.
- Authorization is enforced.

---

# 12. Phase 8 — Event-Driven Messaging Foundation

## Objective

Introduce RabbitMQ and the event-driven infrastructure.

## Tasks

- Add RabbitMQ.
- Configure RabbitMQ connection.
- Create event publisher abstraction.
- Create integration event contracts.
- Configure exchanges.
- Configure queues.
- Configure routing keys.
- Implement message serialization.
- Implement message deserialization.
- Implement consumer infrastructure.
- Implement basic acknowledgment.

Initial event:

```text
OrderCreated
```

Initial flow:

```text
Order
  │
  ▼
OrderCreated
  │
  ▼
RabbitMQ
  │
  ▼
Consumer
```

## Definition of Done

- RabbitMQ connection works.
- Events can be published.
- Events can be consumed.
- Messages are acknowledged after successful processing.
- Failed messages are not incorrectly acknowledged.
- Integration event contracts are defined.
- Messaging integration tests pass.

---

# 13. Phase 9 — Payment Workflow

## Objective

Implement an abstract payment workflow.

## Tasks

Implement:

- Payment entity.
- Payment status.
- Payment provider abstraction.
- Mock payment provider.
- Payment initiation.
- Payment success.
- Payment failure.
- Payment idempotency.

Initial workflow:

```text
OrderCreated
      │
      ▼
PaymentRequested
      │
      ▼
Payment Consumer
      │
      ▼
Mock Payment Provider
      │
      ├── Success
      │      │
      │      ▼
      │  PaymentSucceeded
      │
      └── Failure
             │
             ▼
        PaymentFailed
```

## Definition of Done

- Payment workflow works.
- Payment status is tracked.
- Payment provider is abstracted.
- Payment failures are handled.
- Duplicate payment requests do not create duplicate payments.
- Payment tests pass.

---

# 14. Phase 10 — Reliable Event Processing

## Objective

Make the event-driven system resilient and production-oriented.

## Tasks

Implement:

- Retry policies.
- Exponential backoff.
- Jitter.
- Retry limits.
- Dead-letter queues.
- Dead-letter storage.
- Idempotency.
- Correlation IDs.
- Event metadata.
- Failure logging.

Consider implementing:

- Outbox Pattern.
- Inbox Pattern.

## Event Processing Flow

```text
Event
  │
  ▼
Consumer
  │
  ├── Success ──────> ACK
  │
  └── Failure
         │
         ▼
      Retry
         │
         ▼
   Maximum Attempts?
      │
      ├── No ──> Retry
      │
      └── Yes
             │
             ▼
       Dead Letter
```

## Definition of Done

- Retry policies work.
- Exponential backoff works.
- Jitter is implemented.
- Retry count is limited.
- Duplicate events are handled safely.
- Failed messages reach dead-letter handling.
- Dead-letter information is persisted.
- Correlation IDs flow through event processing.
- Event reliability tests pass.

---

# 15. Phase 11 — Notification System

## Objective

Implement asynchronous notifications.

## Tasks

Implement:

- Notification abstraction.
- Mock notification provider.
- Notification consumer.
- Order confirmation notification.
- Payment notification.
- Shipment notification.
- Delivery notification.
- Failure notification.

Example:

```text
OrderCreated
      │
      ▼
Notification Consumer
      │
      ▼
Notification Provider
      │
      ▼
Customer Notification
```

## Definition of Done

- Notifications are asynchronous.
- Notification provider is abstracted.
- Notification failures are handled.
- Notifications do not block core order processing.
- Notification tests pass.

---

# 16. Phase 12 — Observability

## Objective

Implement production-grade observability.

## Tasks

Implement:

- Structured logging.
- Correlation IDs.
- Health checks.
- Metrics.
- OpenTelemetry.
- Distributed tracing.

Track:

```text
HTTP Requests
Request Duration
HTTP Errors
Orders Created
Events Published
Events Consumed
Event Failures
Retry Attempts
Dead-Letter Messages
Payment Failures
Inventory Failures
```

## Definition of Done

- Logs are structured.
- Correlation IDs are available.
- Health checks work.
- Metrics are emitted.
- Distributed traces are available.
- Sensitive information is not logged.

---

# 17. Phase 13 — Testing & Quality

## Objective

Increase system confidence through comprehensive testing.

## Tasks

Implement and review:

### Unit Tests

Test:

- Domain entities.
- Value objects.
- Business rules.
- Command handlers.
- Query handlers.
- Validators.

### Integration Tests

Test:

- PostgreSQL.
- Redis.
- RabbitMQ.
- Authentication.
- Order workflows.
- Event consumers.

### Architecture Tests

Verify:

- Domain independence.
- Application boundaries.
- Infrastructure boundaries.
- API boundaries.

### End-to-End Tests

Test critical workflows:

```text
Register
   │
   ▼
Login
   │
   ▼
Browse Product
   │
   ▼
Add to Cart
   │
   ▼
Create Order
   │
   ▼
Payment
   │
   ▼
Inventory
   │
   ▼
Notification
```

## Definition of Done

- Critical business logic has unit tests.
- Critical workflows have integration tests.
- Architecture tests pass.
- Tests are deterministic.
- CI can execute the test suite.

---

# 18. Phase 14 — Docker & Local Infrastructure

## Objective

Make the entire development environment reproducible.

## Tasks

Create Docker configuration for:

- PostgreSQL.
- Redis.
- RabbitMQ.
- API.

Create:

```text
docker-compose.yml
```

Configure:

- Environment variables.
- Health checks.
- Service dependencies.
- Persistent volumes where appropriate.

Expected startup:

```text
docker compose up
```

should start the local infrastructure.

## Definition of Done

- PostgreSQL starts through Docker.
- Redis starts through Docker.
- RabbitMQ starts through Docker.
- API can connect to all dependencies.
- Health checks work.
- Local setup is documented.

---

# 19. Phase 15 — CI/CD

## Objective

Automate build, testing, and quality checks.

## Tasks

Create GitHub Actions workflows for:

- Build.
- Restore.
- Format check.
- Unit tests.
- Integration tests.
- Architecture tests.
- Docker build.
- Security checks.

Pipeline:

```text
Pull Request
      │
      ▼
Build
      │
      ▼
Format
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
```

Future deployment may be added later.

## Definition of Done

- CI runs automatically.
- Pull requests are validated.
- Build failures stop the pipeline.
- Test failures stop the pipeline.
- Docker image builds successfully.

---

# 20. Phase 16 — Production Hardening

## Objective

Review the system as if it were preparing for production.

## Tasks

Perform:

- Security review.
- Dependency review.
- Performance review.
- Database index review.
- API rate-limit review.
- Retry policy review.
- Logging review.
- Error handling review.
- Authentication review.
- Authorization review.
- Configuration review.

Test failure scenarios:

```text
PostgreSQL unavailable
Redis unavailable
RabbitMQ unavailable
Payment provider unavailable
Consumer crashes
Duplicate event
Message processing timeout
Database transaction failure
```

## Definition of Done

The system must:

- Fail gracefully.
- Recover from transient failures.
- Avoid data corruption.
- Avoid duplicate side effects.
- Provide useful logs.
- Expose health status.
- Handle infrastructure outages predictably.

---

# 21. Phase 17 — Final Documentation

## Objective

Prepare the project for GitHub, CV, LinkedIn, and technical interviews.

## Tasks

Complete:

- README.md.
- Architecture documentation.
- Setup documentation.
- API documentation.
- Event flow documentation.
- Database documentation.
- Docker instructions.
- Testing instructions.
- CI/CD documentation.
- Architecture Decision Records where useful.

Create architecture diagrams for:

```text
System Architecture
Order Workflow
Event Flow
Authentication Flow
Database Architecture
RabbitMQ Topology
Deployment Architecture
```

## Definition of Done

A new developer should be able to:

1. Clone the repository.
2. Start infrastructure.
3. Configure the application.
4. Run the API.
5. Run tests.
6. Understand the architecture.
7. Understand the event flows.
8. Understand how to contribute.

---

# 22. Phase Completion Checklist

Before marking any phase complete:

```text
[ ] Feature implemented
[ ] Code builds
[ ] Relevant tests written
[ ] Tests pass
[ ] Architecture rules respected
[ ] Security reviewed
[ ] Logging considered
[ ] Error handling implemented
[ ] Cancellation considered
[ ] Documentation updated
[ ] memory.md updated
```

---

# 23. Phase Status Tracking

The current status should always be maintained in `memory.md`.

Example:

```text
Phase 0 — Project Foundation       [COMPLETE]
Phase 1 — Domain Foundation        [COMPLETE]
Phase 2 — Database & Persistence    [COMPLETE]
Phase 3 — Authentication            [IN PROGRESS]
Phase 4 — Product Catalog           [NOT STARTED]
Phase 5 — Inventory                 [NOT STARTED]
Phase 6 — Shopping Cart             [NOT STARTED]
Phase 7 — Order Management          [NOT STARTED]
Phase 8 — Event Messaging            [NOT STARTED]
Phase 9 — Payment Workflow           [NOT STARTED]
Phase 10 — Reliable Events           [NOT STARTED]
Phase 11 — Notifications             [NOT STARTED]
Phase 12 — Observability             [NOT STARTED]
Phase 13 — Testing & Quality         [NOT STARTED]
Phase 14 — Docker                    [NOT STARTED]
Phase 15 — CI/CD                     [NOT STARTED]
Phase 16 — Production Hardening      [NOT STARTED]
Phase 17 — Documentation             [NOT STARTED]
```

Allowed statuses:

```text
NOT STARTED
IN PROGRESS
BLOCKED
COMPLETE
```

---

# 24. Phase Execution Philosophy

The project should evolve through working increments.

At no point should we spend a large amount of time building infrastructure that is not yet needed.

For example:

```text
Do not build RabbitMQ
        │
        ▼
before the application
has a meaningful event to publish.
```

Instead:

```text
Build Order
    │
    ▼
Create OrderCreated Event
    │
    ▼
Introduce RabbitMQ
    │
    ▼
Consume Event
    │
    ▼
Add Reliability
```

Each technology should be introduced when its purpose becomes clear.

---

# 25. Recommended Implementation Order

The recommended order is:

```text
1. Solution Foundation
2. Domain Primitives
3. Database
4. Authentication
5. Authorization
6. Product Catalog
7. Inventory
8. Cart
9. Orders
10. RabbitMQ
11. Payment
12. Reliable Messaging
13. Notifications
14. Observability
15. Testing
16. Docker
17. CI/CD
18. Production Hardening
19. Documentation
```

This order may be adjusted if an architectural dependency requires it.

Any major deviation should be recorded in `memory.md`.

---

# 26. Final Project Milestone

The project is considered complete when the system can demonstrate this end-to-end workflow:

```text
Customer
    │
    ▼
Register
    │
    ▼
Login
    │
    ▼
Browse Products
    │
    ▼
Add Product to Cart
    │
    ▼
Create Order
    │
    ▼
Order Persisted
    │
    ▼
OrderCreated Event
    │
    ▼
RabbitMQ
    │
    ├─────────────────┐
    │                 │
    ▼                 ▼
Payment            Inventory
Consumer           Consumer
    │                 │
    ▼                 ▼
Payment            Reserve
Processing         Stock
    │                 │
    └────────┬────────┘
             ▼
       Order Updated
             │
             ▼
      Notification Event
             │
             ▼
      Notification Worker
             │
             ▼
          Customer
```

The complete workflow must demonstrate:

- Authentication.
- Authorization.
- Product catalog.
- Shopping cart.
- Order processing.
- Payment workflow.
- Inventory reservation.
- RabbitMQ messaging.
- Retry handling.
- Idempotency.
- Dead-letter handling.
- Observability.
- Automated testing.

The final project should be a coherent system rather than a collection of disconnected features.
