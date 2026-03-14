# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

OpenMedSphere is a secure medical research collaboration platform built with .NET 10 and Aspire. The project is in early stages and serves as a personal learning experience focused on exploring cutting-edge technologies including quantum-safe encryption, clean architecture, and domain-driven design principles.

**Key Goals:**
- Secure processing and sharing of anonymized patient data
- HIPAA and GDPR compliance
- Hybrid quantum-safe encryption (ML-KEM-768 + X25519, ML-DSA-65 + ECDSA, AES-256-GCM) for E2E encrypted data sharing
- Self-hostable architecture using .NET Aspire
- Licensed under AGPL v3

## Architecture

The project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles with strict layer separation:

### Layer Structure
```
src/
├── Core/
│   ├── OpenMedSphere.Domain/          # Core business entities (PatientData, ResearchStudy, AnonymizationPolicy, Researcher, DataShare)
│   └── OpenMedSphere.Application/     # Use cases, domain orchestration, CQRS commands/queries
├── Infrastructure/
│   └── OpenMedSphere.Infrastructure/  # Database, external integrations, encryption services
└── Presentation/
    ├── OpenMedSphere.API/             # ASP.NET Core Web API (minimal endpoints)
    ├── OpenMedSphere.AppHost/         # .NET Aspire orchestration host
    └── OpenMedSphere.ServiceDefaults/ # Shared service configuration (telemetry, health checks)
tests/
├── OpenMedSphere.Domain.Tests/        # Domain entity and value object unit tests
└── OpenMedSphere.Application.Tests/   # Command handler unit tests with Moq
```

### Domain Layer Structure
The Domain layer contains the core business logic with a rich domain model:

```
OpenMedSphere.Domain/
├── Entities/
│   ├── PatientData.cs          # Patient data aggregate root
│   ├── ResearchStudy.cs        # Research study aggregate root
│   ├── AnonymizationPolicy.cs  # Anonymization policy aggregate root
│   ├── Researcher.cs           # Researcher aggregate root (identity + crypto keys)
│   ├── DataShare.cs            # E2E encrypted data share aggregate root
│   └── AuditLogEntry.cs        # Audit log entry entity
├── Events/
│   ├── PatientDataCreatedEvent.cs    # Raised on patient data creation
│   ├── PatientDataAnonymizedEvent.cs # Raised on anonymization
│   ├── ResearchStudyCreatedEvent.cs  # Raised on study creation
│   ├── ResearcherCreatedEvent.cs     # Raised on researcher registration
│   ├── PatientDataSharedEvent.cs     # Raised when data is shared
│   ├── DataShareAccessedEvent.cs     # Raised when share is accepted
│   └── DataShareRevokedEvent.cs      # Raised when share is revoked
├── ValueObjects/
│   ├── PatientIdentifier.cs    # Anonymized patient ID (record type)
│   ├── DateRange.cs            # Date range with validation (record type)
│   ├── StudyCode.cs            # Unique study code (record type)
│   ├── MedicalCode.cs          # Structured medical code (record type)
│   └── PublicKeySet.cs         # Hybrid PQC public keys (record type)
├── Enums/
│   ├── AnonymizationLevel.cs   # None, Basic, Standard, Advanced, Full
│   └── DataShareStatus.cs      # Pending, Accepted, Revoked, Expired
└── Primitives/
    ├── Entity.cs               # Base entity with ID and equality
    ├── AggregateRoot.cs        # Base aggregate with domain events
    ├── (ValueObject.cs removed — all value objects are C# records with structural equality)
    └── IDomainEvent.cs         # Domain event marker interface
```

### Dependency Rules
- **Domain** has no dependencies on other layers
- **Application** depends only on Domain
- **Infrastructure** depends on Application and Domain
- **Presentation** depends on Application and can reference Infrastructure for DI setup

