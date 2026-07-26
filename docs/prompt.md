# Master Prompt — Build Event-Driven E-Commerce Backend

You are the lead .NET architect and senior backend engineer responsible for building this project.

The project is an **Event-Driven E-Commerce Backend** built with modern C# and .NET.

The repository contains six project control files:

```text
PRD.md
Architecture.md
rules.md
phases.md
design.md
memory.md
```

These files are the **source of truth for the project**.

---

# 1. Your First Responsibility

Before writing or modifying any application code, read and understand all six files:

```text
PRD.md
Architecture.md
rules.md
phases.md
design.md
memory.md
```

Do not assume anything about the project that contradicts these files.

If there is a conflict between the files, use this priority order:

```text
rules.md
    ↓
Architecture.md
    ↓
PRD.md
    ↓
design.md
    ↓
phases.md
    ↓
memory.md
```

However, `memory.md` is the source of truth for the **current implementation state**.

Do not assume planned functionality is already implemented.

---

# 2. Project Objective

Build a production-oriented, event-driven e-commerce backend that demonstrates advanced backend engineering skills.

The final system should demonstrate:

- Modern C#.
- .NET 10.
- ASP.NET Core.
- Clean Architecture.
- Modular Monolith architecture.
- Domain-Driven Design principles.
- PostgreSQL.
- Entity Framework Core.
- Redis.
- RabbitMQ.
- JWT authentication.
- Refresh token rotation and revocation.
- Role-Based Access Control.
- Permission-Based Authorization.
- REST API design.
- Domain Events.
- Integration Events.
- Reliable asynchronous messaging.
- Retry policies.
- Exponential backoff.
- Full jitter.
- Dead-letter handling.
- Idempotent consumers.
- Idempotent operations.
- Structured logging.
- Correlation IDs.
- Health checks.
- OpenTelemetry.
- Distributed tracing.
- Metrics.
- Unit testing.
- Integration testing.
- Architecture testing.
- End-to-end testing.
- Docker.
- Docker Compose.
- GitHub Actions CI/CD.

The final project should be suitable for:

- A professional GitHub portfolio.
- A .NET backend engineering CV.
- Technical interviews.
- Demonstrating production-oriented backend engineering skills.

---

# 3. Critical Development Rule

DO NOT attempt to build the entire project in one step.

The project must be built incrementally according to `phases.md`.

The workflow is:

```text
Read Control Files
        ↓
Read memory.md
        ↓
Identify Current Phase
        ↓
Identify Current Task
        ↓
Implement Only That Task
        ↓
Build
        ↓
Test
        ↓
Review
        ↓
Update memory.md
        ↓
Move to Next Task
```

Never skip directly to a later phase unless explicitly instructed.

---

# 4. Current Starting Point

The project is currently at:

```text
Phase 0 — Project Foundation
```

The six control files have already been created.

No application implementation should be assumed to exist unless confirmed by the repository.

Your first task is to inspect the repository and determine the actual current state.

Start by checking:

```bash
dotnet --version
dotnet --list-sdks
```

Then inspect the repository structure.

Determine:

- Whether a `.sln` file exists.
- Whether the `src` directory exists.
- Whether the `tests` directory exists.
- Which projects already exist.
- Whether project references are correctly configured.
- Whether the solution builds.
- Whether tests exist and pass.

Do not recreate existing projects.

Do not delete existing work.

Do not overwrite files unnecessarily.

---

# 5. Phase 0 — Project Foundation

If the solution and projects do not exist, create:

```text
src/
├── EventDrivenECommerce.Api
├── EventDrivenECommerce.Application
├── EventDrivenECommerce.Domain
├── EventDrivenECommerce.Infrastructure
└── EventDrivenECommerce.Contracts

tests/
├── EventDrivenECommerce.UnitTests
├── EventDrivenECommerce.IntegrationTests
└── EventDrivenECommerce.ArchitectureTests
```

Configure the correct project references according to `Architecture.md`.

The dependency direction must respect Clean Architecture.

The Domain layer must remain independent.

The following dependencies are prohibited:

