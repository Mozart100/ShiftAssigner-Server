# Shift Assigner Backend & BDD Testing Instructions

**IMPORTANT:** Copilot must keep all answers short and to the point.
Use this instruction with GitHub Copilot Chat inside VS Code.

---

## Overview

You are helping me implement a **C# backend** for a **multi-company shift-assignment system** using **ASP.NET Core Web API**, and helping me write **BDD tests** using **Reqnroll 3.2** and **xUnit**.

---

# Backend Requirements

## Business Rules

* System supports **multiple companies** (multi-tenant).
* Each company has its own **separate shift data**.
* Workers schedule preferred shifts (**Morning / Day / Evening**) for the **upcoming week only**.
* Each day must show all shift slots and indicate **available vs filled** slots.
* **TenantBoss registration requires ShiftConfig** - When registering a new tenant/company, the boss must provide initial shift configuration defining available shifts, worker requirements, and scheduling rules.

---

## Backend Code to Generate

### Domain / Entity Classes

* Company
* Worker (belongs to Company)
* ShiftDefinition (Morning / Day / Evening)
* ShiftAssignment (Worker + Shift + Date + Status)

### DbContext

* Clean DbContext
* Includes all required `DbSet<>` items.

### Guidelines

* Use **.NET 8 patterns**.
* Clear naming and comments.
* Enforce **tenant isolation**.
* Show **shift-availability** logic.

---

# BDD (Reqnroll 3.2 + xUnit)

## BDD Artifacts to Generate

1. **Feature Scenarios**

   * Worker books Morning shift.
   * Worker cannot book overlapping shifts.
   * Worker from Company A cannot access Company B.
   * Shift at full capacity rejects new bookings.

2. **Step Definitions**

   * Use attributes: `[Binding]`, `[Given]`, `[When]`, `[Then]`.
   * Use **WebApplicationFactory<Program>** as the in-memory test server.
   * Steps focus strictly on **behavior**.

3. **Reqnroll Setup**

   * Provide example `reqnroll.json`.
   * Folder structure: `Features/`, `Steps/`, `Hooks/`.
   * `[BeforeScenario]` resets database/state.

### Coding Guidelines

* Use modern C# syntax.
* Add comments explaining:

  * tenant isolation
  * shift capacity
  * availability checks
* Keep structure clean and modular.

---

# Expected Output from Copilot

Copilot should generate:

* Backend models
* DbContext
* API structures (if needed)
* Feature files
* Step definitions
* Reqnroll config
* All code short, clean, and following .NET 8 style.

---

# Additional Rules

### 1. Terminology Rule

When I write **"leader"**, I always mean **Shift Leader**. Copilot must always interpret "leader" as **Shift Leader**.

### 2. ScenarioContext Key Rule

In BDD tests, **every ScenarioContext key must be stored in a `const string` variable**.

Example:

```csharp
private const string WorkerKey = "WorkerKey";
ScenarioContext[WorkerKey] = worker;
```

### 3. Soft Delete Rule

All models use **`IsActive`** property for soft delete. **Never physically delete records**.

- All entities have `IsActive` boolean property (default: `true`)
- "Delete" operations set `IsActive = false` using `UpdateAsync`
- All queries filter by `IsActive = true` to exclude deleted records
- RemoveAsync/Delete methods are **not used**

Example:

```csharp
// Soft delete - mark as inactive
await _repository.UpdateAsync(
    x => x.ID == id && x.IsActive,
    entity => entity.IsActive = false);

// Query only active records
var activeWorkers = await _repository.GetAllAsync(x => x.IsActive);
```

### 4. Validation Rule

Use **FluentValidation** for all request validation.

- Create validators inheriting from `AbstractValidator<T>`
- Business rules in dedicated validation services inheriting from `ServiceValidatorBase`
- Validation errors throw `ShiftAssignmentException`
- Middleware catches exceptions and returns structured error responses

Example:

```csharp
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.ID).NotEmpty().MinimumLength(3);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
    }
}
```

### 5. Repository Pattern & UnitOfWork

Using **Entity Framework repositories with UnitOfWork pattern**:

- `IRepositoryBase<TModel>` interface with CRUD operations  
- `BaseRepository<TModel>` EF implementation
- `ITenantUnitOfWork` aggregates all repositories
- Scoped lifetime for repositories and UnitOfWork

**CRITICAL SaveChanges Rule:**
- **NEVER call `SaveChanges()` or `SaveChangesAsync()` in services**
- **AutoSaveMiddleware automatically saves all pending changes** at the end of each request
- Services should only call repository methods (`InsertAsync`, `UpdateAsync`, etc.)
- UnitOfWork SaveChanges is **only** used by AutoSaveMiddleware

