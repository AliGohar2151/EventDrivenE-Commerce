# System Design & Developer Experience

# Event-Driven E-Commerce Backend

**Project:** Event-Driven E-Commerce Backend
**Target Framework:** .NET 10
**Language:** C#
**Architecture:** Clean Architecture + Modular Monolith + Event-Driven Architecture
**Status:** Planned
**Version:** 1.0

---

# 1. Purpose

This document defines the design standards for the Event-Driven E-Commerce Backend.

Since the project is a backend-focused system, this document focuses on:

- REST API design
- API response conventions
- Error response design
- Endpoint naming
- DTO design
- Pagination
- Filtering
- Sorting
- Validation
- Event naming
- Messaging conventions
- Database naming
- Logging conventions
- Observability
- OpenAPI documentation
- Developer experience

The objective is to make the backend predictable and easy to consume.

---

# 2. Design Philosophy

The system should be designed around the following principles:

```text
Simple
    ↓
Consistent
    ↓
Predictable
    ↓
Explicit
    ↓
Discoverable
```

A developer using the API should not need to guess:

- How endpoints are named.
- How errors are returned.
- How pagination works.
- How validation errors are represented.
- How authentication works.
- How events are named.
- How correlation IDs are tracked.

Consistency is more important than personal preference.

---

# 3. API Base URL

The API should use versioned routes.

Preferred:

```text
/api/v1
```

Examples:

```text
/api/v1/auth
/api/v1/products
/api/v1/categories
/api/v1/cart
/api/v1/orders
/api/v1/users
```

Future breaking API changes should use a new version.

Example:

```text
/api/v1/products
/api/v2/products
```

Do not introduce API versions unnecessarily.

---

# 4. Resource Naming

API resources must use plural nouns.

Preferred:

```text
/products
/orders
/users
/categories
```

Avoid:

```text
/getProducts
/createOrder
/getUser
```

The HTTP method communicates the operation.

---

# 5. HTTP Methods

Use HTTP methods according to their semantic purpose.

## GET

Retrieve resources.

```text
GET /api/v1/products
GET /api/v1/products/{id}
```

## POST

Create resources or initiate actions that are not naturally represented by CRUD.

```text
POST /api/v1/products
POST /api/v1/orders
POST /api/v1/auth/login
```

## PUT

Replace a resource when full replacement semantics are appropriate.

```text
PUT /api/v1/products/{id}
```

## PATCH

Partially update a resource when appropriate.

```text
PATCH /api/v1/products/{id}
```

## DELETE

Delete or deactivate a resource.

```text
DELETE /api/v1/products/{id}
```

---

# 6. HTTP Status Codes

Use standard HTTP status codes.

## 200 OK

Successful request.

```text
GET /products/{id}
```

## 201 Created

Resource successfully created.

```text
POST /products
```

Include a `Location` header where appropriate.

## 202 Accepted

Request accepted for asynchronous processing.

Example:

```text
POST /orders
```

may return `202 Accepted` if the system intentionally processes order creation asynchronously.

The status code must accurately represent the actual behavior.

## 204 No Content

Successful request with no response body.

Example:

```text
DELETE /products/{id}
```

## 400 Bad Request

Invalid request structure or malformed input.

## 401 Unauthorized

Authentication is missing or invalid.

## 403 Forbidden

The user is authenticated but lacks permission.

## 404 Not Found

Requested resource does not exist.

## 409 Conflict

Request conflicts with current system state.

Examples:

- Duplicate SKU.
- Inventory conflict.
- Invalid state transition.

## 422 Unprocessable Entity

The request structure is valid, but business validation fails.

Use only if the project consistently adopts this convention.

## 429 Too Many Requests

Rate limit exceeded.

## 500 Internal Server Error

Unexpected server error.

Never expose internal implementation details.

---

# 7. Standard Success Response

The API should use a consistent response envelope where appropriate.

Example:

```json
{
  "success": true,
  "data": {
    "id": "product-id",
    "name": "Example Product"
  },
  "message": null,
  "errors": []
}
```

The response structure is:

```text
success
data
message
errors
```

---

# 8. Standard Error Response

Example:

```json
{
  "success": false,
  "data": null,
  "message": "The request could not be processed.",
  "errors": [
    {
      "code": "PRODUCT_NOT_FOUND",
      "field": null,
      "message": "The requested product was not found."
    }
  ]
}
```

Errors should provide useful information without exposing internal implementation details.

---

# 9. Validation Error Design

Validation errors should identify the affected field.

Example:

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed.",
  "errors": [
    {
      "code": "REQUIRED",
      "field": "name",
      "message": "Product name is required."
    },
    {
      "code": "INVALID_PRICE",
      "field": "price",
      "message": "Price must be greater than zero."
    }
  ]
}
```

Multiple validation errors may be returned together.

---

# 10. Error Codes

Errors should use stable machine-readable codes.

Examples:

```text
USER_NOT_FOUND
INVALID_CREDENTIALS
ACCOUNT_DISABLED
PRODUCT_NOT_FOUND
PRODUCT_ALREADY_EXISTS
CATEGORY_NOT_FOUND
INSUFFICIENT_INVENTORY
ORDER_NOT_FOUND
INVALID_ORDER_STATE
PAYMENT_FAILED
PAYMENT_ALREADY_PROCESSED
EVENT_PROCESSING_FAILED
```

Error codes should remain stable even if human-readable messages change.

---

# 11. Error Handling Architecture

All unexpected errors must pass through centralized exception handling.

Flow:

```text
Request
   │
   ▼
Controller
   │
   ▼
Application
   │
   ▼
Exception
   │
   ▼
Global Exception Handler
   │
   ▼
Structured Error Response
```

Expected business errors should be represented intentionally.

Unexpected errors should return a generic response.

Example:

```json
{
  "success": false,
  "data": null,
  "message": "An unexpected error occurred.",
  "errors": []
}
```

The actual exception must be logged internally.

---

# 12. Correlation ID

Every incoming request should have a correlation ID.

If the client provides one:

```text
X-Correlation-ID
```

the system may preserve it after validation.

If no correlation ID is provided, the API should generate one.

The correlation ID must flow through:

```text
HTTP Request
    │
    ▼
Application
    │
    ▼
Database Logs
    │
    ▼
Integration Event
    │
    ▼
RabbitMQ
    │
    ▼
Consumer
```

Example:

```text
Correlation ID: 8c12f3e4-...
```

This allows a complete business workflow to be traced.

---

# 13. Request ID vs Correlation ID

These concepts should remain distinct.

## Request ID

Identifies a single HTTP request.

## Correlation ID

Identifies a larger business workflow across multiple components.

Example:

```text
Correlation ID: ORDER-123

Request ID: A
    │
    ▼
Create Order

Request ID: B
    │
    ▼
Payment Callback

Event ID: C
    │
    ▼
Payment Consumer

Event ID: D
    │
    ▼
Notification Consumer
```

All may share the same correlation ID.

---

# 14. Pagination Design

Large collections must support pagination.

Preferred query parameters:

```text
?page=1&pageSize=20
```

Example:

```text
GET /api/v1/products?page=1&pageSize=20
```

Response:

```json
{
  "success": true,
  "data": {
    "items": [],
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 100,
      "totalPages": 5,
      "hasNextPage": true,
      "hasPreviousPage": false
    }
  },
  "message": null,
  "errors": []
}
```

The API should enforce reasonable maximum page sizes.

For example:

```text
Default page size: 20
Maximum page size: 100
```

Exact limits may be configured centrally.

---

# 15. Filtering

Filtering should use query parameters.

Example:

```text
GET /api/v1/products?category=electronics
```

Multiple filters:

```text
GET /api/v1/products
    ?category=electronics
    &minPrice=100
    &maxPrice=1000