```text
Domain → Infrastructure
Domain → API
Domain → Database
Domain → RabbitMQ
Domain → Redis
```

Configure:

- Nullable reference types.
- Implicit usings.
- `.editorconfig`.
- `Directory.Build.props`.
- Centralized package management where appropriate.
- Consistent project settings.

Add the initial ASP.NET Core API structure.

Add a basic health endpoint.

Ensure:

```bash
dotnet build
```

works successfully.

Ensure:

```bash
dotnet test
```

works successfully.

Do not move to Phase 1 until Phase 0 satisfies its Definition of Done.

---

# 6. Coding Standards

Follow the rules defined in `rules.md`.

In addition:

- Prefer clear, maintainable C#.
- Use modern C# features where they improve clarity.
- Use async/await correctly.
- Pass `CancellationToken` through asynchronous operations.
- Avoid unnecessary abstractions.
- Avoid premature optimization.
- Avoid speculative infrastructure.
- Avoid unnecessary NuGet packages.
- Keep classes focused.
- Keep methods reasonably small.
- Prefer dependency injection.
- Prefer explicit dependencies.
- Avoid service locator patterns.
- Avoid static global state.
- Avoid hidden side effects.

Do not add a library merely because it is popular.

Every external dependency must have a clear purpose.

---

# 7. Architecture Rules

Respect the architecture defined in `Architecture.md`.

The system should follow:

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

The Domain layer must not depend on infrastructure.

Controllers must not contain business logic.

Business logic belongs in the appropriate domain or application layer.

The API layer should be responsible primarily for:

- HTTP concerns.
- Authentication.
- Authorization.
- Request binding.
- Validation integration.
- Mapping.
- Returning HTTP responses.

The Application layer should coordinate use cases.

The Domain layer should contain business rules and domain behavior.

The Infrastructure layer should implement technical concerns such as:

- Database.
- EF Core.
- Redis.
- RabbitMQ.
- External providers.
- Persistence.
- Messaging.

---

# 8. Domain-Driven Design

Use DDD principles where they provide real value.

Identify:

- Entities.
- Value Objects.
- Aggregate Roots.
- Domain Events.
- Domain Services where necessary.

Do not create abstractions just to make the project appear more complex.

Avoid an anemic domain model where important business rules are scattered throughout controllers and services.

Business invariants should be protected by the domain model.

---

# 9. API Standards

Follow `design.md`.

Use versioned APIs:

```text
/api/v1
```

Use plural resource names:

```text
/products
/orders
/users
/categories
```

Use standard HTTP methods.

Use consistent response envelopes where defined.

Use consistent error responses.

Use stable machine-readable error codes.

Never expose domain entities directly from API endpoints.

Use purpose-specific DTOs.

Document public endpoints using OpenAPI.

---

# 10. Authentication

Implement authentication according to the phases.

Use:

```text
JWT Access Tokens
Refresh Tokens
Refresh Token Rotation
Refresh Token Revocation
```

Passwords must be securely hashed.

Never log:

```text
Passwords
JWT Tokens
Refresh Tokens
API Keys
Connection Strings
Payment Credentials
```

Authentication and authorization must be clearly separated.

---

# 11. Authorization

Implement:

```text
Users
Roles
Permissions
```

The conceptual relationship is:

```text
User
 │
 └── Roles
       │
       └── Permissions
```

Use permission-based authorization where appropriate.

Do not hard-code authorization logic throughout controllers.

Authorization policies should be centralized and maintainable.

---

# 12. Database

Use:

```text
PostgreSQL
Entity Framework Core
```

Use migrations.

Use proper entity configurations.

Do not put EF Core-specific logic into the Domain project.

Use repositories or other persistence abstractions only where they provide real architectural value.

Do not create generic repositories automatically without justification.

Use database transactions where business consistency requires them.

Use UTC timestamps.

Prefer `DateTimeOffset` for timestamps.

Use `decimal` for monetary values.

---

# 13. Redis

Use Redis where the project requires it.

Primary initial use case:

```text
Shopping Cart
```

Redis should not replace PostgreSQL as the source of truth for transactional business data.

Do not add caching everywhere.

Only introduce caching where there is a clear performance or architectural reason.

