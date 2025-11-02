namespace OpenMedSphere.Domain.ValueObjects;

/// <summary>
/// Represents a date range with a start and end date.
/// </summary>
public sealed record DateRange
{
    /// <summary>
    /// Gets the start date of the range.
    /// </summary>
    public required DateTime Start { get; init; }

    /// <summary>
    /// Gets the end date of the range.
    /// </summary>
    public required DateTime End { get; init; }

    /// <summary>
    /// Gets the duration of the date range.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Creates a new date range.
    /// </summary>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <returns>A new date range if validation succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when the end date is before the start date.</exception>
    public static DateRange Create(DateTime start, DateTime end)
    {
        if (end < start)
        {
            throw new ArgumentException("End date must be after or equal to start date.", nameof(end));
        }

        return new DateRange { Start = start, End = end };
    }

    /// <summary>
    /// Checks if the specified date is within this date range.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is within the range; otherwise, false.</returns>
    public bool Contains(DateTime date) => date >= Start && date <= End;

    /// <summary>
    /// Checks if this date range overlaps with another date range.
    /// </summary>
    /// <param name="other">The other date range.</param>
    /// <returns>True if the ranges overlap; otherwise, false.</returns>
    public bool Overlaps(DateRange other) => Start <= other.End && End >= other.Start;
}