### Key Architectural Patterns (from `.github/copilot-instructions.md`)
The codebase is designed to implement:
- **Repository pattern** for data access
- **CQRS** (Command Query Responsibility Segregation) for separating read/write operations
- **Specification pattern** for complex queries
- **Unit of Work pattern** for managing transactions
- **Inbox/Outbox pattern** for distributed transactions
- **Mediator pattern** for decoupling components (custom implementation with reflection caching)
- **Validation pipeline** integrated into the mediator (custom `IValidator<T>`)
- **Factory, Adapter, Observer, and Builder patterns** as appropriate

### Aspire Service Defaults
`OpenMedSphere.ServiceDefaults` provides shared configuration for all services:
- **OpenTelemetry**: Distributed tracing, metrics, and logging
  - ASP.NET Core instrumentation (requests, middleware)
  - HTTP client instrumentation
  - Runtime instrumentation (GC, thread pool, etc.)
  - OTLP exporter (when `OTEL_EXPORTER_OTLP_ENDPOINT` is configured)
- **Health Checks**: `/health` and `/alive` endpoints (development only)
- **Service Discovery**: Automatic service-to-service discovery
- **Resilience**: Standard resilience handler for HTTP clients (retry, circuit breaker, timeout)

All services call `builder.AddServiceDefaults()` to register these features.

## Technology Stack

