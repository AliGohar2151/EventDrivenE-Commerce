# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 4 — Product Catalog (Completed)
**Next Phase:** Phase 5 — Inventory Management

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
- API Controllers: `ProductsController` and `CategoriesController` with permission policy protection (`HasPermission("Products.Create")`, `Products.Update`, `Products.Delete`) and public catalog queries.
- Unit Tests: `ProductTests.cs` and `ProductServiceTests.cs`.

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Catalog Querying:** Offset-based pagination with `PagedListResponse<T>`, parameterized IQueryable filtering (`Search`, `CategoryId`, `MinPrice`, `MaxPrice`, `Status`), and dynamic sorting.
- **Product Variants:** Modeled as immutable `ValueObject` owned types in EF Core (`product_variants` table).
- **Domain Events:** `ProductCreatedDomainEvent` and `ProductUpdatedDomainEvent` recorded on Product aggregate root during lifecycle state changes.

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All Unit, Integration, and Architecture tests passing.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 5 — Inventory Management**:
  - Implement `InventoryItem` entity (`StockQuantity`, `AvailableQuantity`, `ReservedQuantity`).
  - Implement stock adjustment, stock reservation, stock release, and low-stock detection workflows.
  - Implement concurrency protection (optimistic concurrency with EF Core `xmin` / row versioning).
  - Business rule: Prevent overselling under high concurrency.
