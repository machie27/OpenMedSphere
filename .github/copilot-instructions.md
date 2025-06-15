General

 * Make only high confidence suggestions when reviewing code changes.
 * Always use the latest version C#, currently C# 13 features.
 * Do not add new libraries or NuGet packages unless explicitly requested.

Formatting

 * Apply code-formatting style defined in .editorconfig.
 * Prefer file-scoped namespace declarations and single-line using directives.
 * Insert a newline before the opening curly brace of any code block (e.g., after if, for, while, foreach, using, try, etc.).
 * Ensure that the final return statement of a method is on its own line.
 * Use pattern matching and switch expressions wherever possible.
 * Use nameof instead of string literals when referring to member names.
 * Ensure that XML doc comments are created for any public APIs. When applicable, include and  documentation in the comments.


Nullable Reference Types
 
 * Declare variables non-nullable, and check for null at entry points.
 * Always use is null or is not null instead of == null or != null.
 * Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.
 
Testing

 * Use xUnit for tests.
 * Do not emit "Act", "Arrange" or "Assert" comments.
 * Use Moq for mocking in tests.
 * Unit testing should adhere to [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
   * Name unit tests with `[name of method]_[test scenario]_[expected behavior]`
     * Extension: Name the test class with [name of the class being tested]Tests


Running tests


Architecture
 * Use the Clean Architecture principles.
 * Use the Repository pattern for data access.
 * Use Dependency Injection for managing dependencies.
 * Use CQRS (Command Query Responsibility Segregation) for separating read and write operations.
 * Use the Specification pattern for complex queries.
 * Use the Unit of Work pattern for managing transactions.
 * Use the Factory pattern for creating objects.
 * Use the Adapter pattern for integrating with external systems.
 * Use the Observer pattern for event handling.
 * Use the Builder pattern for constructing complex objects.
 * Use the inbox/outbox pattern for managing distributed transactions.
 * Use the mediator pattern for decoupling components. 