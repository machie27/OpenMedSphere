namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Categorizes the type of error in a failed result.
/// </summary>
public enum ErrorCode
{
    /// <summary>No error.</summary>
    None,

    /// <summary>The requested resource was not found.</summary>
    NotFound,

    /// <summary>The operation conflicts with the current state.</summary>
    Conflict,

    /// <summary>The operation is not valid for the current state.</summary>
    InvalidOperation,

    /// <summary>Input validation failed.</summary>
    ValidationFailed
}

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the error code categorizing the failure.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string? error, ErrorCode errorCode = ErrorCode.None)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a not-found failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A not-found failure result.</returns>
    public static Result NotFound(string error) => new(false, error, ErrorCode.NotFound);

    /// <summary>
    /// Creates a conflict failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A conflict failure result.</returns>
    public static Result Conflict(string error) => new(false, error, ErrorCode.Conflict);

    /// <summary>
    /// Creates an invalid-operation failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>An invalid-operation failure result.</returns>
    public static Result InvalidOperation(string error) => new(false, error, ErrorCode.InvalidOperation);

    /// <summary>
    /// Creates a validation-failed failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A validation-failed failure result.</returns>
    public static Result ValidationFailed(string error) => new(false, error, ErrorCode.ValidationFailed);
}

/// <summary>
/// Represents the result of an operation that returns a value and can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing the value of a failed result.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    private Result(T value) : base(true, null)
    {
        _value = value;
    }

    private Result(string error, ErrorCode errorCode = ErrorCode.None) : base(false, error, errorCode)
    {
        _value = default;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public new static Result<T> Failure(string error) => new(error);

    /// <summary>
    /// Creates a not-found failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A not-found failure result.</returns>
    public new static Result<T> NotFound(string error) => new(error, ErrorCode.NotFound);

    /// <summary>
    /// Creates a conflict failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A conflict failure result.</returns>
    public new static Result<T> Conflict(string error) => new(error, ErrorCode.Conflict);

    /// <summary>
    /// Creates an invalid-operation failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>An invalid-operation failure result.</returns>
    public new static Result<T> InvalidOperation(string error) => new(error, ErrorCode.InvalidOperation);
}
