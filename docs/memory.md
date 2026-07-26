# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 16 — Production Hardening (Completed)
**Next Phase:** Phase 17 — Final Documentation

---

## 1. Overview of Progress

### Phase 0 — Project Foundation (COMPLETED)
- Initialized Git repository, `.gitignore`, `Directory.Build.props` (.NET 10), `Directory.Packages.props`, `.editorconfig`.
- Created Clean Architecture solution (`ECommerce.slnx`) with 5 `src` projects and 3 `tests` projects.

### Phase 1 — Domain Foundation (COMPLETED)
- Domain primitives: `IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `DomainException`.

### Phase 2 — Database & Persistence (COMPLETED)
- EF Core + PostgreSQL, `ApplicationDbContext`, explicit `IEntityTypeConfiguration<T>` mappings.

### Phase 3 — Authentication & Authorization (COMPLETED)
- JWT tokens, PBKDF2/SHA256 hashing, RBAC + Permission Policy Authorization, `AuthenticationController`.

### Phase 4 — Product Catalog (COMPLETED)
- Product & Category CRUD, variants, multi-parameter search/filter/sort/pagination.

### Phase 5 — Inventory Management (COMPLETED)
- Stock reservation, overselling prevention, optimistic concurrency, `InventoryController`.

### Phase 6 — Shopping Cart (COMPLETED)
- Cart CRUD, user isolation, product/inventory validation, `CartController`.

### Phase 7 — Order Management (COMPLETED)
- Order lifecycle state machine, stock reservation, cancellation with release, `OrdersController`.

### Phase 8 — Event-Driven Messaging Foundation (COMPLETED)
- `IEventBus`, `InMemoryEventBus`, integration event contracts, `IIntegrationEventHandler<T>`.

### Phase 9 — Payment Workflow (COMPLETED)
- Payment entity, `IPaymentProvider`, `MockPaymentProvider`, idempotency key, `PaymentsController`.

### Phase 10 — Reliable Event Processing (COMPLETED)
- Outbox + Inbox patterns, `ResilientConsumer` Exponential Backoff + Jitter, Dead-Letter storage, Correlation ID.

### Phase 11 — Notification System (COMPLETED)
- `Notification` aggregate root, `INotificationProvider`, event consumers, `NotificationsController`.

### Phase 12 — Observability (COMPLETED)
- `CorrelationIdMiddleware` (`X-Correlation-ID`), `ECommerceMetrics` custom counters, `/health/live` + `/health/ready`.

### Phase 13 — Testing & Quality (COMPLETED)
- E2E `E2EWorkflowTests`, `CustomWebApplicationFactory`, expanded architecture rules, domain unit tests.

### Phase 14 — Docker & Local Infrastructure (COMPLETED)
- Multi-stage `Dockerfile`, `docker-compose.yml` (`postgres`, `redis`, `rabbitmq`, `api`), `.dockerignore`.

### Phase 15 — CI/CD (COMPLETED)
- `.github/workflows/ci.yml`: build-and-test job + docker-build job, triggered on push/PR to master/main.

### Phase 16 — Production Hardening (COMPLETED)
- `ExceptionHandlingMiddleware`: global unhandled exception handler returning structured `ProblemDetails` JSON.
- ASP.NET Core built-in rate limiting (100 req/min fixed window policy, 429 Too Many Requests on rejection).
- `appsettings.Production.json`: production environment configuration template.
- New unit tests: `ExceptionHandlingMiddlewareTests` (4 tests), `PaymentFailureTests` (5 tests).

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Concurrency Protection:** Optimistic concurrency tokens (`Version`) on `InventoryItem`.
- **Payment Idempotency:** Unique index on `IdempotencyKey` prevents duplicate charges.
- **Messaging Reliability:** Outbox + Inbox patterns, Exponential Backoff + Jitter, Dead-Letter storage.
- **Observability:** `X-Correlation-ID` across logs/HTTP/events, `System.Diagnostics.Metrics` instrumentation.
- **Integration Testing:** Isolated in-memory `CustomWebApplicationFactory` test fixture.
- **Container Orchestration:** Health-checked `docker-compose.yml` with isolated persistent volumes.
- **CI/CD:** GitHub Actions pipeline enforcing build, full test suite, and Docker build on every PR.
- **Production Safety:** Global `ExceptionHandlingMiddleware` + rate limiting + production config template.

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All 81 tests (76 Unit, 2 Integration, 3 Architecture) passing cleanly.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 17 — Final Documentation**:
  - `README.md` (project overview, setup, architecture, API, Docker, CI/CD instructions).
  - Architecture Decision Records (ADRs) for key design choices.
