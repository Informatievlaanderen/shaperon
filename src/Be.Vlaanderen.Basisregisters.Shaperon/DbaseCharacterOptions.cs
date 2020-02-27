namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class DbaseCharacterOptions
    {
        public const string DefaultDateTimeFormat = "yyyyMMdd\\THHmmss";
        public const string DefaultDateTimeOffsetFormat = "yyyy-MM-dd\\THH:mm:ss%K";

        public static readonly DbaseCharacterOptions Default = new DbaseCharacterOptions(
            DefaultDateTimeFormat,
            DefaultDateTimeOffsetFormat
        );

        public DbaseCharacterOptions(string dateTimeFormat, string dateTimeOffsetFormat)
        {
            DateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat));
            DateTimeOffsetFormat = dateTimeOffsetFormat ?? throw new ArgumentNullException(nameof(dateTimeOffsetFormat));
        }

        public DbaseCharacterOptions WithDateTimeFormat(string format)
        {
            return new DbaseCharacterOptions(format, DateTimeOffsetFormat);
        }

        public string DateTimeFormat { get; }

        public DbaseCharacterOptions WithDateTimeOffsetFormat(string format)
        {
            return new DbaseCharacterOptions(DateTimeFormat, format);
        }

        public string DateTimeOffsetFormat { get; }
    }
}
