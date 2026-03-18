using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.API.Extensions;

/// <summary>
/// Extension methods for mapping <see cref="Result"/> to minimal API <see cref="IResult"/> responses.
/// </summary>
internal static class ResultExtensions
{
    /// <summary>
    /// Maps a failed <see cref="Result"/> to the appropriate HTTP error response.
    /// </summary>
    /// <param name="result">The failed result.</param>
    /// <returns>The corresponding HTTP error result.</returns>
    internal static IResult MapError(this Result result) =>
        result.ErrorCode switch
        {
            ErrorCode.NotFound => Results.NotFound(result.Error),
            ErrorCode.Conflict => Results.Conflict(result.Error),
            ErrorCode.InvalidOperation => Results.UnprocessableEntity(result.Error),
            ErrorCode.ValidationFailed => Results.BadRequest(result.Error),
            _ => throw new InvalidOperationException($"Unmapped error code: {result.ErrorCode}")
        };
}
