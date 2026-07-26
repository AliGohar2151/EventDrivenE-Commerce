# Development Rules

# Event-Driven E-Commerce Backend

**Project:** Event-Driven E-Commerce Backend
**Language:** C#
**Framework:** .NET 10
**Architecture:** Clean Architecture + Modular Monolith + Event-Driven Architecture
**Status:** Active Rules
**Version:** 1.0

---

# 1. Purpose

This document defines the mandatory development rules for the Event-Driven E-Commerce Backend.

These rules apply to:

- Human developers
- AI coding agents
- Code generation
- Refactoring
- Bug fixes
- New features
- Architectural changes
- Tests
- Documentation

The purpose of these rules is to maintain:

- Architectural integrity
- Code quality
- Consistency
- Security
- Testability
- Reliability
- Maintainability
- Observability

No implementation should violate these rules without an explicit architectural decision and documentation update.

---

# 2. Core Development Philosophy

The project must follow these priorities:

```text
Correctness
    >
Security
    >
Maintainability
    >
Reliability
    >
Observability
    >
Performance
    >
Convenience
```

Code must be written for long-term maintainability.

Do not optimize for:

- Minimum lines of code
- Fastest initial implementation
- Unnecessary abstraction
- Clever code
- Framework-specific shortcuts

Prefer code that is:

- Explicit
- Readable
- Testable
- Predictable
- Easy to debug

---

# 3. AI Agent Rules

AI agents working on this project must follow these rules.

## 3.1 Read Project Context First

Before modifying code, the AI must read:

1. `PRD.md`
2. `Architecture.md`
3. `rules.md`
4. `phases.md`
5. `design.md`
6. `memory.md`

The AI must understand the current project phase before writing code.

---

## 3.2 Respect Current Phase

The AI must only implement work that belongs to the active phase unless explicitly instructed otherwise.

Do not:

- Jump ahead to future features
- Implement unrelated modules
- Add unnecessary infrastructure
- Refactor unrelated code
- Introduce future architecture prematurely

If a requested change conflicts with the current phase, the conflict must be identified before implementation.

---

## 3.3 Inspect Existing Code First

Before creating a new file or modifying an existing file, inspect relevant existing code.

Do not assume:

- A class does not exist
- An interface does not exist
- A dependency is missing
- A service needs to be created
- A pattern has not already been implemented

Avoid duplicate implementations.

---

## 3.4 Minimal Scope Changes

When implementing a feature, modify only the files necessary for that feature.

Avoid unrelated refactoring.

Do not change working code simply because a different style is preferred.

---

## 3.5 Update Memory

After completing meaningful work, `memory.md` must be updated.

The update should record:

- What was completed
- What files were changed
- Important architectural decisions
- Current phase
- Current task
- Known issues
- Next recommended task

---

# 4. Architecture Rules

## 4.1 Dependency Direction

Dependencies must point inward.

Allowed:

```text
API
  ↓
Application
  ↓
Domain

Infrastructure
  ↓
Application
  ↓
Domain
```

The Domain must never depend on:

- API
- Infrastructure
- EF Core
- PostgreSQL
- Redis
- RabbitMQ
- ASP.NET Core

---

## 4.2 Domain Independence

The Domain project must remain framework-independent wherever practical.

Do not place:

- Database logic
- HTTP logic
- RabbitMQ logic
- Redis logic
- JWT implementation
- External API calls

inside Domain entities.

---

## 4.3 API Must Stay Thin

Controllers must not contain business logic.

Controllers should only:

1. Receive HTTP requests.
2. Bind input.
3. Dispatch commands or queries.
4. Return HTTP responses.

Avoid:

```csharp
public async Task<IActionResult> CreateOrder(...)
{
    // Business logic
    // Database operations
    // Payment logic
    // Inventory logic
}
```

Prefer:

```csharp
public async Task<IActionResult> CreateOrder(
    CreateOrderCommand command,
    CancellationToken cancellationToken)
{
    var result = await sender.Send(command, cancellationToken);

    return Ok(result);
}
```

---

## 4.4 Business Logic Location

Business rules must be placed in the correct layer.

Use:

- Domain entities for entity-specific business rules.
- Value objects for domain concepts with validation and invariants.
- Domain services for domain logic that does not naturally belong to one entity.
- Application handlers for application orchestration.
- Infrastructure for external system integration.

