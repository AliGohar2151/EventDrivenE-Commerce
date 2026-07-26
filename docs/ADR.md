# Architecture Decision Records

This document captures key architectural decisions made during the development of the Event-Driven E-Commerce Backend.

---

## ADR-001: Clean Architecture with Explicit Layer Dependencies

**Status:** Accepted

**Context:**
Need a maintainable structure that isolates business logic from infrastructure concerns and remains testable without external services.

**Decision:**
Adopt Clean Architecture with explicit layer dependency flow: `Domain ← Application ← Infrastructure ← API`. Domain has zero external dependencies.

**Consequences:**
- Domain and Application layers are fully testable with in-memory fakes.
- Infrastructure can be swapped without touching business logic.
- Architecture tests enforce dependency rules automatically.

---

## ADR-002: Domain Events + Integration Events Separation

**Status:** Accepted

**Context:**
Domain state changes need to propagate both within the aggregate boundary and across service boundaries.

**Decision:**
Use `IDomainEvent` for intra-aggregate signaling, and `IIntegrationEvent` contracts in `ECommerce.Contracts` for cross-service messaging via `IEventBus`.

**Consequences:**
- Domain layer remains free of messaging infrastructure.
- Integration events are versioned and serializable.

---

## ADR-003: Outbox + Inbox Patterns for Reliable Messaging

**Status:** Accepted

**Context:**
`InMemoryEventBus` is not durable — events can be lost on crashes.

**Decision:**
Implement Outbox Pattern (`OutboxMessage`) to atomically persist events with the originating DB transaction. Implement Inbox Pattern (`InboxMessage`) for idempotent consumer deduplication keyed on `(MessageId, HandlerName)`.

**Consequences:**
- Zero event loss on crash (at-least-once delivery).
- Duplicate events handled safely without side effects.

---

## ADR-004: Optimistic Concurrency for Inventory

**Status:** Accepted

**Context:**
Concurrent order requests for the same product risk overselling.

**Decision:**
Add `Version` concurrency token to `InventoryItem`. EF Core throws `DbUpdateConcurrencyException` on stale-read writes, which the caller retries.

**Consequences:**
- No pessimistic locking overhead.
- Overselling prevented at the database level.

---

## ADR-005: Idempotency Keys for Payment Processing

**Status:** Accepted

**Context:**
Payment provider calls may be retried, causing duplicate charges.

**Decision:**
`Payment.IdempotencyKey` is stored with a unique DB index. Duplicate payment requests with the same key return the original result instead of processing again.

**Consequences:**
- Safe payment retries with zero duplicate charge risk.
- Client controls idempotency key generation.

---

## ADR-006: Rate Limiting at API Layer

**Status:** Accepted

**Context:**
Public API endpoints need protection against abuse and resource exhaustion.

**Decision:**
Use ASP.NET Core built-in `AddRateLimiter` with a fixed-window policy (100 requests per minute, queue limit 10). Returns `429 Too Many Requests` on rejection.

**Consequences:**
- Simple configuration without external infrastructure.
- Per-IP limiting sufficient for current scale.

---

## ADR-007: Correlation ID Propagation

**Status:** Accepted

**Context:**
Distributed event processing makes log tracing across services difficult without request context.

**Decision:**
`CorrelationIdMiddleware` extracts or generates an `X-Correlation-ID` UUID on every request, injects it into `ILogger` scope, and echoes it in the HTTP response header.

**Consequences:**
- Every log entry for a request carries the same Correlation ID.
- Integration events carry `CorrelationId` for end-to-end trace stitching.