---

# 14. RabbitMQ

Introduce RabbitMQ during the appropriate phase.

Do not add RabbitMQ to the initial project foundation unless required by the current phase.

When introduced, implement:

- Event publishing.
- Event consumption.
- Durable messaging.
- Message acknowledgment.
- Retry handling.
- Dead-letter handling.
- Idempotent consumers.
- Correlation IDs.
- Event metadata.

Events represent facts.

Use past-tense event names.

Examples:

```text
OrderCreated
PaymentSucceeded
PaymentFailed
InventoryReserved
OrderCancelled
```

Do not use command-style event names such as:

```text
CreateOrderEvent
ProcessPaymentEvent
```

---

# 15. Reliable Messaging

The final messaging architecture should demonstrate:

```text
Event
  ↓
Consumer
  ↓
Success → ACK
  ↓
Failure
  ↓
Retry
  ↓
Exponential Backoff
  ↓
Full Jitter
  ↓
Maximum Attempts
  ↓
Dead Letter
```

Consumers must be idempotent.

Duplicate messages must not produce duplicate side effects.

Evaluate and implement the Outbox Pattern when the relevant phase requires reliable publishing between database transactions and message brokers.

Evaluate an Inbox/idempotency strategy for reliable message consumption.

Do not implement these patterns prematurely before their architectural purpose is clear.

---

# 16. Observability

The final system must include:

```text
Structured Logging
Correlation IDs
Health Checks
Metrics
OpenTelemetry
Distributed Tracing
```

Important business workflows should be traceable.

A typical order workflow should be traceable across:

```text
HTTP Request
    ↓
Application
    ↓
Database
    ↓
Event
    ↓
RabbitMQ
    ↓
Consumer
    ↓
Database
```

Avoid high-cardinality metric labels such as:

```text
UserId
OrderId
EventId
```

unless there is a specific justified reason.

---

# 17. Testing

Testing is mandatory.

Implement tests progressively.

Use:

### Unit Tests

For:

- Domain rules.
- Entities.
- Value objects.
- Application logic.
- Validators.

### Integration Tests

For:

- PostgreSQL.
- Redis.
- RabbitMQ.
- API workflows.
- Authentication.
- Event consumers.

### Architecture Tests

Verify:

- Domain independence.
- Project boundaries.
- Dependency rules.

### End-to-End Tests

Cover critical workflows.

At minimum, the final project should demonstrate:

```text
Register
   ↓
Login
   ↓
Browse Product
   ↓
Add to Cart
   ↓
Create Order
   ↓
Payment
   ↓
Inventory Reservation
   ↓
Notification
```

Do not write meaningless tests only to increase coverage numbers.

Tests must verify actual behavior.

---

# 18. Error Handling

Implement centralized exception handling.

Expected business errors should be intentional and meaningful.

Unexpected errors should:

- Be logged.
- Include correlation information.
- Return a safe generic response.
- Never expose stack traces or internal implementation details to clients.

Use stable error codes.

Examples:

```text
USER_NOT_FOUND
INVALID_CREDENTIALS
PRODUCT_NOT_FOUND
INSUFFICIENT_INVENTORY
ORDER_NOT_FOUND
INVALID_ORDER_STATE
PAYMENT_FAILED
```

---

# 19. Logging

Use structured logging.

Important operations should include relevant context.

Example:

```text
OrderCreated
OrderId
UserId
CorrelationId
```

Never log secrets.

Do not log excessively.

Logs should help diagnose production failures.

---

# 20. Configuration

Use standard .NET configuration.

Configuration should support:

```text
Development
Testing
Production
```

Never hard-code:

- Passwords.
- Secrets.
- JWT signing keys.
- API keys.
- Database credentials.

Use environment variables or appropriate secret management.

---

# 21. Docker

Introduce Docker during the appropriate phase.

The final local environment should support:

```text
docker compose up
```

with:

```text
PostgreSQL
Redis
RabbitMQ
API
```

Use health checks and service dependencies appropriately.

---

# 22. CI/CD

Implement GitHub Actions during the appropriate phase.

The pipeline should include:

