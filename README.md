# Event-Driven E-Commerce Backend

A production-grade, event-driven e-commerce backend built with **C# / .NET 10** following **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** principles.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                        │
│           Controllers · Middleware · Rate Limiting       │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                   Application Layer                     │
│         Services · Abstractions · Event Consumers        │
└─────────────────────┬───────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                    Domain Layer                         │
│    Entities · Value Objects · Domain Events · Rules      │
└─────────────────────────────────────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────────────┐
│                 Infrastructure Layer                    │
│  EF Core · PostgreSQL · InMemoryEventBus · Middleware    │
└─────────────────────────────────────────────────────────┘
```

### Layers

| Layer | Project | Responsibility |
| :--- | :--- | :--- |
| Domain | `ECommerce.Domain` | Entities, Value Objects, Domain Events, business rules |
| Contracts | `ECommerce.Contracts` | Shared DTOs, integration event contracts |
| Application | `ECommerce.Application` | Services, use case orchestration, event consumers |
| Infrastructure | `ECommerce.Infrastructure` | EF Core, PostgreSQL, messaging, middleware |
| API | `ECommerce.Api` | HTTP endpoints, authentication, rate limiting |

---

## Features

| Feature | Description |
| :--- | :--- |
| **Authentication & Authorization** | JWT access tokens + refresh token rotation, PBKDF2/SHA-256 password hashing, RBAC permission policies |
| **Product Catalog** | Product & category management, product variants, multi-parameter search/filter/sort/pagination |
| **Inventory Management** | Stock reservation, overselling prevention (`AvailableQty = Stock - Reserved`), optimistic concurrency |
| **Shopping Cart** | Per-user cart management with product and inventory validation |
| **Order Management** | Full order lifecycle state machine (`Pending → Paid → Shipped → Delivered`) |
| **Payment Processing** | `IPaymentProvider` gateway abstraction, idempotency key deduplication |
| **Event-Driven Messaging** | `IEventBus`, `InMemoryEventBus`, Outbox + Inbox patterns, retry + dead-letter |
| **Notifications** | Order & payment event-triggered async notifications |
| **Observability** | `X-Correlation-ID` propagation, custom metrics, `/health/live` + `/health/ready` probes |
| **Rate Limiting** | 100 req/min fixed-window per IP (429 Too Many Requests) |
| **Global Error Handling** | Structured `ProblemDetails` responses with correlation ID |

---

## Technology Stack

| Technology | Usage |
| :--- | :--- |
| .NET 10 | Target framework |
| ASP.NET Core | HTTP API framework |
| Entity Framework Core 9 | ORM |
| PostgreSQL (Npgsql) | Primary database |
| JWT Bearer | Authentication |
| xUnit + FluentAssertions | Unit & integration testing |
| NetArchTest | Architecture boundary enforcement |
| Docker + Docker Compose | Container orchestration |
| GitHub Actions | CI/CD pipeline |

---

## Project Structure

```
EventDrivenE-Commerce/
├── src/
│   ├── ECommerce.Contracts/          # Shared DTOs & integration event contracts
│   ├── ECommerce.Domain/             # Domain entities, value objects, domain events
│   ├── ECommerce.Application/        # Application services, consumers, abstractions
│   ├── ECommerce.Infrastructure/     # EF Core, PostgreSQL, messaging, middleware
│   └── ECommerce.Api/                # API controllers, Program.cs, middleware pipeline
├── tests/
│   ├── ECommerce.UnitTests/          # Unit tests (Domain, Services, Middleware)
│   ├── ECommerce.IntegrationTests/   # Integration & E2E workflow tests
│   └── ECommerce.ArchitectureTests/  # NetArchTest architecture boundary rules
├── docs/
│   ├── phases.md                     # Project phases & status tracking
│   ├── memory.md                     # Persistent project memory
│   ├── PRD.md                        # Product Requirements Document
│   ├── Architecture.md               # Architecture Decision Records
│   └── rules.md                      # Coding standards & rules
├── Dockerfile                        # Multi-stage container build
├── docker-compose.yml                # Local infrastructure orchestration
├── .github/workflows/ci.yml          # GitHub Actions CI/CD pipeline
└── Directory.Packages.props          # Central Package Management
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK (preview)](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Local Development (Docker)

Start the full local infrastructure stack:

```bash
docker compose up --build
```

This starts:
- **PostgreSQL 16** on port `5432`
- **Redis 7** on port `6379`
- **RabbitMQ 3** on port `5672` (Management UI: http://localhost:15672)
- **API** on port `8080` (http://localhost:8080/health)

### Local Development (.NET)

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run --project src/ECommerce.Api
```

---

## API Endpoints

### Authentication
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/v1/auth/register` | Register a new user |
| `POST` | `/api/v1/auth/login` | Login and receive JWT + refresh token |
| `POST` | `/api/v1/auth/refresh` | Refresh access token |
| `POST` | `/api/v1/auth/revoke` | Revoke refresh token |
| `GET` | `/api/v1/auth/me` | Get current user info |

### Products & Categories
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/products` | List products (search, filter, sort, paginate) |
| `POST` | `/api/v1/products` | Create product |
| `GET` | `/api/v1/products/{id}` | Get product by ID |
| `PUT` | `/api/v1/products/{id}` | Update product |
| `DELETE` | `/api/v1/products/{id}` | Delete product |
| `GET` | `/api/v1/categories` | List categories |
| `POST` | `/api/v1/categories` | Create category |

### Inventory
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/inventory/{productId}` | Get inventory for product |
| `POST` | `/api/v1/inventory/{productId}/adjust` | Adjust stock quantity |
| `POST` | `/api/v1/inventory/{productId}/reserve` | Reserve stock |
| `POST` | `/api/v1/inventory/{productId}/release` | Release reservation |

### Cart
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/cart` | View current cart |
| `POST` | `/api/v1/cart/items` | Add item to cart |
| `PUT` | `/api/v1/cart/items/{productId}` | Update item quantity |
| `DELETE` | `/api/v1/cart/items/{productId}` | Remove item from cart |
| `DELETE` | `/api/v1/cart` | Clear cart |

### Orders
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/v1/orders` | Place a new order |
| `GET` | `/api/v1/orders` | List user orders |
| `GET` | `/api/v1/orders/{id}` | Get order details |
| `POST` | `/api/v1/orders/{id}/cancel` | Cancel order |

### Payments
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/v1/payments` | Process payment for an order |
| `GET` | `/api/v1/payments/{orderId}` | Get payment status |

### Notifications
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/v1/notifications` | Get user notifications |

### Health
| Endpoint | Description |
| :--- | :--- |
| `/health` | Full health check |
| `/health/live` | Liveness probe |
| `/health/ready` | Readiness probe |

---

## Event Flow

```
Order Placed
    │
    ▼
OrderCreatedIntegrationEvent published
    │
    ├──► OrderCreatedIntegrationEventHandler  → reserve stock
    └──► OrderCreatedNotificationConsumer     → send order confirmation notification

Payment Processed
    │
    ▼
PaymentSucceededIntegrationEvent published
    │
    └──► PaymentNotificationConsumer          → send payment success notification

PaymentFailedIntegrationEvent published
    │
    └──► PaymentNotificationConsumer          → send payment failed notification
```

---

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test tests/ECommerce.UnitTests
dotnet test tests/ECommerce.IntegrationTests
dotnet test tests/ECommerce.ArchitectureTests
```

### Test Summary

| Suite | Count | Description |
| :--- | :--- | :--- |
| Unit Tests | 76 | Domain, services, middleware |
| Integration Tests | 2 | E2E workflow + health check |
| Architecture Tests | 3 | Clean Architecture boundary rules |
| **Total** | **81** | |

---

## CI/CD

GitHub Actions pipeline (`.github/workflows/ci.yml`) automatically runs on every push and pull request to `master`/`main`:

```
Checkout → .NET 10 Setup → Restore → Build (Release) → Test → Docker Build
```

---

## Reliability Patterns

| Pattern | Implementation |
| :--- | :--- |
| **Outbox Pattern** | `OutboxMessage` — atomic DB + event publishing |
| **Inbox Pattern** | `InboxMessage` — idempotent consumer deduplication |
| **Retry with Backoff** | `ResilientConsumer` — Exponential Backoff + Jitter |
| **Dead-Letter Storage** | `DeadLetterMessage` — failed message routing |
| **Idempotency Keys** | `Payment.IdempotencyKey` — unique index preventing duplicate charges |
| **Optimistic Concurrency** | `InventoryItem.Version` — concurrent stock update protection |

---

## License

MIT
