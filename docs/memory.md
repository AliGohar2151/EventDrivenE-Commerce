# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 15 — CI/CD (Completed)
**Next Phase:** Phase 16 — Production Hardening

---

## 1. Overview of Progress

### Phase 0 — Project Foundation (COMPLETED)
- Initialized Git repository, `.gitignore`, `Directory.Build.props` (.NET 10), `Directory.Packages.props`, `.editorconfig`.
- Created Clean Architecture solution (`ECommerce.slnx`) with 5 `src` projects and 3 `tests` projects.
- Added native ASP.NET Core Health Checks at `/health`.

### Phase 1 — Domain Foundation (COMPLETED)
- Created domain primitives (`IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `DomainException`).

### Phase 2 — Database & Persistence (COMPLETED)
- EF Core and PostgreSQL packages. `ApplicationDbContext` and explicit `IEntityTypeConfiguration<T>` mappings.

### Phase 3 — Authentication & Authorization (COMPLETED)
- JWT Authentication, PBKDF2/SHA256 password hashing, RBAC + Permission Policy Authorization. `AuthenticationController`.

### Phase 4 — Product Catalog (COMPLETED)
- Product & Category management, variants, SKU tracking, multi-parameter search/filter/sort/pagination. `ProductsController` & `CategoriesController`.

### Phase 5 — Inventory Management (COMPLETED)
- Stock reservation, overselling prevention, optimistic concurrency tokens. `InventoryController`.

### Phase 6 — Shopping Cart (COMPLETED)
- Cart creation, item management, user cart isolation. `CartController`.

### Phase 7 — Order Management (COMPLETED)
- Order lifecycle state machine. Stock reservation on order creation, release on cancellation. `OrdersController`.

### Phase 8 — Event-Driven Messaging Foundation (COMPLETED)
- Integration event contracts, `IEventBus` abstraction, `InMemoryEventBus` with JSON transport.

### Phase 9 — Payment Workflow (COMPLETED)
- Payment entity, `IPaymentProvider` gateway abstraction, `MockPaymentProvider`, idempotency key tracking. `PaymentsController`.

### Phase 10 — Reliable Event Processing (COMPLETED)
- Outbox Pattern, Inbox Pattern deduplication, `ResilientConsumer` with Exponential Backoff + Jitter, Dead-Letter storage, distributed Correlation ID.

### Phase 11 — Notification System (COMPLETED)
- `Notification` aggregate root, `INotificationProvider`, `MockNotificationProvider`, `OrderCreatedNotificationConsumer`, `PaymentNotificationConsumer`. `NotificationsController`.

### Phase 12 — Observability (COMPLETED)
- `CorrelationIdMiddleware` (`X-Correlation-ID` header + logger scope), `ECommerceMetrics` custom counters, `/health/live` & `/health/ready` probe endpoints.

### Phase 13 — Testing & Quality (COMPLETED)
- End-to-End Workflow Integration Tests (`E2EWorkflowTests`), `CustomWebApplicationFactory` isolated fixture, expanded Architecture boundary rules, Domain unit tests.

### Phase 14 — Docker & Local Infrastructure (COMPLETED)
- Multi-stage `Dockerfile`, `docker-compose.yml` orchestrating `postgres`, `redis`, `rabbitmq`, and `api` with health checks and persistent volumes. `.dockerignore`.

### Phase 15 — CI/CD (COMPLETED)
- GitHub Actions workflow (`.github/workflows/ci.yml`) automating: checkout, .NET 10 SDK setup, `dotnet restore`, `dotnet build -c Release`, `dotnet test`, and Docker image build verification.
- Pipeline triggers on `push` and `pull_request` to `master`/`main`.

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Concurrency Protection:** Optimistic concurrency tokens (`Version`) on `InventoryItem`.
- **Payment Idempotency:** Unique index on `IdempotencyKey` prevents duplicate transaction processing.
- **Messaging Reliability:** Outbox + Inbox patterns, Exponential Backoff + Jitter retries, Dead-Letter storage.
- **Observability:** `X-Correlation-ID` across logs/HTTP/events, `System.Diagnostics.Metrics` instrumentation.
- **Integration Testing:** Isolated in-memory `CustomWebApplicationFactory` test fixture.
- **Container Orchestration:** Health-checked `docker-compose.yml` with isolated persistent volumes.
- **CI/CD:** GitHub Actions pipeline enforcing build quality, full test suite, and Docker build on every PR.

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All 72 tests (67 Unit, 2 Integration, 3 Architecture) passing cleanly.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 16 — Production Hardening**:
  - Security review, dependency review, rate limiting, input validation hardening.
