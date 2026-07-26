<div align="center">

# 🛒 EventDrivenE-Commerce

### A production-grade event-driven e-commerce backend built with .NET 10

[![CI/CD](https://github.com/AliGohar2151/EventDrivenE-Commerce/actions/workflows/ci.yml/badge.svg)](https://github.com/AliGohar2151/EventDrivenE-Commerce/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0--preview-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-81%20passing-brightgreen?logo=xunit)](tests/)
[![Docker](https://img.shields.io/badge/docker-compose-2496ED?logo=docker)](docker-compose.yml)

[Features](#-features) · [Architecture](#-architecture) · [Getting Started](#-getting-started) · [API Reference](#-api-reference) · [Event Flow](#-event-flow) · [Testing](#-testing)

</div>

---

## ✨ Features

| | Feature | Description |
|---|---|---|
| 🔐 | **Authentication & Authorization** | JWT access tokens, refresh token rotation, PBKDF2/SHA-256 hashing, RBAC permission policies |
| 📦 | **Product Catalog** | Full CRUD with variants, SKU tracking, multi-parameter search, filter, sort & pagination |
| 🏭 | **Inventory Management** | Stock reservation, overselling prevention, optimistic concurrency control |
| 🛒 | **Shopping Cart** | Per-user cart with real-time product & inventory validation |
| 📋 | **Order Management** | Full lifecycle state machine (`Pending → Paid → Shipped → Delivered`) |
| 💳 | **Payment Processing** | Provider abstraction, idempotency key deduplication, event-driven status tracking |
| 📡 | **Event-Driven Messaging** | Outbox + Inbox reliability patterns, retry with Exponential Backoff + Jitter |
| 🔔 | **Notifications** | Async order & payment event-triggered notifications |
| 👁️ | **Observability** | `X-Correlation-ID` propagation, custom metrics, liveness & readiness probes |
| 🛡️ | **Production Hardening** | Global exception handling, rate limiting, structured `ProblemDetails` responses |
| 🐳 | **Docker** | Single-command local environment with all infrastructure services |
| ⚙️ | **CI/CD** | GitHub Actions pipeline — build, test & Docker verification on every PR |

---

## 🏗️ Architecture

This project follows **Clean Architecture** with strict layer boundaries enforced by automated architecture tests.

```
┌──────────────────────────────────────────────────────────────┐
│                          API Layer                           │
│        Controllers  ·  Middleware  ·  Rate Limiting          │
└──────────────────────────────┬───────────────────────────────┘
                               │  depends on
┌──────────────────────────────▼───────────────────────────────┐
│                      Application Layer                       │
│           Services  ·  Abstractions  ·  Event Consumers      │
└──────────────────────────────┬───────────────────────────────┘
                               │  depends on
┌──────────────────────────────▼───────────────────────────────┐
│                       Domain Layer                           │
│     Entities  ·  Value Objects  ·  Domain Events  ·  Rules   │
│               ← zero external dependencies →                  │
└──────────────────────────────────────────────────────────────┘
                               ↑  implements
┌──────────────────────────────┴───────────────────────────────┐
│                   Infrastructure Layer                       │
│     EF Core  ·  PostgreSQL  ·  Messaging  ·  Middleware      │
└──────────────────────────────────────────────────────────────┘
```

### Project Structure

```
EventDrivenE-Commerce/
├── src/
│   ├── ECommerce.Contracts/          # Shared DTOs & integration event contracts
│   ├── ECommerce.Domain/             # Entities, Value Objects, Domain Events
│   ├── ECommerce.Application/        # Services, consumers, abstractions
│   ├── ECommerce.Infrastructure/     # EF Core, PostgreSQL, messaging, middleware
│   └── ECommerce.Api/                # HTTP API, controllers, pipeline
├── tests/
│   ├── ECommerce.UnitTests/          # Domain, services, middleware unit tests
│   ├── ECommerce.IntegrationTests/   # E2E workflow & health check tests
│   └── ECommerce.ArchitectureTests/  # Clean Architecture boundary enforcement
├── docs/                             # Architecture docs, ADRs, phases
├── .github/workflows/ci.yml          # GitHub Actions CI/CD pipeline
├── Dockerfile                        # Multi-stage container build
└── docker-compose.yml                # Local infrastructure orchestration
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (preview)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Option 1 — Docker (Recommended)

Start the entire infrastructure with a single command:

```bash
docker compose up --build
```

This spins up:

| Service | Port | Description |
|---|---|---|
| **API** | `8080` | ASP.NET Core REST API |
| **PostgreSQL 16** | `5432` | Primary database |
| **Redis 7** | `6379` | Cache |
| **RabbitMQ 3** | `5672` / `15672` | Message broker / Management UI |

> API will be available at **http://localhost:8080**  
> Health check at **http://localhost:8080/health**  
> RabbitMQ Management UI at **http://localhost:15672** (guest / guest)

### Option 2 — .NET CLI

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run API
dotnet run --project src/ECommerce.Api
```

---

## 📡 API Reference

### 🔐 Authentication

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/auth/register` | Register a new user account |
| `POST` | `/api/v1/auth/login` | Login and receive JWT + refresh token |
| `POST` | `/api/v1/auth/refresh` | Refresh an expired access token |
| `POST` | `/api/v1/auth/revoke` | Revoke a refresh token |
| `GET` | `/api/v1/auth/me` | Get current authenticated user |

### 📦 Products & Categories

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/products` | List products (search, filter, sort, paginate) |
| `POST` | `/api/v1/products` | Create a new product |
| `GET` | `/api/v1/products/{id}` | Get product by ID |
| `PUT` | `/api/v1/products/{id}` | Update product |
| `DELETE` | `/api/v1/products/{id}` | Delete product |
| `GET` | `/api/v1/categories` | List all categories |
| `POST` | `/api/v1/categories` | Create a new category |

### 🏭 Inventory

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/inventory/{productId}` | Get inventory status |
| `POST` | `/api/v1/inventory/{productId}/adjust` | Adjust stock quantity |
| `POST` | `/api/v1/inventory/{productId}/reserve` | Reserve stock |
| `POST` | `/api/v1/inventory/{productId}/release` | Release reservation |

### 🛒 Cart

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/cart` | View current cart |
| `POST` | `/api/v1/cart/items` | Add item to cart |
| `PUT` | `/api/v1/cart/items/{productId}` | Update item quantity |
| `DELETE` | `/api/v1/cart/items/{productId}` | Remove item |
| `DELETE` | `/api/v1/cart` | Clear cart |

### 📋 Orders

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/orders` | Place a new order |
| `GET` | `/api/v1/orders` | List user orders |
| `GET` | `/api/v1/orders/{id}` | Get order details |
| `POST` | `/api/v1/orders/{id}/cancel` | Cancel an order |

### 💳 Payments

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/payments` | Process payment for an order |
| `GET` | `/api/v1/payments/{orderId}` | Get payment status |

### 🔔 Notifications

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/notifications` | Get user notifications |

### ❤️ Health Checks

| Endpoint | Description |
|---|---|
| `/health` | Full health check |
| `/health/live` | Liveness probe |
| `/health/ready` | Readiness probe |

---

## 📡 Event Flow

```
User places order
       │
       ▼
 OrderCreatedIntegrationEvent
       │
       ├──► OrderCreatedIntegrationEventHandler
       │         └── Reserve stock in inventory
       │
       └──► OrderCreatedNotificationConsumer
                 └── Send order confirmation notification

User payment processed
       │
       ▼
 PaymentSucceededIntegrationEvent ──► PaymentNotificationConsumer
 PaymentFailedIntegrationEvent    ──► PaymentNotificationConsumer
```

---

## 🛡️ Reliability Patterns

| Pattern | Implementation | Purpose |
|---|---|---|
| **Outbox Pattern** | `OutboxMessage` | Atomic DB + event publishing — zero event loss |
| **Inbox Pattern** | `InboxMessage` | Idempotent consumer deduplication |
| **Retry with Backoff** | `ResilientConsumer` | Exponential Backoff + Jitter on failures |
| **Dead-Letter Storage** | `DeadLetterMessage` | Route exhausted messages for inspection |
| **Idempotency Keys** | `Payment.IdempotencyKey` | Prevent duplicate payment charges |
| **Optimistic Concurrency** | `InventoryItem.Version` | Prevent overselling under concurrent load |

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific suites
dotnet test tests/ECommerce.UnitTests
dotnet test tests/ECommerce.IntegrationTests
dotnet test tests/ECommerce.ArchitectureTests
```

| Suite | Tests | Coverage |
|---|---|---|
| Unit Tests | **76** | Domain, Services, Middleware |
| Integration Tests | **2** | E2E workflow, health checks |
| Architecture Tests | **3** | Layer dependency enforcement |
| **Total** | **81** | **0 failures** |

---

## ⚙️ CI/CD Pipeline

Every push and pull request to `master`/`main` triggers:

```
Checkout
    │
    ▼
Setup .NET 10
    │
    ▼
dotnet restore  →  dotnet build -c Release
    │
    ▼
dotnet test (81 tests)
    │
    ▼
docker build (verify container builds)
```

---

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 10.0 (preview) | Target framework |
| ASP.NET Core | 10.0 | HTTP API & middleware |
| Entity Framework Core | 9.0 | ORM |
| PostgreSQL / Npgsql | 9.0 | Primary database |
| JWT Bearer | 9.0 | Authentication |
| xUnit | 2.9 | Test framework |
| FluentAssertions | 7.2 | Test assertions |
| NetArchTest | 1.3 | Architecture tests |
| Docker Compose | v3.8 | Container orchestration |
| GitHub Actions | — | CI/CD |

---

## 📖 Documentation

| Document | Description |
|---|---|
| [`docs/ADR.md`](docs/ADR.md) | Architecture Decision Records |
| [`docs/Architecture.md`](docs/Architecture.md) | Detailed architecture documentation |
| [`docs/phases.md`](docs/phases.md) | Project phases & completion status |

---

## 📄 License

MIT © [Ali Gohar](https://github.com/AliGohar2151)
