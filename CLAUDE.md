# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

OpenMedSphere is a secure medical research collaboration platform built with .NET 10 and Aspire. The project is in early stages and serves as a personal learning experience focused on exploring cutting-edge technologies including quantum-safe encryption, clean architecture, and domain-driven design principles.

**Key Goals:**
- Secure processing and sharing of anonymized patient data
- HIPAA and GDPR compliance
- OpenPGP with quantum-safe encryption (CRYSTALS-Kyber, Falcon) - planned but not yet implemented
- Self-hostable architecture using .NET Aspire
- Licensed under AGPL v3

## Architecture

The project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles with strict layer separation:

### Layer Structure
```
src/
├── Core/
│   ├── OpenMedSphere.Domain/          # Core business entities (PatientData, ResearchStudy, AnonymizationPolicy)
│   └── OpenMedSphere.Application/     # Use cases, domain orchestration, CQRS commands/queries
├── Infrastructure/
│   └── OpenMedSphere.Infrastructure/  # Database, external integrations, encryption services
└── Presentation/
    ├── OpenMedSphere.API/             # ASP.NET Core Web API (minimal endpoints)
    ├── OpenMedSphere.Web/             # Blazor WebAssembly frontend with PWA support
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
│   └── AuditLogEntry.cs        # Audit log entry entity
├── Events/
│   ├── PatientDataCreatedEvent.cs    # Raised on patient data creation
│   ├── PatientDataAnonymizedEvent.cs # Raised on anonymization
│   └── ResearchStudyCreatedEvent.cs  # Raised on study creation
├── ValueObjects/
│   ├── PatientIdentifier.cs    # Anonymized patient ID (record type)
│   ├── DateRange.cs            # Date range with validation (record type)
│   ├── StudyCode.cs            # Unique study code (record type)
│   └── MedicalCode.cs          # Structured medical code (record type)
├── Enums/
│   └── AnonymizationLevel.cs   # None, Basic, Standard, Advanced, Full
└── Primitives/
    ├── Entity.cs               # Base entity with ID and equality
    ├── AggregateRoot.cs        # Base aggregate with domain events
    ├── ValueObject.cs          # Base value object
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
- **.NET Aspire 13.1.0**: Cloud-native orchestration, service discovery, observability
- **Blazor WebAssembly**: Frontend with PWA/service worker support
- **EF Core 10.0.2** with PostgreSQL (Npgsql)
- **OpenTelemetry 1.15.0**: Distributed tracing and observability
- **JWT Bearer Authentication**: API security with configurable JWT tokens
- **Rate Limiting**: Built-in ASP.NET Core rate limiter (fixed window policies)
- **xUnit 2.9.3**: Testing framework
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
- **Frontend**: Blazor WebAssembly with external HTTP endpoints
- **Aspire Dashboard** (typically at `http://localhost:15888`) showing:
  - All running services and their URLs
  - Distributed traces
  - Metrics and logs
  - Service health status

The frontend waits for the API to be ready before starting (`WaitFor(api)`).

### Run Individual Services
```bash
# API only
dotnet run --project src/Presentation/OpenMedSphere.API/OpenMedSphere.API.csproj

# Blazor Web frontend only
dotnet run --project src/Presentation/OpenMedSphere.Web/OpenMedSphere.Web.csproj
```