Do not place business rules in controllers.

---

# 5. Domain Rules

## 5.1 Entities

Entities must have meaningful identity.

Avoid creating entities that are simply data containers with no domain behavior when meaningful behavior belongs there.

---

## 5.2 Aggregate Boundaries

Aggregate boundaries must be explicit.

Do not load large object graphs unnecessarily.

Modify aggregates through their root where appropriate.

---

## 5.3 Value Objects

Use value objects where they improve correctness and expressiveness.

Potential examples:

```text
Money
EmailAddress
Address
ProductSku
OrderId
UserId
```

Do not introduce value objects purely for abstraction.

---

## 5.4 Domain Events

Domain events represent important business facts.

Examples:

```text
OrderCreated
OrderCancelled
PaymentCompleted
InventoryReserved
```

Domain events must not contain infrastructure logic.

Domain events should be separate from integration events.

---

# 6. CQRS Rules

The project uses CQRS for application use cases.

## 6.1 Commands

Commands:

- Represent an intention to change state.
- May modify data.
- Must have a single clear responsibility.

Examples:

```text
CreateOrderCommand
CancelOrderCommand
ReserveInventoryCommand
```

---

## 6.2 Queries

Queries:

- Retrieve information.
- Must not modify business state.
- Should be optimized for read operations.

Examples:

```text
GetProductQuery
GetOrderQuery
GetOrdersQuery
```

---

## 6.3 Command and Query Separation

Do not create methods that both modify state and return unrelated read models unless there is a clear reason.

Keep write and read responsibilities clear.

---

# 7. Database Rules

## 7.1 PostgreSQL Is the Source of Truth

PostgreSQL is the primary source of truth for transactional business data.

Redis must not replace PostgreSQL as the source of truth for:

- Orders
- Payments
- Inventory
- Users
- Products

unless explicitly documented as an architectural decision.

---

## 7.2 EF Core

EF Core must remain in Infrastructure.

Application and Domain layers must not directly depend on EF Core.

---

## 7.3 Migrations

Database migrations must be:

- Version-controlled
- Reproducible
- Reviewable

Never manually modify production databases without a documented migration strategy.

---

## 7.4 Query Performance

Avoid:

- N+1 queries
- Unnecessary eager loading
- Loading entire tables
- Unbounded queries

Use:

- Pagination
- Projection
- Appropriate indexes
- `AsNoTracking()` for read-only queries where appropriate

---

## 7.5 Transactions

Transactions must be used when multiple database operations must succeed or fail together.

Do not create unnecessarily large transactions.

Keep transaction boundaries explicit.

---

# 8. Repository Rules

Do not create generic repositories automatically.

Avoid:

```text
IGenericRepository<T>
```

unless there is a demonstrated architectural need.

Prefer:

- EF Core queries for simple use cases.
- Specific repositories for complex aggregate persistence.
- Application abstractions where Infrastructure details must be hidden.

The repository pattern must not become an unnecessary abstraction over EF Core.

---

# 9. Messaging Rules

RabbitMQ is the messaging infrastructure.

## 9.1 Events

Events must represent facts that have already happened.

Prefer:

```text
OrderCreated
PaymentSucceeded
InventoryReserved
```

Avoid event names that represent commands when defining events.

Commands represent intentions.

Events represent facts.

---

## 9.2 Event Contracts

Integration event contracts must be stable.

Do not expose internal domain entities directly as RabbitMQ messages.

Never serialize EF Core entities as integration events.

---

## 9.3 Event IDs

Every integration event must have a unique identifier.

Events should include relevant metadata such as:

```text
EventId
OccurredAt
CorrelationId
CausationId
```

Where appropriate.

---

## 9.4 Event Versioning

Integration events must be designed with future compatibility in mind.

Breaking changes to event contracts must not be introduced casually.

If a breaking change is necessary, consider event versioning.

---

# 10. RabbitMQ Consumer Rules

Consumers must be resilient.

Every consumer must:

- Support cancellation.
- Handle exceptions.
- Log failures.
- Avoid acknowledging failed messages.
- Support retry policies.
- Support idempotency.

A message must only be acknowledged after successful processing.

---

# 11. Retry Rules

Retries must never be infinite.

Retry policies must define:

- Maximum attempts
- Backoff strategy
- Jitter
- Retryable exceptions

