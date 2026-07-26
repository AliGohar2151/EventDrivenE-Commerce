# Project Memory — Event-Driven E-Commerce Backend

**Last Updated:** 2026-07-26
**Current Phase:** Phase 3 — Authentication & Authorization (Completed)
**Next Phase:** Phase 4 — Product Catalog

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
- Configured liveness `/health` and readiness `/health/ready` probe endpoints.

### Phase 3 — Authentication & Authorization (COMPLETED)
- Added JWT Authentication & Security packages (`Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`, `Microsoft.IdentityModel.Tokens`).
- Created DTOs in `src/ECommerce.Contracts/Authentication`: `RegisterUserRequest`, `LoginRequest`, `RefreshTokenRequest`, `RevokeTokenRequest`, `AuthenticationResponse`, `UserResponse`.
- Extended domain entities (`User`, `Role`, `Permission`) and added `RefreshToken` entity (rotation & revocation), `UserRole`, `RolePermission`.
- Created Application interfaces (`IPasswordHasher`, `IJwtProvider`, `IAuthenticationService`) and `AuthenticationService` implementation.
- Implemented `JwtProvider` (access token generation with claims), `PasswordHasher` (PBKDF2/SHA256 secure hashing), and `PermissionAuthorizationHandler` + `HasPermissionAttribute` for permission-based policy enforcement.
- Created `AuthenticationController` in `src/ECommerce.Api/Controllers` exposing `/api/v1/auth/register`, `/login`, `/refresh`, `/revoke`, and `/me`.
- Added unit tests in `tests/ECommerce.UnitTests/Authentication`: `PasswordHasherTests.cs`, `JwtProviderTests.cs`, `AuthenticationServiceTests.cs`.

---

## 2. Key Architectural Decisions

- **Target Framework:** .NET 10 (`net10.0`).
- **Token Strategy:** Short-lived JWT Access Tokens (15 min) + Refresh Tokens (7 days) with sliding rotation and revocation tracking stored in EF Core database.
- **Password Security:** PBKDF2 with SHA-256 and salt per user using `Rfc2898DeriveBytes`.
- **Authorization:** Permission-based authorization model using custom `IAuthorizationPolicyProvider` and `IAuthorizationHandler` (`permission` claims).

---

## 3. Current State & Known Issues

- **Build Status:** Clean compilation under .NET 10 SDK.
- **Tests Status:** All Unit, Integration, and Architecture tests passing.
- **Known Issues:** None.

---

## 4. Next Recommended Task

- Proceed to **Phase 4 — Product Catalog**:
  - Implement Product & Category management use cases (Commands/Queries).
  - Implement Product variants, SKU, price, status.
  - Implement Product search, pagination, filtering, sorting.
  - API endpoints for product CRUD operations protected by permission policies.