### Testing
```bash
# Run all tests (108 tests: 93 domain + 15 application)
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
    public DateTime? UpdatedAtUtc { get; set; }

    private MyEntity() : base() { }
    private MyEntity(Guid id) : base(id) { CreatedAtUtc = DateTime.UtcNow; }

    public static MyEntity Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var entity = new MyEntity(Guid.NewGuid()) { Name = name };
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

### Testing Conventions
- **Framework**: xUnit for all tests
- **Mocking**: Moq for test doubles
- **Naming**: `[MethodName]_[TestScenario]_[ExpectedBehavior]`
- **Test Class Naming**: `[ClassBeingTested]Tests`
- **No Comment Markers**: DO NOT emit "Arrange", "Act", or "Assert" comments
- Follow [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## Package Management

This solution uses **Central Package Management** via `Directory.Packages.props`.

### Current Package Versions
| Package | Version |
|---------|---------|
| Aspire.Hosting.AppHost | 13.1.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.2 |
| Microsoft.AspNetCore.OpenApi | 10.0.2 |
| Microsoft.AspNetCore.Components.WebAssembly | 10.0.2 |
| Microsoft.EntityFrameworkCore | 10.0.2 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0 |
| Microsoft.Extensions.Http.Resilience | 10.2.0 |
| Microsoft.Extensions.ServiceDiscovery | 10.2.0 |
| OpenTelemetry.* | 1.15.0 |
| Scalar.AspNetCore | 2.12.35 |
| xUnit | 2.9.3 |
| Moq | 4.20.72 |

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
- Configuration via `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` settings (with dev defaults)
- Development token endpoint: `POST /api/auth/dev-token` (only available in Development environment)

### Rate Limiting
- **Fixed window** policy (100 requests/minute) for read endpoints (GET)
- **Write** policy (30 requests/minute) for write endpoints (POST, PUT, DELETE)
- Returns HTTP 429 when exceeded

### Input Validation
- Custom `IValidator<T>` pipeline integrated into the mediator
- All commands and queries are validated before handler execution
- Validation errors return HTTP 400 with detailed error messages

### Audit Logging
- EF Core `SaveChangesInterceptor` tracks changes to `PatientData`, `ResearchStudy`, and `AnonymizationPolicy` entities
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
- Aspire orchestration with AppHost (API + Frontend with health checks)
- ServiceDefaults with OpenTelemetry, health checks, resilience
- Basic Blazor WebAssembly frontend with PWA support
- API with OpenAPI support and Scalar API reference
- **Domain Layer (complete):**
  - Aggregate roots: `PatientData`, `ResearchStudy`, `AnonymizationPolicy`, `AuditLogEntry`
  - Value objects: `PatientIdentifier`, `DateRange`, `StudyCode`, `MedicalCode`
  - Domain events: `PatientDataCreatedEvent`, `PatientDataAnonymizedEvent`, `ResearchStudyCreatedEvent`
  - Enums: `AnonymizationLevel` (None, Basic, Standard, Advanced, Full)
  - Primitives: `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `IDomainEvent`
  - Encapsulated mutable collections with IReadOnlyCollection properties
- **Application Layer (complete):**
  - Custom mediator with reflection caching for handler/validator lookup
  - CQRS commands/queries for PatientData, ResearchStudy, AnonymizationPolicy, MedicalTerminology
  - Custom validation pipeline (`IValidator<T>`, `ValidationResult`)
  - Specification pattern for complex queries
- **Infrastructure Layer (complete):**
  - EF Core with PostgreSQL via Npgsql
  - Generic repository pattern with specification evaluator
  - ICD-11 medical terminology integration (API + fallback static dataset)
  - Memory cache with size limits
  - Audit logging via SaveChangesInterceptor
  - Database indexes on key query columns
- **API Layer (complete):**
  - JWT Bearer authentication on all endpoints
  - Rate limiting (fixed window for reads, write policy for mutations)
  - Development token endpoint
  - Minimal API endpoints for all entities and medical terminology
- **Testing (108 tests):**
  - Domain entity tests (PatientData, ResearchStudy, AnonymizationPolicy)
  - Value object tests (PatientIdentifier, DateRange, StudyCode, MedicalCode)
  - Command handler tests with Moq (CreatePatientData, AnonymizePatientData, CreateResearchStudy, CreateAnonymizationPolicy)

**Planned (Not Yet Implemented):**
- Quantum-safe encryption services (CRYSTALS-Kyber, Falcon)
- Message queue integration (RabbitMQ/Kafka)
- Integration tests
- Blazor frontend connected to API

## Security Considerations

- Avoid OWASP Top 10 vulnerabilities (XSS, SQL injection, command injection, etc.)
- LIKE wildcard injection prevented in repository queries (escaping `%`, `_`, `\`)
- Never commit secrets, API keys, or tokens
- Use User Secrets for local development
- JWT authentication required on all data endpoints
- Rate limiting to prevent abuse
- Audit logging for compliance tracking
- Future: Implement HIPAA and GDPR compliance requirements
- Future: Quantum-safe encryption for data at rest and in transit