Use exponential backoff.

Avoid:

```text
Retry immediately
Retry immediately
Retry immediately
```

Prefer:

```text
Attempt 1
    ↓
Delay
    ↓
Attempt 2
    ↓
Longer Delay
    ↓
Attempt 3
```

Permanent failures should not be retried indefinitely.

---

# 12. Idempotency Rules

All important event consumers must assume duplicate delivery is possible.

Consumers must be safe if the same event is received multiple times.

For example:

```text
OrderCreated
OrderCreated
OrderCreated
```

must not create three orders.

Idempotency must be implemented at the correct business boundary.

Do not rely solely on in-memory dictionaries for idempotency in a distributed or multi-instance environment.

---

# 13. Dead-Letter Rules

Messages that cannot be processed after maximum retries must be routed to dead-letter handling.

Dead-letter records should retain:

- Event ID
- Event type
- Original payload
- Exception
- Retry count
- Timestamp
- Correlation ID

Dead-letter handling must allow future inspection and potential replay.

---

# 14. Redis Rules

Redis is for:

- Caching
- Shopping carts
- Idempotency data where appropriate
- Short-lived distributed state

Do not blindly cache every database query.

Every cache must have a clear:

- Key strategy
- Expiration strategy
- Invalidation strategy

Cache invalidation must be explicitly considered whenever underlying data changes.

---

# 15. Authentication Rules

Passwords must never be stored in plain text.

Passwords must be securely hashed using an established password hashing algorithm.

Never:

- Log passwords
- Return passwords
- Store plaintext passwords
- Include passwords in events

JWT tokens must have appropriate expiration times.

Refresh tokens must support secure rotation and revocation.

---

# 16. Authorization Rules

Authorization must be enforced server-side.

Never trust:

- Client-provided roles
- Client-provided permissions
- Hidden UI elements
- Frontend restrictions

The backend must independently verify authorization.

Prefer policy-based authorization and centralized permission definitions.

---

# 17. Security Rules

Never commit:

- Passwords
- API keys
- JWT secrets
- Database credentials
- RabbitMQ credentials
- Redis credentials
- Private certificates

Use:

- Environment variables
- User Secrets for local development
- Secure CI/CD secrets
- Cloud secret managers when deployed

---

# 18. Validation Rules

Validate input at the application boundary.

Validation must include:

- Required fields
- Length limits
- Numeric ranges
- Valid formats
- Business constraints where appropriate

Never trust client input.

Validation must not replace domain invariants.

The domain must still protect itself from invalid state.

---

# 19. Error Handling Rules

All unexpected exceptions must be handled centrally.

Do not expose:

- Stack traces
- SQL exceptions
- Internal class names
- Database connection details
- Infrastructure implementation details

Production responses should be safe for clients.

Development environments may expose additional diagnostic information.

---

# 20. Result and Error Response Rules

API responses must be consistent.

Success:

```json
{
  "success": true,
  "data": {},
  "message": null,
  "errors": []
}
```

Validation failure:

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed.",
  "errors": [
    {
      "field": "email",
      "message": "A valid email address is required."
    }
  ]
}
```

Do not create multiple incompatible response formats without a documented reason.

---

# 21. Logging Rules

Use structured logging.

Prefer:

```csharp
logger.LogInformation(
    "Order {OrderId} created for user {UserId}",
    orderId,
    userId);
```

Avoid:

```csharp
logger.LogInformation(
    $"Order {orderId} created for user {userId}");
