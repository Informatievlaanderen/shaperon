namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    internal static class DateTimeRounding
    {
        public static DateTime RoundToSeconds(this DateTime value) =>
            new DateTime(
                value.Year,
                value.Month,
                value.Day,
                value.Hour,
                value.Minute,
                value.Second,
                0,
                value.Kind);

        public static DateTime? RoundToSeconds(this DateTime? value) => value?.RoundToSeconds();

        public static DateTimeOffset RoundToSeconds(this DateTimeOffset value) =>
            new DateTimeOffset(
                value.Year,
                value.Month,
                value.Day,
                value.Hour,
                value.Minute,
                value.Second,
                0,
                value.Offset);

        public static DateTimeOffset? RoundToSeconds(this DateTimeOffset? value) => value?.RoundToSeconds();

        public static DateTime RoundToDay(this DateTime value) =>
            new DateTime(
                value.Year,
                value.Month,
                value.Day,
                0,
                0,
                0,
                0,
                value.Kind);

        public static DateTime? RoundToDay(this DateTime? value) => value?.RoundToDay();
    }
}
