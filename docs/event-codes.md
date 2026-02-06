# Structured Logging Event Codes

All structured log messages in OpenMedSphere use `[LoggerMessage]` source-generated methods with
assigned event IDs. This document maps the reserved ranges to each domain area.

## Event Code Ranges

| Range       | Domain                       | Status      |
|-------------|------------------------------|-------------|
| 1000 - 1099 | Application / Mediator       | Implemented |
| 2000 - 2019 | MedicalTerminologyService    | Implemented |
| 2020 - 2039 | Icd11TerminologyProvider     | Implemented |
| 2040 - 2059 | FallbackTerminologyProvider  | Implemented |
| 3000 - 3099 | PatientData                  | Reserved    |
| 4000 - 4099 | ResearchStudies              | Reserved    |
| 5000 - 5099 | AnonymizationPolicies        | Reserved    |
| 6000 - 6099 | Infrastructure / Persistence | Reserved    |

## Implemented Events

### Mediator (1000 - 1099)

| ID   | Level   | Message |
|------|---------|---------|
| 1000 | Debug   | Dispatching command {CommandName} |
| 1001 | Debug   | Command {CommandName} succeeded in {ElapsedMs}ms |
| 1002 | Warning | Command {CommandName} failed with error '{Error}' in {ElapsedMs}ms |
| 1003 | Error   | Command {CommandName} threw an exception after {ElapsedMs}ms |
| 1010 | Debug   | Dispatching command {CommandName} with response type {ResponseType} |
| 1011 | Debug   | Command {CommandName}<{ResponseType}> succeeded in {ElapsedMs}ms |
| 1012 | Warning | Command {CommandName}<{ResponseType}> failed with error '{Error}' in {ElapsedMs}ms |
| 1013 | Error   | Command {CommandName}<{ResponseType}> threw an exception after {ElapsedMs}ms |
| 1020 | Debug   | Dispatching query {QueryName} with response type {ResponseType} |
| 1021 | Debug   | Query {QueryName}<{ResponseType}> succeeded in {ElapsedMs}ms |
| 1022 | Warning | Query {QueryName}<{ResponseType}> failed with error '{Error}' in {ElapsedMs}ms |
| 1023 | Error   | Query {QueryName}<{ResponseType}> threw an exception after {ElapsedMs}ms |

### MedicalTerminologyService (2000 - 2019)

| ID   | Level | Message |
|------|-------|---------|
| 2000 | Debug | Returning {Count} supported coding systems |
| 2001 | Debug | Routing search to provider '{CodingSystem}' for '{SearchText}' |
| 2002 | Debug | Search for '{SearchText}' (system: {CodingSystem}) returned {ResultCount} total results |
| 2003 | Debug | Routing code lookup to provider '{CodingSystem}' for '{Code}' |
| 2004 | Debug | Code '{Code}' found via provider '{CodingSystem}' |
| 2005 | Debug | Entity URI lookup completed for '{EntityUri}' via provider '{CodingSystem}' |

### Icd11TerminologyProvider (2020 - 2039)

| ID   | Level   | Message |
|------|---------|---------|
| 2020 | Debug   | ICD-11 search cache hit for '{SearchText}' with {ResultCount} results |
| 2021 | Warning | Failed to search ICD-11 API for '{SearchText}' |
| 2022 | Debug   | ICD-11 code lookup cache hit for '{Code}' |
| 2023 | Warning | Failed to lookup ICD-11 code '{Code}' |
| 2024 | Debug   | ICD-11 entity URI lookup cache hit for '{EntityUri}' |
| 2025 | Warning | Failed to lookup ICD-11 entity URI '{EntityUri}' |

### FallbackTerminologyProvider (2040 - 2059)

| ID   | Level | Message |
|------|-------|---------|
| 2040 | Debug | Fallback search for '{SearchText}' returned {ResultCount} results |
| 2041 | Debug | Fallback code lookup for '{Code}', found: {Found} |
| 2042 | Debug | Fallback entity URI lookup for '{EntityUri}', found: {Found} |
| 2043 | Debug | Fallback provider initialized with {CodeCount} static codes |