```

Never log:

- Passwords
- Access tokens
- Refresh tokens
- API keys
- Payment secrets
- Sensitive personal information unnecessarily

---

# 22. Observability Rules

Important operations must be observable.

Include relevant:

- Correlation IDs
- Trace IDs
- Event IDs
- Order IDs
- User IDs where appropriate

Logs, metrics, and traces should provide enough context to investigate production failures.

---

# 23. Cancellation Rules

Asynchronous operations must support cancellation where appropriate.

Use:

```csharp
CancellationToken
```

Do not ignore cancellation tokens in:

- HTTP requests
- Database operations
- Message consumers
- Background workers

Background workers must shut down gracefully.

---

# 24. Async Programming Rules

Use asynchronous APIs for I/O operations.

Prefer:

```csharp
await repository.GetAsync(cancellationToken);
```

Avoid:

```csharp
repository.GetAsync().Result;
```

Avoid:

```csharp
repository.GetAsync().GetAwaiter().GetResult();
```

Do not block asynchronous execution unnecessarily.

Avoid unnecessary `Task.Run()` for I/O-bound work.

---

# 25. Dependency Injection Rules

Use constructor injection.

Prefer:

```csharp
public class OrderService(
    IOrderRepository repository,
    IEventPublisher eventPublisher)
{
}
```

Avoid service locator patterns.

Avoid injecting `IServiceProvider` into business services unless there is a strong architectural justification.

Dependencies must be explicit.

---

# 26. Configuration Rules

Configuration must be strongly typed where practical.

Prefer options classes:

```text
RabbitMqOptions
RedisOptions
JwtOptions
DatabaseOptions
```

Avoid scattering configuration keys throughout the codebase.

---

# 27. Naming Rules

Use clear and descriptive names.

Classes:

```text
CreateOrderCommand
CreateOrderCommandHandler
GetOrderQuery
GetOrderQueryHandler
```

Avoid vague names:

```text
Manager
Helper
Processor
Utils
Service
```

unless the name accurately represents the responsibility.

---

# 28. File Organization Rules

Organize code primarily by feature in the Application layer.

Prefer:

```text
Features
└── Orders
    ├── Commands
    └── Queries
```

over large global folders such as:

```text
Commands
Queries
Services
DTOs
Validators
```

containing hundreds of unrelated files.

---

# 29. Code Quality Rules

Code must:

- Compile without warnings where practical.
- Pass automated tests.
- Follow formatting rules.
- Avoid dead code.
- Avoid commented-out code.
- Avoid unnecessary duplication.

Do not leave TODO comments for incomplete required functionality unless the task is explicitly deferred.

---

# 30. Nullable Reference Types

Nullable reference types must remain enabled.

Do not suppress nullable warnings blindly.

Avoid:

```csharp
SomeProperty!.DoSomething();
```

unless the nullability guarantee is genuinely established.

Prefer explicit validation or safe handling.

---

# 31. Exception Rules

Exceptions must represent exceptional situations.

Do not use exceptions for normal control flow.

Use domain-specific exceptions where they provide meaningful value.

Examples:

```text
OrderNotFoundException
InsufficientInventoryException
InvalidOrderStateException
```

Do not create an exception class for every possible error unnecessarily.

---

# 32. Testing Rules

New business functionality must include tests.

Tests should cover:

- Happy paths
- Validation failures
- Business rule failures
- Edge cases
- Failure scenarios

Critical workflows must have integration tests.

---

# 33. Test Independence

Tests must be deterministic.

Avoid tests that depend on:

- Execution order
- Current time without control
- External internet services
- Developer-specific machine configuration

Tests should clean up their own resources.

---

# 34. Integration Testing Rules

Integration tests should test real infrastructure where appropriate.

Use Testcontainers when practical for:

- PostgreSQL
- Redis
- RabbitMQ

Do not mock every external dependency in integration tests.

The purpose of integration tests is to verify that real components work together.

---

# 35. Architecture Testing Rules

Architecture tests should enforce dependency rules.

For example:

```text
Domain
  ❌ Cannot depend on Application
  ❌ Cannot depend on Infrastructure
  ❌ Cannot depend on API

Application
  ❌ Cannot depend on API
  ❌ Cannot depend on Infrastructure implementations

API
  ✔ Can depend on Application
  ✔ Can depend on Infrastructure registration
