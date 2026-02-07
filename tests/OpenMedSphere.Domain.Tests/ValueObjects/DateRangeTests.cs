using OpenMedSphere.Domain.ValueObjects;
using Xunit;

namespace OpenMedSphere.Domain.Tests.ValueObjects
{
    public sealed class DateRangeTests
    {
        [Fact]
        public void Create_WithValidRange_ReturnsDateRange()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            DateRange result = DateRange.Create(start, end);

            Assert.NotNull(result);
            Assert.Equal(start, result.Start);
            Assert.Equal(end, result.End);
        }

        [Fact]
        public void Create_WithSameStartAndEnd_ReturnsDateRange()
        {
            DateTime date = new(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            DateRange result = DateRange.Create(date, date);

            Assert.Equal(date, result.Start);
            Assert.Equal(date, result.End);
        }

        [Fact]
        public void Create_WithEndBeforeStart_ThrowsArgumentException()
        {
            DateTime start = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.Throws<ArgumentException>(() => DateRange.Create(start, end));
        }

        [Fact]
        public void Duration_ReturnsCorrectTimeSpan()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);

            TimeSpan duration = range.Duration;

            Assert.Equal(TimeSpan.FromDays(30), duration);
        }

        [Fact]
        public void Contains_WithDateInsideRange_ReturnsTrue()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);
            DateTime testDate = new(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            bool result = range.Contains(testDate);

            Assert.True(result);
        }

        [Fact]
        public void Contains_WithDateOnStartBoundary_ReturnsTrue()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);

            bool result = range.Contains(start);

            Assert.True(result);
        }

        [Fact]
        public void Contains_WithDateOnEndBoundary_ReturnsTrue()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);

            bool result = range.Contains(end);

            Assert.True(result);
        }

        [Fact]
        public void Contains_WithDateOutsideRange_ReturnsFalse()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);
            DateTime testDate = new(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            bool result = range.Contains(testDate);

            Assert.False(result);
        }

        [Fact]
        public void Contains_WithDateBeforeRange_ReturnsFalse()
        {
            DateTime start = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = new(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            DateRange range = DateRange.Create(start, end);
            DateTime testDate = new(2023, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            bool result = range.Contains(testDate);

            Assert.False(result);
        }

        [Fact]
        public void Overlaps_WithOverlappingRange_ReturnsTrue()
        {
            DateRange range1 = DateRange.Create(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc));
            DateRange range2 = DateRange.Create(
                new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 9, 30, 0, 0, 0, DateTimeKind.Utc));

            bool result = range1.Overlaps(range2);

            Assert.True(result);
        }

        [Fact]
        public void Overlaps_WithAdjacentRanges_ReturnsTrue()
        {
            DateRange range1 = DateRange.Create(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc));
            DateRange range2 = DateRange.Create(
                new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc));

            bool result = range1.Overlaps(range2);

            Assert.True(result);
        }

        [Fact]
        public void Overlaps_WithNonOverlappingRange_ReturnsFalse()
        {
            DateRange range1 = DateRange.Create(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 3, 31, 0, 0, 0, DateTimeKind.Utc));
            DateRange range2 = DateRange.Create(
                new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc));

            bool result = range1.Overlaps(range2);

            Assert.False(result);
        }
    }
}