✅ **Correct pattern:**
```csharp
// In service - NO SaveChanges
await _unitOfWork.Workers.InsertAsync(worker);
// AutoSaveMiddleware will save automatically
```

❌ **Incorrect pattern:**
```csharp
// NEVER do this in services
await _unitOfWork.Workers.InsertAsync(worker);
await _unitOfWork.SaveChangesAsync(); // ❌ WRONG!
```

Example repository:

```csharp
public interface IWorkerRepository : IRepositoryBase<Worker> { }
public class WorkerRepository : BaseRepository<Worker>, IWorkerRepository { }
```

### 6. BDD Step Organization Rule

When creating or modifying BDD features, **common step methods must be automatically organized into shared directories**:

- **Given steps used by multiple features** → `Steps/Given/GivenCommonSteps.cs`
- **When steps used by multiple features** → `Steps/When/WhenCommonSteps.cs`  
- **Then steps used by multiple features** → `Steps/Then/ThenCommonSteps.cs`

**Feature-specific steps** stay in their dedicated step files (e.g., `WorkerRegistrationValidationSteps.cs`).

**Constants organization:**
- **Multi-tenant constants** → `TwoTenantsStep` base class
- **Single-tenant constants** → `SingleTenantStep` base class
- No duplicate constants across step files

Example structure:
```csharp
// Common step used by multiple features
[Given("I have tenant registrations for {string} and {string}")]
public void GivenIHaveTenantRegistrationsFor(string tenantA, string tenantB) // → GivenCommonSteps.cs

// Feature-specific step
[When("I register a worker with valid data")]  
public async Task WhenIRegisterAWorkerWithValidData(Table table) // → WorkerRegistrationValidationSteps.cs
```

### 7. Test Execution Verification Rule

**CRITICAL:** When creating or refactoring tests, **ALWAYS run and verify they are executable**:

- **After creating new features** → Run `dotnet build` and `dotnet test` 
- **After modifying step definitions** → Verify no ambiguous bindings exist
- **After refactoring** → Ensure all tests still pass
- **Before completing task** → Confirm test suite is fully runnable

**Test verification checklist:**
1. ✅ Build compiles without errors
2. ✅ No ambiguous step definitions 
3. ✅ All tests discovered correctly (`dotnet test --list-tests`)
4. ✅ Test suite runs without binding errors
5. ✅ Existing functionality remains unbroken

**Never leave tests in a non-runnable state.** If conflicts arise, resolve them immediately.

### 8. Authentication & Authorization

- JWT tokens for authentication
- `JwtService` generates tokens with user ID, role, and tenant
- No role-based authorization implemented yet

### 9. Error Handling

- `ErrorHandlingMiddleware` catches all unhandled exceptions
- `ShiftAssignmentException` for validation errors (HTTP 400)
- Generic exceptions return HTTP 500 with error ID
- Development mode includes stack traces

---

# Current Architecture Summary

## Models
- **PersonBase** (abstract): Base for Worker, ShiftLeader, BossTenant
- **Worker**: Regular worker assigned to shift leaders
- **ShiftLeader**: Manages workers, belongs to tenant
- **BossTenant**: Company admin, creates shift leaders
- **Tenant**: Company/organization
- **StuffBooking**: Worker-to-ShiftLeader assignment for periods
- **ShiftConfig**: Defines company's shift patterns, worker requirements, and scheduling rules

## Services
- **WorkerService**: Worker management and retirement
- **ShiftLeaderService**: Shift leader operations
- **TenantService**: Tenant management and ShiftConfig creation during registration
- **StuffBookingService**: Worker assignments and reassignments
- **RegistrationValidationService**: Registration request validation

## Controllers
- **AuthController**: Registration endpoints (worker, shift leader, tenant with ShiftConfig - only tenant registration includes ShiftConfig)
- **WorkersController**: Worker queries and retirement
- **ShiftLeadersController**: Shift leader operations
- **TenantsController**: Tenant operations
- **StuffBookingsController**: Assignment and reassignment endpoints

## Key Features
- ✅ Multi-tenant isolation
- ✅ Soft delete pattern
- ✅ Worker reassignment between shift leaders
- ✅ Worker retirement
- ✅ FluentValidation integration
- ✅ Comprehensive BDD test coverage
- ✅ JWT authentication
- ✅ Error handling middleware
- ✅ **TenantBoss registration with mandatory ShiftConfig**
