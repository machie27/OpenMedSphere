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
- **Mediator pattern** for decoupling components
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

- **.NET 9** (C# 13)
- **.NET Aspire 9.3.1**: Cloud-native orchestration, service discovery, observability
- **Blazor WebAssembly**: Frontend with PWA/service worker support
- **OpenTelemetry 1.12.0**: Distributed tracing and observability
- **xUnit**: Testing framework
- **Moq**: Mocking framework for tests

## Development Commands

### Build
```bash
# Build entire solution
dotnet build OpenMedSphere.sln

# Build specific project
dotnet build src/Presentation/OpenMedSphere.API/OpenMedSphere.API.csproj
```

### Run with Aspire (Recommended)
The AppHost orchestrates all services with built-in dashboard and telemetry:
```bash
dotnet run --project src/Presentation/OpenMedSphere.AppHost/OpenMedSphere.AppHost.csproj
```
This starts the Aspire dashboard (typically at `http://localhost:15888`) which shows:
- All running services and their URLs
- Distributed traces
- Metrics and logs
- Service health status

### Run Individual Services
```bash
# API only
dotnet run --project src/Presentation/OpenMedSphere.API/OpenMedSphere.API.csproj

# Blazor Web frontend only
dotnet run --project src/Presentation/OpenMedSphere.Web/OpenMedSphere.Web.csproj
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests in specific project
dotnet test tests/OpenMedSphere.Tests/OpenMedSphere.Tests.csproj

# Run single test by name
dotnet test --filter "FullyQualifiedName~MethodName_Scenario_ExpectedBehavior"

# Run tests by category/trait
dotnet test --filter "Category=Unit"
```

Test structure mirrors source structure:
- `tests/OpenMedSphere.Tests/Domain/` for domain entity tests
- `tests/OpenMedSphere.Tests/Application/` for application service tests

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
- **Namespaces**: Prefer file-scoped namespace declarations
- **Pattern Matching**: Use pattern matching and switch expressions wherever possible
- **Member Names**: Use `nameof()` instead of string literals
- **Curly Braces**: Insert newline before opening brace of code blocks
- **Final Return**: Ensure final return statement is on its own line

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

### Testing Conventions
- **Framework**: xUnit for all tests
- **Mocking**: Moq for test doubles
- **Naming**: `[MethodName]_[TestScenario]_[ExpectedBehavior]`
- **Test Class Naming**: `[ClassBeingTested]Tests`
- **No Comment Markers**: DO NOT emit "Arrange", "Act", or "Assert" comments
- Follow [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

Example:
```csharp
public class PatientDataTests
{
    [Fact]
    public void Anonymize_WithValidPolicy_RemovesIdentifiableInformation()
    {
        // Test implementation without comment markers
    }
}
```

## Package Management

This solution uses **Central Package Management** via `Directory.Packages.props`.

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

### User Secrets
AppHost uses User Secrets for local development:
- **Secret ID**: `dba9a07f-af44-47a8-832c-910ca1722e64`
- Manage with: `dotnet user-secrets set "key" "value" --project src/Presentation/OpenMedSphere.AppHost`

## CI/CD

GitHub Actions workflow (`.github/workflows/pr-validation.yml`):
- Runs on: PRs and pushes to `master` branch
- .NET 9 setup
- Full solution build
- Future: Tests, CodeQL security scanning, deployment

## Current Implementation Status

**Implemented:**
- Solution structure with Clean Architecture layers
- Aspire orchestration with AppHost
- ServiceDefaults with OpenTelemetry, health checks, resilience
- Basic Blazor WebAssembly frontend with PWA support
- API with sample weather forecast endpoint
- Test project structure (Domain/Application folders)

**Planned (Not Yet Implemented):**
- Domain entities (PatientData, ResearchStudy, AnonymizationPolicy)
- Application layer with CQRS commands/queries
- Repository implementations and database context
- Quantum-safe encryption services (CRYSTALS-Kyber, Falcon)
- Message queue integration (RabbitMQ/Kafka)
- Comprehensive test coverage

## Security Considerations

- Avoid OWASP Top 10 vulnerabilities (XSS, SQL injection, command injection, etc.)
- Never commit secrets, API keys, or tokens
- Use User Secrets for local development
- Future: Implement HIPAA and GDPR compliance requirements
- Future: Quantum-safe encryption for data at rest and in transit
