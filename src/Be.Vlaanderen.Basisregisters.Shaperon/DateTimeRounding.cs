namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    internal static class DateTimeRounding
    {
        public static DateTime RoundToSeconds(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0,
                value.Kind);
        }

        public static DateTime? RoundToSeconds(this DateTime? value)
        {
            return value?.RoundToSeconds();
        }

        public static DateTime RoundToDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0, value.Kind);
        }
    }
}
