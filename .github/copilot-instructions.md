# Copilot Instructions – PersonalFinance

## Architecture Overview

Clean Architecture with four layers:

| Project | Role |
|---|---|
| `PersonalFinance.Domain` | Entities, enums, repository interfaces, `IAuditableEntity` / `ISoftDelete` |
| `PersonalFinance.Application` | Service interfaces, DTOs, `ApiResponse<T>` |
| `PersonalFinance.Infrastructure` | EF Core, SQL Server, repository + service implementations |
| `PersonalFinanceWeb/PersonalFinanceWeb` | Blazor **Auto** host: API controllers, server-side service impls, Identity |
| `PersonalFinanceWeb/PersonalFinanceWeb.Client` | Blazor WASM: pages, HTTP client service impls, client-side models |

## Dual-Mode Service Pattern (Critical)

Blazor Auto renders components on the server first, then re-hydrates in WASM. Every feature therefore has **two service implementations** behind a shared `IXxxClientService` interface:

- `Server*ClientService` (in `PersonalFinanceWeb/Services/`) — called during SSR; calls application services directly (no HTTP round-trip).
- `Http*ClientService` (in `PersonalFinanceWeb.Client/Services/`) — called from WASM; hits the REST API via `HttpClient`.

Both are registered against the same interface (server in `Program.cs`, client in `PersonalFinanceWeb.Client/Program.cs`). When adding a new feature, implement both.

## API Response Contract

All `/api/*` endpoints return `ApiResponse<T>` (`PersonalFinance.Application.Common.ApiResponse<T>`):
```json
{ "status": true, "message": "Success", "data": {...}, "pagination": null }
```
Use the `BaseApiController` helpers: `OkData<T>()`, `CreatedData<T>()`. API cookie auth returns 401/403 JSON (not redirect) when the path starts with `/api`.

## User Scoping

All domain data is keyed to `UserProfile.Id` (a `Guid`), not to the ASP.NET Identity `ApplicationUser.Id` (a `string`). Every controller action calls `GetUserProfileIdAsync(_profileService, ct)` (from `BaseApiController`) to resolve the profile Guid. `IUserProfileService.EnsureCreatedAsync` lazily creates a profile on first use.

## Entity Conventions

- All entities implement `IAuditableEntity` (`CreatedAt`, `UpdatedAt`, `CreatedBy`) and `ISoftDelete` (`IsDeleted`).
- EF Core **global query filters** on every entity exclude soft-deleted rows automatically — never filter manually on `IsDeleted`.
- Enums (`AccountType`, `CategoryType`, `TransactionType`) are stored as **strings** in the database (`.HasConversion<string>()`).
- `DbContextFactory<FinanceDbContext>` is registered as `Scoped` (not singleton).
- Migrations history table: `__FinanceMigrationsHistory`. Run migrations from `PersonalFinanceWeb/PersonalFinanceWeb`.

## Client-Side Models vs Application DTOs

WASM pages use types in `PersonalFinanceWeb.Client/Models/` (e.g. `TransactionListItem`, `TransactionDetailModel`). These differ from `PersonalFinance.Application.DTOs`. The `Server*ClientService` classes are responsible for mapping between them.

## Transfer Transactions

A bank transfer creates **two linked `Transaction` records** both with `IsTransfer = true` and `TransferPairId` pointing to each other. Use `ITransactionService.CreateTransferAsync` and handle the pair when deleting or editing.

## Styling

Dark neobrutalism theme with Tailwind CSS. The Tailwind binary is bundled at `tools/tailwindcss-macos` (macOS) and is invoked **automatically by MSBuild before every build** — no separate CSS watch process needed. Key custom tokens defined in `tailwind.config.cjs`:
- `neon` (#39FF14), `vivid-yellow` (#FFE600), `hot-pink` (#FF2D78)
- `brutal-green/yellow/pink` box-shadow utilities
- Dark base: `base-bg` / `base-surface` / `base-border`

## Developer Workflows

```bash
# Run the app (from solution root)
cd PersonalFinanceWeb/PersonalFinanceWeb && dotnet run

# Add EF migration
cd PersonalFinanceWeb/PersonalFinanceWeb
dotnet ef migrations add <Name> --project ../../PersonalFinance.Infrastructure

# Apply migrations
dotnet ef database update --project ../../PersonalFinance.Infrastructure

# Configuration (copy and fill in connection string + SMTP)
cp appsettings.Example.json appsettings.json
# Then add "DefaultConnection" via user-secrets or appsettings.json
```

## Key Files

- [`PersonalFinance.Infrastructure/InfrastructureServiceExtensions.cs`](../PersonalFinance.Infrastructure/InfrastructureServiceExtensions.cs) — all DI registrations for infra layer
- [`PersonalFinanceWeb/PersonalFinanceWeb/Controllers/BaseApiController.cs`](../PersonalFinanceWeb/PersonalFinanceWeb/Controllers/BaseApiController.cs) — shared controller base
- [`PersonalFinance.Infrastructure/Persistence/FinanceDbContext.cs`](../PersonalFinance.Infrastructure/Persistence/FinanceDbContext.cs) — EF model configuration & query filters
- [`PersonalFinanceWeb/PersonalFinanceWeb/tailwind.config.cjs`](../PersonalFinanceWeb/PersonalFinanceWeb/tailwind.config.cjs) — design token definitions
