namespace OpenMedSphere.Application.Common;

/// <summary>
/// Represents a paged result set.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Gets the total count of items across all pages.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}