```

Filters must be validated.

Avoid exposing raw database query syntax through API parameters.

---

# 16. Sorting

Sorting should use explicit parameters.

Example:

```text
GET /api/v1/products?sortBy=price&sortDirection=asc
```

Allowed sort fields should be explicitly defined.

The API must not allow arbitrary property names to become database queries.

---

# 17. Search

Search should use a dedicated query parameter.

Example:

```text
GET /api/v1/products?search=iphone
```

Search behavior should be documented.

Search should be implemented efficiently.

Avoid loading all products into application memory and filtering them in C#.

Filtering and searching should be pushed to the database where appropriate.

---

# 18. DTO Design

DTOs should be purpose-specific.

Examples:

```text
CreateProductRequest
UpdateProductRequest
ProductResponse
ProductListItemResponse
CreateOrderRequest
OrderResponse
```

Avoid one giant DTO used for:

- Create
- Update
- Read
- Internal processing

DTOs should represent the needs of their consumers.

---

# 19. Domain Model Exposure

Domain entities must never be returned directly from API controllers.

Avoid:

```csharp
return Ok(order);
```

if `order` is a domain entity.

Prefer:

```csharp
return Ok(new OrderResponse(...));
```

This protects the domain model from accidental API contract coupling.

---

# 20. Authentication API Design

Authentication endpoints should follow:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
```

Example login request:

```json
{
  "email": "user@example.com",
  "password": "secure-password"
}
```

Example response:

```json
{
  "success": true,
  "data": {
    "accessToken": "...",
    "refreshToken": "...",
    "expiresAt": "2026-07-26T12:00:00Z"
  },
  "message": null,
  "errors": []
}
```

Tokens must never be logged.

---

# 21. Product API Design

Preferred endpoints:

```text
GET    /api/v1/products
GET    /api/v1/products/{id}
POST   /api/v1/products
PUT    /api/v1/products/{id}
DELETE /api/v1/products/{id}
```

Category endpoints:

```text
GET    /api/v1/categories
GET    /api/v1/categories/{id}
POST   /api/v1/categories
PUT    /api/v1/categories/{id}
DELETE /api/v1/categories/{id}
```

Administrative operations must require appropriate permissions.

---

# 22. Cart API Design

Preferred endpoints:

```text
GET    /api/v1/cart
POST   /api/v1/cart/items
PATCH  /api/v1/cart/items/{productId}
DELETE /api/v1/cart/items/{productId}
DELETE /api/v1/cart
```

The cart belongs to the authenticated user.

The API must not accept a customer ID from the client to determine whose cart is being accessed.

The authenticated identity should determine ownership.

---

# 23. Order API Design

Preferred endpoints:

```text
POST /api/v1/orders
GET  /api/v1/orders
GET  /api/v1/orders/{id}
POST /api/v1/orders/{id}/cancel
```

Administrative endpoints may include:

```text
GET   /api/v1/admin/orders
PATCH /api/v1/admin/orders/{id}/status
```

The API must enforce authorization.

A customer must only access their own orders.

---

# 24. Order State Design

Order states must be explicit.

Example:

```text
Pending
PaymentProcessing
Paid
Processing
Shipped
Delivered
Cancelled
```

State transitions must be validated.

Example:

