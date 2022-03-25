namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    internal static class Int32Extensions
    {
        public static int AsByteLengthValue(this int value)
        {
            var absolute = Math.Abs(value);
            return absolute == int.MaxValue
                ? 0
                : absolute % 2 == 0
                    ? absolute
                    : absolute + 1;
        }

        public static int AsMaximumFieldCount(this int value)
        {
            return new Random(value).Next(0, DbaseSchema.MaximumFieldCount + 1);
        }

        public static int AsRecordNumberValue(this int value)
        {
            var absolute = Math.Abs(value);
            return absolute == 0 ? 1 : absolute;
        }

        public static int AsDbaseFieldNameLength(this int value)
        {
            return new Random(value).Next(1, 12);
        }

        public static int AsDbaseFieldLengthValue(this int value)
        {
            return new Random(value).Next(1, 254);
        }

        public static int AsDbaseFieldLengthValue(this int value, int maximum)
        {
            return new Random(value).Next(1, Math.Min(255, maximum));
        }

        public static int AsDbaseRecordCountValue(this int value)
        {
            return new Random(value).Next(0, int.MaxValue);
        }

        public static int AsDbaseRecordCountValue(this int value, int maximum)
        {
            return new Random(value).Next(0, maximum);
        }

        public static int AsShapeRecordCountValue(this int value)
        {
            return new Random(value).Next(0, int.MaxValue / 4);
        }

        public static int AsShapeRecordCountValue(this int value, int maximum)
        {
            return new Random(value).Next(0, maximum);
        }

        public static int AsDbaseRecordLengthValue(this int value)
        {
            return new Random(value).Next(0, 128 * 255);
        }

        public static int AsDbaseDecimalCountValue(this int value)
        {
            return new Random(value).Next(0, 254);
        }

        public static int AsDbaseDecimalCountValue(this int value, int maximum)
        {
            return new Random(value).Next(0, Math.Min(255, maximum));
        }

        public static int AsDbaseIntegerDigitsValue(this int value)
        {
            return new Random(value).Next(0, 255);
        }

        public static int AsDbaseFieldCompatibleDbaseIntegerDigitsValue(this int value)
        {
            return new Random(value).Next(1, 255);
        }

        public static byte AsDbaseCodePageValue(this byte value)
        {
            return DbaseCodePage.All[new Random(value).Next(0, DbaseCodePage.All.Length)].ToByte();
        }
    }
}