```

Architecture tests should prevent accidental dependency violations.

---

# 36. Performance Rules

Do not optimize without evidence.

Before optimizing:

1. Identify the bottleneck.
2. Measure it.
3. Make the change.
4. Measure again.

Avoid premature optimization.

However, obvious performance problems must not be introduced intentionally.

Examples to avoid:

- N+1 database queries
- Unbounded result sets
- Blocking I/O
- Infinite retries
- Excessive object allocation in hot paths

---

# 37. API Design Rules

API endpoints must:

- Use meaningful HTTP verbs.
- Return appropriate status codes.
- Validate input.
- Use consistent response structures.
- Support pagination for large collections.
- Avoid exposing internal database entities directly.

Do not return EF Core entities directly from controllers.

Use DTOs or dedicated response models.

---

# 38. DTO Rules

DTOs must represent API or application contracts.

Do not automatically expose every domain property.

DTOs should contain only the data required by the consumer.

Sensitive internal fields must never be exposed accidentally.

---

# 39. Database Entity Rules

Do not expose EF Core entities directly through API responses.

Database models and domain models should not be coupled to API contracts.

Changes to database persistence should not automatically become breaking API changes.

---

# 40. Eventual Consistency Rules

Event-driven workflows may be eventually consistent.

The API must not pretend that asynchronous operations completed synchronously when they have not.

For example:

```text
Order Created
Payment Processing
```

must not be represented as:

```text
Payment Completed
```

until payment processing actually succeeds.

API responses must accurately represent the current state.

---

# 41. Distributed Transaction Rules

Do not attempt to use a distributed database transaction across PostgreSQL, RabbitMQ, Redis, or external services.

Prefer:

- Local transactions
- Reliable event publishing strategies
- Idempotent consumers
- Compensation workflows
- Saga-style orchestration where necessary

The exact strategy must be chosen based on the workflow.

---

# 42. Outbox Pattern Rule

For critical business events, the project should consider the Outbox Pattern.

The following failure must be prevented:

```text
Database Transaction
      │
      ├── Order Saved ✅
      │
      └── Event Publish ❌
```

This could result in an order existing without its corresponding event being published.

Where required, use:

```text
Database Transaction
      │
      ├── Save Order
      │
      └── Save Outbox Event
              │
              ▼
          Commit
              │
              ▼
       Outbox Publisher
              │
              ▼
          RabbitMQ
```

The Outbox Pattern should be introduced when the order/event workflow requires reliable event delivery.

Do not implement it prematurely if it is not yet needed by the active project phase.

---

# 43. No Premature Microservices

The initial project must remain a modular monolith.

Do not create multiple deployable services simply to claim that the project uses microservices.

Microservice extraction is a future architectural capability.

The current objective is to demonstrate strong modular design and event-driven architecture first.

---

# 44. No Unnecessary Libraries

Before adding a NuGet package, ask:

1. Is this functionality genuinely required?
2. Can the .NET platform already provide it?
3. Does the package have strong maintenance and community support?
4. Does it introduce unnecessary complexity?
5. Does it align with the project architecture?

Avoid adding dependencies for trivial functionality.

---

# 45. Dependency Version Rules

Use centralized package management where practical.

Dependencies should be versioned consistently.

Avoid mixing incompatible major versions.

Before upgrading a major dependency:

- Review breaking changes.
- Run the full test suite.
- Verify compatibility.

---

# 46. Documentation Rules

Important architectural decisions must be documented.

Documentation should explain:

- Why a technology was chosen.
- Why a pattern was introduced.
- What trade-offs were considered.
- What alternatives were rejected.

Do not document only what the code does.

Document important reasons behind architectural decisions.

---

# 47. Git Rules

Commits should be focused and meaningful.

Prefer:

```text
feat: add product catalog
feat: implement JWT authentication
fix: prevent duplicate inventory reservation
test: add order integration tests
refactor: simplify event publisher
docs: update architecture
```

Avoid:

```text
update
changes
fix stuff
final
final2
```

---

# 48. Pull Request Rules

Every meaningful change should be reviewable.

A change should clearly explain:

- What changed
- Why it changed
- How it was tested
- Any architectural implications

Large unrelated changes should not be mixed into one change.

---

# 49. Definition of Safe Change

A change is considered safe when:

- The solution builds.
- Tests pass.
- Architecture rules remain valid.
- Security is not weakened.
- Existing behavior is preserved unless intentionally changed.
- Documentation is updated where necessary.
- `memory.md` reflects the new project state.

---

# 50. Final Rule

The most important rule is:

> **Do not add complexity just because the project is intended to demonstrate advanced engineering.**

Every architectural pattern, library, infrastructure component, and abstraction must have a clear purpose.

The project should demonstrate that the developer understands:

```text
Why
```

not just:

```text
How
```

The final codebase must be something the developer can confidently explain in a technical interview, maintain over time, and extend without fear of breaking the architecture.

When in doubt, prefer:

```text
Simple
Explicit
Testable
Observable
Reliable
```

over:

```text
Complex
Implicit
Over-abstracted
Unobservable
Fragile
```
