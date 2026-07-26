# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 12 — Observability (Completed)
**Next Phase:** Phase 13 — Testing & Quality

---

## 1. Overview of Progress

### Phase 0 — Project Foundation (COMPLETED)
- Initialized Git repository, `.gitignore`, `Directory.Build.props` (.NET 10), `Directory.Packages.props`, `.editorconfig`.
- Created Clean Architecture solution (`ECommerce.slnx`) with 5 `src` projects and 3 `tests` projects.
- Added native ASP.NET Core Health Checks at `/health`.

### Phase 1 — Domain Foundation (COMPLETED)
- Created domain primitives (`IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `DomainException`).
- Unit tests covering domain primitives and architecture boundary rules.

### Phase 2 — Database & Persistence (COMPLETED)
- EF Core and PostgreSQL packages (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`).
- Created foundational domain entities (`User`, `Role`, `Permission`, `Category`, `Product`).
- Created `ApplicationDbContext` and explicit `IEntityTypeConfiguration<T>` mappings.

### Phase 3 — Authentication & Authorization (COMPLETED)
- JWT Authentication & Security packages (`Microsoft.AspNetCore.Authentication.JwtBearer`).
- DTOs, domain entities (`RefreshToken`, `UserRole`, `RolePermission`), Application services (`AuthenticationService`), and Infrastructure security (`PasswordHasher`, `JwtProvider`, `PermissionAuthorizationHandler`).
- `AuthenticationController` exposing `/api/v1/auth/register`, `/login`, `/refresh`, `/revoke`, and `/me`.

### Phase 4 — Product Catalog (COMPLETED)
- Contracts DTOs in `src/ECommerce.Contracts/Products`: `CreateProductRequest`, `UpdateProductRequest`, `ProductResponse`, `CreateCategoryRequest`, `UpdateCategoryRequest`, `CategoryResponse`, `ProductQueryParameters`, `PagedListResponse<T>`.
- Extended domain models: `Product` (AggregateRoot with status, variants, domain events), `Category`, `ProductVariant` (ValueObject), `ProductStatus` enum, `ProductCreatedDomainEvent`, `ProductUpdatedDomainEvent`.
- Application Services: `IProductService`, `ICategoryService`, `ProductService`, `CategoryService` (supporting multi-parameter search, category filter, min/max price filter, status filter, multi-field sorting, and page-based pagination).
- Infrastructure mappings: `ProductConfiguration` updated for `ProductStatus` conversion and `ProductVariant` owned collection mapping.
- API Controllers: `ProductsController` and `CategoriesController`.

### Phase 5 — Inventory Management (COMPLETED)
- Stock reservation, overselling prevention (`AvailableQuantity = StockQuantity - ReservedQuantity`), stock adjustment, release, commit, low-stock alerts.
- EF Core optimistic concurrency tokens (`Version`).
- `InventoryController` API endpoints.

### Phase 6 — Shopping Cart (COMPLETED)
- Cart creation, view, item addition (with product & inventory stock validation), quantity updates, item removal, cart clearing.
- User cart isolation & thread-safe storage repository (`CartRepository`).
- `CartController` API endpoints.

### Phase 7 — Order Management (COMPLETED)
- Core order lifecycle and state machine transitions (`Pending` -> `PaymentProcessing` -> `Paid` -> `Processing` -> `Shipped` -> `Delivered`).
- Order placement with inventory stock reservation and shopping cart deletion.
- Cancellation with stock release.
- `OrdersController` API endpoints.

### Phase 8 — Event-Driven Messaging Foundation (COMPLETED)
- Integration event contracts (`OrderCreatedIntegrationEvent`, `OrderStatusChangedIntegrationEvent`, `StockReservedIntegrationEvent`, `PaymentRequestedIntegrationEvent`).
- Publisher abstraction `IEventBus` & consumer handler `IIntegrationEventHandler<TEvent>`.
- Transport bus `InMemoryEventBus` with JSON payload transport serialization and handler dispatching.

### Phase 9 — Payment Workflow (COMPLETED)
- Payment domain model & status tracking (`Pending`, `Processing`, `Completed`, `Failed`, `Refunded`).
- `IPaymentProvider` gateway abstraction and `MockPaymentProvider` implementation.
- `IdempotencyKey` tracking to guarantee duplicate requests return original transaction without double-charging.
- Integration event publishing (`PaymentRequestedIntegrationEvent`, `PaymentSucceededIntegrationEvent`, `PaymentFailedIntegrationEvent`).
- `PaymentsController` API endpoints.

### Phase 10 — Reliable Event Processing (COMPLETED)
- Outbox Pattern (`OutboxMessage`) for atomic database transaction event publishing.
- Inbox Pattern (`InboxMessage`) for consumer idempotency & deduplication (`MessageId` + `HandlerName`).
- Resilient Consumer (`ResilientConsumer`) with Exponential Backoff + Jitter retry policy and configurable retry limit.
- Dead-Letter Queue / Storage (`DeadLetterMessage`) for routing exhausted message failures.
- Distributed Correlation ID propagation across all integration events.

### Phase 11 — Notification System (COMPLETED)
- Notification aggregate root (`Notification`), status tracking (`Pending`, `Sent`, `Failed`), and domain events (`NotificationSentDomainEvent`, `NotificationFailedDomainEvent`).
- `INotificationProvider` abstraction and `MockNotificationProvider` implementation.
- Async Integration Event Consumers (`OrderCreatedNotificationConsumer`, `PaymentNotificationConsumer`).
- `NotificationsController` API endpoints (`/api/v1/notifications`).

### Phase 12 — Observability (COMPLETED)
- Correlation ID middleware (`CorrelationIdMiddleware`) extracting/generating `X-Correlation-ID` header and pushing into logging scope.
- Custom metrics instrumentation (`ECommerceMetrics`) for orders, payments, events, retries, and dead-letter messages.
- Liveness (`/health/live`) and readiness (`/health/ready`) probe endpoints.

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Catalog Querying:** Offset-based pagination with `PagedListResponse<T>`, parameterized IQueryable filtering (`Search`, `CategoryId`, `MinPrice`, `MaxPrice`, `Status`), and dynamic sorting.
- **Product Variants:** Modeled as immutable `ValueObject` owned types in EF Core (`product_variants` table).
- **Concurrency Protection:** Optimistic concurrency tokens (`Version`) on `InventoryItem`.
- **Payment Idempotency:** Unique index on `IdempotencyKey` prevents duplicate transaction processing.
- **Messaging Reliability:** Outbox database persistence, Inbox deduplication, Exponential Backoff + Jitter retries, and Dead-Letter storage routing.
- **Async Notifications:** Triggered out-of-band by event consumers without blocking core checkout or payment request threads.
- **Observability:** Distributed `X-Correlation-ID` tracing across logs, HTTP responses, and events with `System.Diagnostics.Metrics` instrumentation.

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All 68 Unit, Integration, and Architecture tests passing cleanly.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 13 — Testing & Quality**:
  - Code coverage review, edge case test expansion, boundary validation, and architecture rule enforcement.