- **.NET 10.0** (C# 14)
- **.NET Aspire 13.1.2**: Cloud-native orchestration, service discovery, observability
- **EF Core 10.0.5** with PostgreSQL (Npgsql)
- **OpenTelemetry 1.15.0**: Distributed tracing and observability
- **JWT Bearer Authentication**: API security with researcher ID extracted from JWT `NameIdentifier` claim
- **Rate Limiting**: Built-in ASP.NET Core rate limiter (fixed window policies)
- **xUnit v3 (3.2.2)**: Testing framework
- **Moq 4.20.72**: Mocking framework for tests

## Development Commands

### Build
```bash
# Build entire solution (uses .slnx XML format)
dotnet build OpenMedSphere.slnx

# Build specific project
dotnet build src/Presentation/OpenMedSphere.API/OpenMedSphere.API.csproj
```

### Run with Aspire (Recommended)
The AppHost orchestrates all services with built-in dashboard and telemetry:
```bash
dotnet run --project src/Presentation/OpenMedSphere.AppHost/OpenMedSphere.AppHost.csproj
```
This starts:
- **API**: Backend service with health checks at `/health`
- **PostgreSQL**: Database with pgAdmin
- **Redis**: Distributed cache
- **Aspire Dashboard** (typically at `http://localhost:15888`) showing:
  - All running services and their URLs
  - Distributed traces
  - Metrics and logs
  - Service health status

### Run Individual Services
```bash
# API only
dotnet run --project src/Presentation/OpenMedSphere.API/OpenMedSphere.API.csproj
```

### Testing
```bash
# Run all tests (172 tests: 133 domain + 39 application)
dotnet test OpenMedSphere.slnx

# Run domain tests only
dotnet test tests/OpenMedSphere.Domain.Tests/OpenMedSphere.Domain.Tests.csproj

# Run application tests only
dotnet test tests/OpenMedSphere.Application.Tests/OpenMedSphere.Application.Tests.csproj

# Run single test by name
dotnet test --filter "FullyQualifiedName~MethodName_Scenario_ExpectedBehavior"
```

### Restore & Clean
```bash
# Restore dependencies
dotnet restore

# Clean build artifacts
dotnet clean
```

## Code Style Guidelines

Follow formatting in `.editorconfig` and `.github/copilot-instructions.md`:

### C# Conventions
- **C# Version**: Always use C# 14 features (latest version)
- **Nullable Reference Types**: Always enabled. Declare non-nullable by default, check for null at entry points
- **Null Checks**: Use `is null` or `is not null` (NEVER `== null` or `!= null`)
- **Namespaces**: Use block-scoped namespace declarations (per `.editorconfig`)
- **var Usage**: Prefer `var` when the type is apparent (`csharp_style_var_*` = true)
- **Pattern Matching**: Use pattern matching and switch expressions wherever possible
- **Member Names**: Use `nameof()` instead of string literals
- **Curly Braces**: Insert newline before opening brace of code blocks (`csharp_new_line_before_open_brace = all`)
- **Expression Bodies**: Use for accessors, indexers, lambdas, and properties; avoid for constructors, methods, operators
- **Primary Constructors**: Preferred when appropriate

### Documentation
- **XML Comments**: Required for all public APIs
- Include `<param>` and `<returns>` tags where applicable
- Example:
```csharp
/// <summary>
/// Anonymizes patient data according to the specified policy.
/// </summary>
/// <param name="data">The patient data to anonymize.</param>
/// <param name="policy">The anonymization policy to apply.</param>
/// <returns>Anonymized patient data.</returns>
public AnonymizedData Anonymize(PatientData data, AnonymizationPolicy policy)
```

### Domain Entity Patterns
Follow the established patterns in existing entities:
- Use private constructors with static factory methods (`Create`)
- Include parameterless constructor for EF Core
- Mark aggregates as `sealed`
- Use `required` modifier for mandatory properties
- Track `CreatedAtUtc` and `UpdatedAtUtc` timestamps
- Use `private set` for state properties modified by domain methods (e.g., `Status`, `UpdatedAtUtc`) — EF Core sets them via reflection
- Validate inputs using `ArgumentNullException.ThrowIfNull()`, `ArgumentException.ThrowIfNullOrWhiteSpace()`, etc.
- Raise domain events in factory methods and critical state changes
- Encapsulate mutable collections with private backing fields and `IReadOnlyCollection<T>` public properties

Example:
```csharp
public sealed class MyEntity : AggregateRoot<Guid>
{
    private readonly List<string> _items = [];

    public required string Name { get; set; }
    public IReadOnlyCollection<string> Items => _items.AsReadOnly();
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private MyEntity() : base() { }
    private MyEntity(Guid id) : base(id) { CreatedAtUtc = DateTime.UtcNow; }

    public static MyEntity Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var entity = new MyEntity(Guid.CreateVersion7()) { Name = name };
        entity.RaiseDomainEvent(new MyEntityCreatedEvent(entity.Id));
        return entity;
    }
}
```

### Validation Patterns
- Custom `IValidator<T>` interface in `Application/Messaging/`
- Validators are auto-discovered and registered alongside handlers in DI
- The mediator runs validation before dispatching to handlers
- Use simple `if` checks that add `ValidationError` entries
- Validators return `ValidationResult` (not exceptions)
- Every command should have a validator — even if it only checks for empty GUIDs
- Use built-in .NET validation where possible (e.g., `MailAddress.TryCreate` for email)

### Command Handler Patterns
- Check preconditions (status, permissions, active state) before calling domain methods
- Return `Result.InvalidOperation()` for failed preconditions — do NOT catch domain exceptions for control flow
- Validate entity exists → check authorization → check state → perform action
- Use `Guid.CreateVersion7()` for all new entity IDs (better index performance, time-sortable)

### Testing Conventions
- **Framework**: xUnit v3 for all tests
- **Mocking**: Moq for test doubles
- **Naming**: `[MethodName]_[TestScenario]_[ExpectedBehavior]`
- **Test Class Naming**: `[ClassBeingTested]Tests`
- **No Comment Markers**: DO NOT emit "Arrange", "Act", or "Assert" comments
- Follow [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## Package Management

This solution uses **Central Package Management** via `Directory.Packages.props`.

### Current Package Versions
See `Directory.Packages.props` for the authoritative list. Key packages:
| Package | Version |
|---------|---------|
| Aspire.AppHost.Sdk | 13.1.2 |
| Aspire.Hosting.PostgreSQL / Redis | 13.1.2 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.5 |
| Microsoft.EntityFrameworkCore | 10.0.5 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0 |
| Microsoft.Extensions.Caching.Hybrid | 10.4.0 |
| OpenTelemetry.* | 1.15.x |
| Scalar.AspNetCore | 2.13.8 |
| xunit.v3 | 3.2.2 |
| Moq | 4.20.72 |

**Note:** `Aspire.Hosting.AppHost` is implicitly provided by the `Aspire.AppHost.Sdk` and must NOT be listed in `Directory.Packages.props`.

### Adding New Packages
1. Add version to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="PackageName" Version="x.y.z" />
   ```
2. Reference in `.csproj` WITHOUT version:
   ```xml
   <PackageReference Include="PackageName" />
   ```

**DO NOT** add new packages unless explicitly requested by the user.

## Project Configuration

Global settings in `Directory.Build.props`:
- **Target Framework**: .NET 10.0
- **Implicit Usings**: Enabled
- **Nullable Reference Types**: Enabled

### Solution File Format
This project uses the new `.slnx` XML-based solution format introduced in .NET 9. The `OpenMedSphere.slnx` file contains:
- Project references organized by folder structure
- Solution items including `CLAUDE.md`, `.editorconfig`, config files
- Virtual folders for organizing projects (Core, Infrastructure, Presentation, tests)

### User Secrets
AppHost uses User Secrets for local development:
- **Secret ID**: `dba9a07f-af44-47a8-832c-910ca1722e64`
- Manage with: `dotnet user-secrets set "key" "value" --project src/Presentation/OpenMedSphere.AppHost`

## API Security

### Authentication
- **JWT Bearer** authentication on all API endpoints
- Researcher identity extracted from JWT `NameIdentifier` claim (not from request parameters)
- Configuration via `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` settings (with dev defaults)
- Development token endpoint: `POST /api/auth/dev-token?researcherId={guid}` (only available in Development environment)

### Rate Limiting
- **Fixed window** policy (100 requests/minute) for read endpoints (GET)
- **Write** policy (30 requests/minute) for write endpoints (POST, PUT, DELETE)
- Returns HTTP 429 when exceeded

### Input Validation
- Custom `IValidator<T>` pipeline integrated into the mediator
- All commands and queries are validated before handler execution
- Validation errors return HTTP 400 with detailed error messages

### Audit Logging
- EF Core `SaveChangesInterceptor` tracks changes to `PatientData`, `ResearchStudy`, `AnonymizationPolicy`, `Researcher`, and `DataShare` entities
- Records entity type, ID, action (Created/Modified/Deleted), old/new values as JSON, and timestamp
- Stored in the `AuditLog` table

## CI/CD

GitHub Actions workflow (`.github/workflows/pr-validation.yml`):
- Runs on: PRs and pushes to `master` branch
- .NET 10 setup (10.x)
- Full solution build using `.slnx` format
- Future: Tests, CodeQL security scanning, deployment

## Current Implementation Status

**Implemented:**
- Solution structure with Clean Architecture layers
- Aspire orchestration with AppHost (API + PostgreSQL + Redis with health checks)
- ServiceDefaults with OpenTelemetry, health checks, resilience
- API with OpenAPI support and Scalar API reference
- **Domain Layer (complete):**
  - Aggregate roots: `PatientData`, `ResearchStudy`, `AnonymizationPolicy`, `Researcher`, `DataShare`, `AuditLogEntry`
  - Value objects: `PatientIdentifier`, `DateRange`, `StudyCode`, `MedicalCode`, `PublicKeySet`
  - Domain events: `PatientDataCreatedEvent`, `PatientDataAnonymizedEvent`, `ResearchStudyCreatedEvent`, `ResearcherCreatedEvent`, `PatientDataSharedEvent`, `DataShareAccessedEvent`, `DataShareRevokedEvent`
  - Enums: `AnonymizationLevel`, `DataShareStatus` (Pending, Accepted, Revoked, Expired)
  - Primitives: `Entity<TId>`, `AggregateRoot<TId>`, `IDomainEvent` (all value objects are C# records, no base class needed)
  - Encapsulated mutable collections with IReadOnlyCollection properties
- **Application Layer (complete):**
  - Custom mediator with reflection caching for handler/validator lookup
  - CQRS commands/queries for PatientData, ResearchStudy, AnonymizationPolicy, MedicalTerminology, Researchers, DataShares
  - Custom validation pipeline (`IValidator<T>`, `ValidationResult`)
  - Specification pattern for complex queries
- **Infrastructure Layer (complete):**
  - EF Core with PostgreSQL via Npgsql
  - Generic repository pattern with specification evaluator
  - ICD-11 medical terminology integration (API + fallback static dataset)
  - HybridCache (`Microsoft.Extensions.Caching.Hybrid`) for ICD-11 API response caching
  - Audit logging via SaveChangesInterceptor
  - FK constraints on DataShare → Researcher and PatientData (with Restrict delete)
  - Database indexes on key query columns
- **API Layer (complete):**
  - JWT Bearer authentication on all endpoints
  - Rate limiting (fixed window for reads, write policy for mutations)
  - Development token endpoint
  - Minimal API endpoints for all entities, medical terminology, researchers, and data shares
- **E2E Encrypted Data Sharing (server-side complete):**
  - `Researcher` entity with hybrid PQC public keys (ML-KEM-768, ML-DSA-65, X25519, ECDSA P-256)
  - `DataShare` entity for opaque encrypted blob storage (zero-knowledge server)
  - Key rotation with monotonic version enforcement
  - CQRS commands: RegisterResearcher, UpdatePublicKeys, CreateDataShare, AcceptDataShare, RevokeDataShare
  - Full lifecycle: create → accept → revoke with domain events
  - Authorization via JWT claims (researcher ID extracted from `NameIdentifier` claim, not request parameters)
  - Active status checks on sender/recipient during share creation
  - Handler-level precondition checks for state transitions (no exception-driven control flow)
- **Testing (172 tests):**
  - Domain entity tests (PatientData, ResearchStudy, AnonymizationPolicy, Researcher, DataShare)
  - Value object tests (PatientIdentifier, DateRange, StudyCode, MedicalCode, PublicKeySet)
  - Command handler tests with Moq (CreatePatientData, AnonymizePatientData, CreateResearchStudy, CreateAnonymizationPolicy, RegisterResearcher, CreateDataShare, AcceptDataShare, RevokeDataShare, UpdateResearcherPublicKeys)
  - Validator tests (CreatePatientDataCommandValidator)

**Planned (Not Yet Implemented):**
- React + Vite + TypeScript frontend (Blazor WASM has been removed)
- Rust WASM crypto module (client-side hybrid PQC encryption via `pqcrypto` crate)
- Client-side key generation, encryption, decryption, signing, verification
- IndexedDB private key storage (passphrase-protected)
- Message queue integration (RabbitMQ/Kafka)
- Integration tests

## Security Considerations

- Avoid OWASP Top 10 vulnerabilities (XSS, SQL injection, command injection, etc.)
- LIKE wildcard injection prevented in repository queries (escaping `%`, `_`, `\`)
- Never commit secrets, API keys, or tokens
- Use User Secrets for local development
- JWT authentication required on all data endpoints
- Rate limiting to prevent abuse
- Audit logging for compliance tracking
- E2E encrypted data sharing with zero-knowledge server architecture
- Hybrid quantum-safe cryptography: ML-KEM-768 + X25519 (key agreement), ML-DSA-65 + ECDSA (signatures), AES-256-GCM (symmetric)
- Future: Implement HIPAA and GDPR compliance requirements
- Future: Client-side crypto via Rust WASM for data at rest and in transit
