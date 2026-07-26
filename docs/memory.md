# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 2 — Database & Persistence (Completed)
**Next Phase:** Phase 3 — Authentication & Authorization

---

## 1. Overview of Progress

### Phase 0 — Project Foundation (COMPLETED)
- Initialized Git repository.
- Created `.gitignore`, `Directory.Build.props` (targeting `.NET 10`), `Directory.Packages.props` (CPM), and `.editorconfig`.
- Created Clean Architecture solution (`ECommerce.sln` / `ECommerce.slnx`) with 5 `src` projects and 3 `tests` projects.
- Configured inward dependency directions according to Clean Architecture rules.
- Added native ASP.NET Core Health Checks registered at `/health`.

### Phase 1 — Domain Foundation (COMPLETED)
- Created domain primitives in `src/ECommerce.Domain/Primitives`: `IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `DomainException`.
- Created unit tests covering domain primitives and architecture boundary rules.

### Phase 2 — Database & Persistence (COMPLETED)
- Added EF Core and PostgreSQL packages (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`).
- Created foundational domain entities in `src/ECommerce.Domain/Entities`: `User`, `Role`, `Permission`, `Category`, `Product`.
- Created `ApplicationDbContext` and explicit `IEntityTypeConfiguration<T>` mappings in `src/ECommerce.Infrastructure/Persistence/Configurations`.
- Configured `ApplicationDbContext` registration with PostgreSQL provider in `src/ECommerce.Infrastructure/DependencyInjection.cs`.
- Configured EF Core DbContext Health Check (`AddDbContextCheck<ApplicationDbContext>()`) in `src/ECommerce.Api/Program.cs` and `ConnectionStrings:Database` in `appsettings.json`.
- Added unit tests in `tests/ECommerce.UnitTests/Infrastructure/DbContextTests.cs` verifying EF Core entity mapping and in-memory query execution.

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Domain Purity:** Domain entities contain ZERO EF Core annotations/attributes. All schema mappings are configured explicitly using EF Core `IEntityTypeConfiguration<T>` inside Infrastructure.
- **Persistence Provider:** PostgreSQL with Npgsql provider; in-memory EF Core provider used for unit testing.
- **Health Checks:** Native DbContext health check integrated with ASP.NET Core Health Checks subsystem.

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All Unit, Integration, and Architecture tests passing.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 3 — Authentication & Authorization**:
  - Implement User registration & Password hashing (ASP.NET Core Identity PasswordHasher or BCrypt / Argon2).
  - Implement JWT access token generation & validation middleware.
  - Implement Refresh tokens with rotation and revocation.
  - Implement Role-based access control (RBAC) and Permission-based authorization policies.
  - Add authentication and authorization unit & integration tests.
