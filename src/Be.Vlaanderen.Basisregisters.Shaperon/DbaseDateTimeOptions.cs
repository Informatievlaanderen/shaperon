namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class DbaseDateTimeOptions
    {
        public const string DefaultDateTimeFormat = "yyyyMMdd\\THHmmss";

        public static readonly DbaseDateTimeOptions Default = new DbaseDateTimeOptions(
            DefaultDateTimeFormat
        );

        public DbaseDateTimeOptions(string dateTimeFormat)
        {
            DateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat));
        }

        public DbaseDateTimeOptions WithDateTimeFormat(string format)
        {
            return new DbaseDateTimeOptions(format);
        }

        public string DateTimeFormat { get; }
    }
}