```text
Pending
   │
   ├── PaymentProcessing
   │
   └── Cancelled

PaymentProcessing
   │
   ├── Paid
   │
   └── Cancelled

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

Invalid transitions must return a meaningful business error.

---

# 25. Event Naming

Events represent facts.

Use past tense.

Preferred:

```text
OrderCreated
PaymentSucceeded
PaymentFailed
InventoryReserved
InventoryReservationFailed
OrderCancelled
OrderShipped
OrderDelivered
```

Avoid:

```text
CreateOrderEvent
ProcessPaymentEvent
ReserveInventoryEvent
```

These names represent actions or commands rather than facts.

---

# 26. Event Envelope

Integration events should use a standard envelope.

Example:

```json
{
  "eventId": "event-id",
  "eventType": "OrderCreated",
  "occurredAt": "2026-07-26T12:00:00Z",
  "correlationId": "correlation-id",
  "causationId": "causation-id",
  "version": 1,
  "payload": {}
}
```

The exact implementation may use strongly typed C# contracts rather than raw JSON objects.

---

# 27. Event Naming Convention

RabbitMQ routing keys should follow a consistent convention.

Preferred:

```text
order.created
order.cancelled
payment.requested
payment.succeeded
payment.failed
inventory.reserved
inventory.reservation_failed
```

The naming strategy must be consistent across the entire messaging system.

---

# 28. Queue Naming

Queues should identify the consuming module.

Examples:

```text
orders.payment
orders.inventory
orders.notifications
```

For example:

```text
ecommerce.payment
ecommerce.inventory
ecommerce.notification
```

The final naming convention should be standardized before production messaging implementation.

---

# 29. Dead-Letter Naming

Dead-letter queues should clearly identify their source.

Example:

```text
ecommerce.payment.dlq
ecommerce.inventory.dlq
ecommerce.notification.dlq
```

Dead-letter records should be searchable by:

- Event ID
- Correlation ID
- Event type
- Failure reason

---

# 30. Idempotency Key Design

Operations requiring idempotency should use a unique key.

Example:

```text
Idempotency-Key: 7f4a2d...
```

For HTTP operations:

```text
POST /api/v1/orders
```

the client may provide an idempotency key.

The server should ensure repeated requests with the same key do not create duplicate business operations.

The exact implementation should be finalized during the reliability phase.

---

# 31. Database Naming

Use consistent naming.

Tables:

```text
Users
Roles
Permissions
Products
Categories
InventoryItems
Orders
OrderItems
Payments
```

Columns should use consistent naming conventions.

Preferred C#:

```csharp
CreatedAt
UpdatedAt
```

Database naming strategy should be consistent with EF Core configuration.

---

# 32. Audit Fields

Important persistent entities should consider:

```text
CreatedAt
UpdatedAt
CreatedBy
UpdatedBy
```

Not every entity necessarily requires all fields.

Audit requirements should be determined by business importance.

---

# 33. Date and Time

All persisted timestamps should use UTC.

Preferred:

```csharp
DateTimeOffset
```

or another explicitly UTC-aware representation.

Do not store server-local time.

API timestamps should use ISO 8601 format.

Example:

```text
2026-07-26T12:30:00Z
```

---

# 34. Money Representation

Monetary values should use `decimal`.

Avoid floating-point types for financial values.

Example:

```csharp
decimal price;
```

Currency should be explicitly represented where required.

Example:

```text
amount
currency
```

Potential representation:

```json
{
  "amount": 4999.99,
  "currency": "PKR"
}
```

The system should not assume that all prices are in the same currency if multi-currency support is introduced later.

---

# 35. Logging Design

Logs should be structured.

Preferred:

```text
OrderCreated
OrderId: 123
UserId: 456
CorrelationId: abc
```

Log levels:

### Trace

Very detailed diagnostic information.

### Debug

Development and troubleshooting information.

### Information

Normal important business operations.

### Warning

Unexpected but recoverable situations.

### Error

Operation failed.

### Critical

Application or infrastructure failure requiring immediate attention.

---

# 36. Business Logging

Important business events should be logged.

Examples:

```text
UserRegistered
UserLoggedIn
OrderCreated
PaymentSucceeded
PaymentFailed
InventoryReservationFailed
MessageMovedToDeadLetter
```

Avoid excessive logging for every trivial operation.

---

# 37. Sensitive Data Logging

Never log:

```text
Passwords
Access Tokens
Refresh Tokens
API Keys
Connection Strings
Payment Credentials
```

Avoid logging sensitive personal data unless necessary.

---

# 38. Health Check Design

Health checks should distinguish between:

```text
Liveness
Readiness
```

## Liveness

Answers:

> Is the application process alive?

## Readiness

Answers:

> Can the application handle requests successfully?

Readiness may verify:

```text
PostgreSQL
Redis
RabbitMQ
```

The exact dependency checks should be configured carefully to avoid making the entire API unavailable because of a non-critical dependency.

---

# 39. Metrics Design

Metrics should follow consistent names.

Potential metrics:

```text
http.server.request.duration
ecommerce.orders.created
ecommerce.orders.cancelled
ecommerce.payments.succeeded
ecommerce.payments.failed
ecommerce.inventory.reservation.failed
ecommerce.events.published
ecommerce.events.consumed
ecommerce.events.failed
ecommerce.events.retried
ecommerce.events.dead_lettered
```

Metric labels should have controlled cardinality.

Avoid labels containing:

- User IDs
- Order IDs
- Event IDs

as high-cardinality dimensions.

---

# 40. Distributed Tracing

Important operations should produce traces.

Example:

```text
Trace
│
├── HTTP POST /orders
│
├── Database Transaction
│
├── Outbox Insert
│
├── RabbitMQ Publish
│
├── Inventory Consumer
│
├── Inventory Database Operation
│
├── Payment Consumer
│
└── Notification Consumer
```

Trace context should propagate across asynchronous messaging where supported.

---

# 41. OpenAPI / Swagger Design

The API must provide OpenAPI documentation.

Every public endpoint should document:

- Summary
- Description
- Parameters
- Request body
- Response codes
- Response models
- Authentication requirements
- Validation errors

Example:

```text
POST /api/v1/orders