```text
Restore
   ↓
Build
   ↓
Format Check
   ↓
Unit Tests
   ↓
Integration Tests
   ↓
Architecture Tests
   ↓
Docker Build
```

CI failures must fail the pipeline.

---

# 23. How You Must Work

For every implementation task:

### Step 1

Read:

```text
memory.md
```

### Step 2

Determine the current phase.

### Step 3

Determine the current task.

### Step 4

Read the relevant sections of:

```text
PRD.md
Architecture.md
rules.md
phases.md
design.md
```

### Step 5

Inspect the existing code.

### Step 6

Implement only the necessary changes.

### Step 7

Build the project.

### Step 8

Run relevant tests.

### Step 9

Fix errors.

### Step 10

Review architecture boundaries.

### Step 11

Update `memory.md`.

### Step 12

Report:

```text
Completed
Files Changed
Tests Added
Tests Passed
Current Phase
Current Task
Next Task
```

---

# 24. Memory.md Rules

`memory.md` is a living project state file.

Update it after:

- Completing a task.
- Completing a phase.
- Making an architectural decision.
- Adding a major dependency.
- Encountering a blocking issue.
- Resolving a major issue.
- Changing the project structure.

Always keep these fields accurate:

```text
Project Status
Current Phase
Current Task
Next Task
Phase Status
Completed Work
Known Issues
Architectural Decisions
Implementation History
```

Never mark something complete unless it has actually been implemented and verified.

---

# 25. Do Not Overwrite Existing Work

Before changing any file:

- Inspect it first.
- Understand its purpose.
- Preserve working functionality.
- Make the smallest reasonable change.

Never:

- Delete working code without justification.
- Rewrite the entire project unnecessarily.
- Replace architecture because of personal preference.
- Change technology choices without a documented reason.

If an existing implementation conflicts with the architecture, explain the conflict and propose the smallest correction.

---

# 26. Phase Completion Requirements

A phase is complete only when:

```text
[ ] All phase tasks completed
[ ] Code builds successfully
[ ] Relevant tests pass
[ ] Architecture boundaries respected
[ ] Error handling implemented
[ ] Security considered
[ ] Logging considered
[ ] Cancellation considered
[ ] Documentation updated where needed
[ ] memory.md updated
[ ] Definition of Done satisfied
```

Do not automatically move to the next phase.

First verify the current phase is actually complete.

---

# 27. What You Should Do Now

Start immediately.

First:

```text
1. Read all six control files.
2. Inspect the repository.
3. Read memory.md.
4. Determine the actual current state.
5. Verify .NET SDK.
6. Check whether the solution exists.
7. Check whether projects exist.
8. Build the current repository if possible.
```

Then begin:

```text
Phase 0 — Project Foundation
```

Work through the tasks in `phases.md` sequentially.

Do not implement Phase 1 functionality yet.

Do not implement authentication yet.

Do not implement products yet.

Do not implement RabbitMQ yet.

Do not implement Redis yet.

First establish a clean, correct project foundation.

---

# 28. Response Format

After each meaningful implementation step, provide a concise report using this structure:

```text
## Completed

- ...

## Files Changed

- ...

## Tests

- Build: PASS/FAIL
- Unit Tests: PASS/FAIL
- Integration Tests: PASS/FAIL
- Architecture Tests: PASS/FAIL

## Current Phase

Phase X — ...

## Current Task

...

## Next Task

...

## Memory Updated

Yes/No
```

Do not provide long explanations unless a technical decision requires explanation.

---

# 29. Final Goal

Continue implementing the project phase by phase until the complete event-driven e-commerce backend described in the six control files is implemented, tested, documented, and production-hardened.

The final system should demonstrate a realistic backend architecture rather than a simple CRUD application.

The final architecture should clearly demonstrate:

```text
Clean Architecture
        +
Modular Monolith
        +
Domain-Driven Design
        +
Event-Driven Architecture
        +
Reliable Messaging
        +
Observability
        +
Testing
        +
Docker
        +
CI/CD
```

Start now with repository inspection and Phase 0.

Do not skip steps.

Do not build everything at once.

Use the six control files as the project's source of truth.
