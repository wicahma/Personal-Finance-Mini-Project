# PersonalFinance – Copilot Instructions

## Architecture Overview

Four-layer Clean Architecture with a Blazor Auto (SSR + WASM) front-end:

```
PersonalFinance.Domain        → Entities, Enums, Repository interfaces, IAuditableEntity, ISoftDelete
PersonalFinance.Application   → IXService interfaces, DTOs (records), ApiResponse<T>
PersonalFinance.Infrastructure → EF Core DbContext, Repository & Service implementations, AddInfrastructure()
PersonalFinanceWeb/
  PersonalFinanceWeb          → ASP.NET Core host: Blazor SSR, REST API controllers, Identity, Server*ClientService
  PersonalFinanceWeb.Client   → Blazor WASM: pages, Http*ClientService, client-side models
```

## Critical Pattern: Dual-Mode Client Services

Every feature has **one interface**, **two implementations** — this is the project's most distinctive pattern.

| Location                              | Class                    | How it works                           |
| ------------------------------------- | ------------------------ | -------------------------------------- |
| `PersonalFinanceWeb.Client/Services/` | `ITagClientService`      | Shared interface                       |
| `PersonalFinanceWeb/Services/`        | `ServerTagClientService` | Calls `ITagService` directly (no HTTP) |
| `PersonalFinanceWeb.Client/Services/` | `HttpTagClientService`   | Calls REST API via `HttpClient`        |

Server `Program.cs` registers `Server*` implementations; WASM `Program.cs` registers `Http*` implementations. When adding a new feature, you must create **both** implementations.

## User Identity Flow

`ApplicationUser` (ASP.NET Identity) → `UserProfile` (domain entity, scoped to all data).

- Controllers call `await GetUserProfileIdAsync(_profileService, ct)` — defined in `BaseApiController`.
- Server client services call `_profileService.EnsureCreatedAsync(userId, ct)` via `IHttpContextAccessor`.
- Every domain entity has a `UserProfileId` (Guid) FK — never query without scoping to it.

## API Response Envelope

All `/api/*` endpoints return `ApiResponse<T>` (`Status`, `Message`, `Data`, optional `Pagination`).  
HTTP client services always check `response?.Status == true` before accessing `.Data`.

```csharp
// Controller
return OkData(tags);                          // wraps in ApiResponse<T>.Success(...)
return NotFound(ApiResponse<object>.Failure("Tag not found."));

// Http client
var response = await http.GetFromJsonAsync<ApiResponse<List<TagManagementModel>>>("api/tags", ct);
return response?.Status == true && response.Data is not null ? response.Data : Array.Empty<TagManagementModel>();
```

## Domain Conventions

- All entities implement `IAuditableEntity` (`CreatedAt`, `UpdatedAt`, `CreatedBy`) and `ISoftDelete` (`IsDeleted`).
- Primary keys are `Guid`, initialized in the entity: `public Guid Id { get; set; } = Guid.NewGuid();`.
- DTOs in the Application layer are `record` types, e.g., `record CreateTagDto(Guid UserProfileId, string Name, string Color)`.
- Client-side view models live in `PersonalFinanceWeb.Client/Models/` — separate from Application DTOs.

## Adding a New Feature (checklist)

1. **Domain**: Add entity (implement `IAuditableEntity`, `ISoftDelete`), add repository interface in `Domain/Abstractions/`.
2. **Application**: Add service interface (`IXService`) + DTOs in `Application/Services/` and `Application/DTOs/`.
3. **Infrastructure**: Implement repository and service; register both in `InfrastructureServiceExtensions.AddInfrastructure()`.
4. **API Controller**: Inherit `BaseApiController`, add `[Authorize]`, call `GetUserProfileIdAsync()`.
5. **Client interface**: Add `IXClientService` in `PersonalFinanceWeb.Client/Services/`.
6. **Server client service**: Add `ServerXClientService` in `PersonalFinanceWeb/Services/`; register in server `Program.cs`.
7. **HTTP client service**: Add `HttpXClientService` in `PersonalFinanceWeb.Client/Services/`; register in WASM `Program.cs`.
8. **EF Migration**: `cd PersonalFinance.Infrastructure && dotnet ef migrations add <Name> --startup-project ../PersonalFinanceWeb/PersonalFinanceWeb`

## Developer Workflow

```powershell
# Run the application (from repo root)
cd PersonalFinanceWeb/PersonalFinanceWeb && dotnet run

# Add EF migration (from repo root)
dotnet ef migrations add <Name> `
  --project PersonalFinance.Infrastructure `
  --startup-project PersonalFinanceWeb/PersonalFinanceWeb

# Rebuild Tailwind CSS
tools/download-tailwind.ps1
```

## Key Files

- [InfrastructureServiceExtensions.cs](PersonalFinance.Infrastructure/InfrastructureServiceExtensions.cs) — all DI registrations for Infrastructure
- [BaseApiController.cs](PersonalFinanceWeb/PersonalFinanceWeb/Controllers/BaseApiController.cs) — `GetUserId()`, `GetUserProfileIdAsync()`, `OkData()`, `CreatedData()`
- [ApiResponse.cs](PersonalFinance.Application/Common/ApiResponse.cs) — response envelope used everywhere
- Server `Program.cs`: [PersonalFinanceWeb/PersonalFinanceWeb/Program.cs](PersonalFinanceWeb/PersonalFinanceWeb/Program.cs)
- WASM `Program.cs`: [PersonalFinanceWeb/PersonalFinanceWeb.Client/Program.cs](PersonalFinanceWeb/PersonalFinanceWeb.Client/Program.cs)
