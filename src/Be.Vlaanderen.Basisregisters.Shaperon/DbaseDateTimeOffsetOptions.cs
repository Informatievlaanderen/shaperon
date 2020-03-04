namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class DbaseDateTimeOffsetOptions
    {
        public const string DefaultDateTimeOffsetFormat = "yyyy-MM-dd\\THH:mm:ss%K";

        public static readonly DbaseDateTimeOffsetOptions Default = new DbaseDateTimeOffsetOptions(
            DefaultDateTimeOffsetFormat
        );

        public DbaseDateTimeOffsetOptions(string dateTimeOffsetFormat)
        {
            DateTimeOffsetFormat = dateTimeOffsetFormat ?? throw new ArgumentNullException(nameof(dateTimeOffsetFormat));
        }

        public DbaseDateTimeOffsetOptions WithDateTimeOffsetFormat(string format)
        {
            return new DbaseDateTimeOffsetOptions(format);
        }

        public string DateTimeOffsetFormat { get; }
    }
}