201 Created
400 Bad Request
401 Unauthorized
409 Conflict
500 Internal Server Error
```

---

# 42. API Documentation Examples

Important endpoints should include request and response examples.

Example:

```json
{
  "items": [
    {
      "productId": "product-123",
      "quantity": 2
    }
  ]
}
```

Responses should clearly communicate the resulting state.

---

# 43. API Versioning

Versioning should be introduced when there is a genuine breaking contract change.

Do not create:

```text
v1
v2
v3
```

without a real reason.

Non-breaking changes should generally be backward compatible.

Breaking changes require:

- New API version.
- Documentation update.
- Migration strategy.

---

# 44. Configuration Design

Configuration should be grouped by concern.

Example:

```text
Database
Redis
RabbitMQ
Jwt
OpenTelemetry
```

Example configuration structure:

```json
{
  "ConnectionStrings": {
    "Database": "..."
  },
  "Redis": {
    "ConnectionString": "..."
  },
  "RabbitMq": {
    "Host": "...",
    "Port": 5672
  },
  "Jwt": {
    "Issuer": "...",
    "Audience": "..."
  }
}
```

Secrets must not be committed to source control.

---

# 45. Environment Design

The system should support:

```text
Development
Testing
Production
```

Environment-specific configuration should be handled through standard .NET configuration mechanisms.

Do not hard-code environment-specific values.

---

# 46. Developer Experience

A new developer should be able to:

```text
Clone Repository
      │
      ▼
Run Docker Compose
      │
      ▼
Configure Environment
      │
      ▼
Run API
      │
      ▼
Open Swagger
      │
      ▼
Authenticate
      │
      ▼
Test API
```

The README must provide clear instructions for this workflow.

---

# 47. API Design Consistency

All API modules must follow the same conventions.

For example:

```text
Authentication
Products
Categories
Cart
Orders
Users
```

must all use consistent:

- Response structures.
- Error structures.
- Pagination.
- Authentication.
- Authorization.
- Naming.
- HTTP semantics.

A new module should not introduce its own response format without a strong reason.

---

# 48. Design Review Checklist

Before introducing a new endpoint, verify:

```text
[ ] Correct HTTP method
[ ] Correct resource naming
[ ] Correct API version
[ ] Authentication considered
[ ] Authorization considered
[ ] Input validation implemented
[ ] DTO defined
[ ] Response status codes defined
[ ] Error codes defined
[ ] Logging considered
[ ] Correlation ID available
[ ] OpenAPI documentation added
[ ] Tests added
```

---

# 49. Design Review for Events

Before introducing a new integration event, verify:

```text
[ ] Event represents a fact
[ ] Event name is in past tense
[ ] Event has unique EventId
[ ] Event has OccurredAt
[ ] CorrelationId considered
[ ] CausationId considered
[ ] Event version defined
[ ] Consumer identified
[ ] Retry behavior defined
[ ] Idempotency considered
[ ] Dead-letter behavior defined
[ ] Contract documented
```

---

# 50. Final Design Principle

The backend should feel like a coherent product rather than a collection of APIs.

Every part of the system should communicate clearly:

```text
API
  ↓
Application
  ↓
Domain
  ↓
Infrastructure
  ↓
External Systems
```

And every asynchronous workflow should communicate clearly:

```text
Command
  ↓
Business Operation
  ↓
Event
  ↓
Message Broker
  ↓
Consumer
  ↓
Side Effect
```

The design goal is to make the system:

```text
Easy to Understand
        +
Easy to Use
        +
Easy to Debug
        +
Easy to Test
        +
Easy to Extend
```

The system should prioritize consistency and clarity over unnecessary sophistication.
