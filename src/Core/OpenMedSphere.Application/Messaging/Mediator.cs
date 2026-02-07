using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenMedSphere.Application.Messaging;

/// <summary>
/// Default mediator implementation that resolves handlers from the service provider.
/// </summary>
internal sealed partial class Mediator(
    IServiceProvider serviceProvider,
    ILogger<Mediator> logger) : IMediator
{
    private static readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> CommandHandlerCache = new();
    private static readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> CommandWithResponseHandlerCache = new();
    private static readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> QueryHandlerCache = new();
    private static readonly ConcurrentDictionary<Type, (Type ValidatorType, MethodInfo ValidateMethod)> ValidatorCache = new();

    /// <inheritdoc />
    public async Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        Type commandType = command.GetType();
        string commandName = commandType.Name;
        LogCommandDispatching(commandName);

        Result? validationFailure = await ValidateAsync(commandType, command, cancellationToken);
        if (validationFailure is not null)
        {
            return validationFailure;
        }

        (Type handlerType, MethodInfo handleMethod) = CommandHandlerCache.GetOrAdd(commandType, static type =>
        {
            Type ht = typeof(ICommandHandler<>).MakeGenericType(type);
            MethodInfo hm = ht.GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync))
                ?? throw new InvalidOperationException($"Handler for {type.Name} does not have a HandleAsync method.");
            return (ht, hm);
        });

        object handler = serviceProvider.GetRequiredService(handlerType);

        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            object? result = handleMethod.Invoke(handler, [command, cancellationToken]);

            Result commandResult = result is Task<Result> task
                ? await task.ConfigureAwait(false)
                : throw new InvalidOperationException($"Handler for {commandName} returned an unexpected type.");

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);

            if (commandResult.IsSuccess)
            {
                LogCommandSucceeded(commandName, elapsed.TotalMilliseconds);
            }
            else
            {
                LogCommandFailed(commandName, commandResult.Error!, elapsed.TotalMilliseconds);
            }

            return commandResult;
        }
        catch (Exception ex)
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            LogCommandException(commandName, elapsed.TotalMilliseconds, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        Type commandType = command.GetType();
        string commandName = commandType.Name;
        LogCommandWithResponseDispatching(commandName, typeof(TResponse).Name);

        Result? validationFailure = await ValidateAsync(commandType, command, cancellationToken);
        if (validationFailure is not null)
        {
            return Result<TResponse>.Failure(validationFailure.Error!);
        }

        (Type handlerType, MethodInfo handleMethod) = CommandWithResponseHandlerCache.GetOrAdd(commandType, type =>
        {
            Type ht = typeof(ICommandHandler<,>).MakeGenericType(type, typeof(TResponse));
            MethodInfo hm = ht.GetMethod(nameof(ICommandHandler<ICommand<object>, object>.HandleAsync))
                ?? throw new InvalidOperationException($"Handler for {type.Name} does not have a HandleAsync method.");
            return (ht, hm);
        });

        object handler = serviceProvider.GetRequiredService(handlerType);

        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            object? result = handleMethod.Invoke(handler, [command, cancellationToken]);

            Result<TResponse> commandResult = result is Task<Result<TResponse>> task
                ? await task.ConfigureAwait(false)
                : throw new InvalidOperationException($"Handler for {commandName} returned an unexpected type.");

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);

            if (commandResult.IsSuccess)
            {
                LogCommandWithResponseSucceeded(commandName, typeof(TResponse).Name, elapsed.TotalMilliseconds);
            }
            else
            {
                LogCommandWithResponseFailed(commandName, typeof(TResponse).Name, commandResult.Error!, elapsed.TotalMilliseconds);
            }

            return commandResult;
        }
        catch (Exception ex)
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            LogCommandWithResponseException(commandName, typeof(TResponse).Name, elapsed.TotalMilliseconds, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Result<TResponse>> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        Type queryType = query.GetType();
        string queryName = queryType.Name;
        LogQueryDispatching(queryName, typeof(TResponse).Name);

        Result? validationFailure = await ValidateAsync(queryType, query, cancellationToken);
        if (validationFailure is not null)
        {
            return Result<TResponse>.Failure(validationFailure.Error!);
        }

        (Type handlerType, MethodInfo handleMethod) = QueryHandlerCache.GetOrAdd(queryType, type =>
        {
            Type ht = typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TResponse));
            MethodInfo hm = ht.GetMethod(nameof(IQueryHandler<IQuery<object>, object>.HandleAsync))
                ?? throw new InvalidOperationException($"Handler for {type.Name} does not have a HandleAsync method.");
            return (ht, hm);
        });

        object handler = serviceProvider.GetRequiredService(handlerType);

        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            object? result = handleMethod.Invoke(handler, [query, cancellationToken]);

            Result<TResponse> queryResult = result is Task<Result<TResponse>> task
                ? await task.ConfigureAwait(false)
                : throw new InvalidOperationException($"Handler for {queryName} returned an unexpected type.");

            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);

            if (queryResult.IsSuccess)
            {
                LogQuerySucceeded(queryName, typeof(TResponse).Name, elapsed.TotalMilliseconds);
            }
            else
            {
                LogQueryFailed(queryName, typeof(TResponse).Name, queryResult.Error!, elapsed.TotalMilliseconds);
            }

            return queryResult;
        }
        catch (Exception ex)
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            LogQueryException(queryName, typeof(TResponse).Name, elapsed.TotalMilliseconds, ex);
            throw;
        }
    }

    private async Task<Result?> ValidateAsync<T>(Type messageType, T message, CancellationToken cancellationToken = default)
    {
        var (validatorType, validateMethod) = ValidatorCache.GetOrAdd(messageType, static type =>
        {
            var vt = typeof(IValidator<>).MakeGenericType(type);
            var vm = vt.GetMethod(nameof(IValidator<object>.ValidateAsync))
                ?? throw new InvalidOperationException($"IValidator<{type.Name}> does not have a ValidateAsync method.");
            return (vt, vm);
        });

        object? validator = serviceProvider.GetService(validatorType);
        if (validator is null)
        {
            return null;
        }

        object? resultObj = validateMethod.Invoke(validator, [message, cancellationToken]);
        ValidationResult validationResult = resultObj is Task<ValidationResult> task
            ? await task.ConfigureAwait(false)
            : throw new InvalidOperationException($"Validator for {messageType.Name} returned an unexpected type.");

        if (validationResult.IsValid)
        {
            return null;
        }

        string errorMessage = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
        return Result.ValidationFailed(errorMessage);
    }

    [LoggerMessage(EventId = 1000, Level = LogLevel.Debug, Message = "Dispatching command {CommandName}")]
    private partial void LogCommandDispatching(string commandName);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Debug, Message = "Command {CommandName} succeeded in {ElapsedMs:F1}ms")]
    private partial void LogCommandSucceeded(string commandName, double elapsedMs);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning, Message = "Command {CommandName} failed with error '{Error}' in {ElapsedMs:F1}ms")]
    private partial void LogCommandFailed(string commandName, string error, double elapsedMs);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Error, Message = "Command {CommandName} threw an exception after {ElapsedMs:F1}ms")]
    private partial void LogCommandException(string commandName, double elapsedMs, Exception ex);

    [LoggerMessage(EventId = 1010, Level = LogLevel.Debug, Message = "Dispatching command {CommandName} with response type {ResponseType}")]
    private partial void LogCommandWithResponseDispatching(string commandName, string responseType);

    [LoggerMessage(EventId = 1011, Level = LogLevel.Debug, Message = "Command {CommandName}<{ResponseType}> succeeded in {ElapsedMs:F1}ms")]
    private partial void LogCommandWithResponseSucceeded(string commandName, string responseType, double elapsedMs);

    [LoggerMessage(EventId = 1012, Level = LogLevel.Warning, Message = "Command {CommandName}<{ResponseType}> failed with error '{Error}' in {ElapsedMs:F1}ms")]
    private partial void LogCommandWithResponseFailed(string commandName, string responseType, string error, double elapsedMs);

    [LoggerMessage(EventId = 1013, Level = LogLevel.Error, Message = "Command {CommandName}<{ResponseType}> threw an exception after {ElapsedMs:F1}ms")]
    private partial void LogCommandWithResponseException(string commandName, string responseType, double elapsedMs, Exception ex);

    [LoggerMessage(EventId = 1020, Level = LogLevel.Debug, Message = "Dispatching query {QueryName} with response type {ResponseType}")]
    private partial void LogQueryDispatching(string queryName, string responseType);

    [LoggerMessage(EventId = 1021, Level = LogLevel.Debug, Message = "Query {QueryName}<{ResponseType}> succeeded in {ElapsedMs:F1}ms")]
    private partial void LogQuerySucceeded(string queryName, string responseType, double elapsedMs);

    [LoggerMessage(EventId = 1022, Level = LogLevel.Warning, Message = "Query {QueryName}<{ResponseType}> failed with error '{Error}' in {ElapsedMs:F1}ms")]
    private partial void LogQueryFailed(string queryName, string responseType, string error, double elapsedMs);

    [LoggerMessage(EventId = 1023, Level = LogLevel.Error, Message = "Query {QueryName}<{ResponseType}> threw an exception after {ElapsedMs:F1}ms")]
    private partial void LogQueryException(string queryName, string responseType, double elapsedMs, Exception ex);
}
