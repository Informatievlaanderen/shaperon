namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class DbaseDouble : DbaseFieldValue
    {
        // REMARK: Actual max double integer digits is 308, field only supports 254, but number dbase type only supports 18.
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = new DbaseIntegerDigits(18);
        public static readonly DbaseFieldLength MaximumLength = new DbaseFieldLength(18);
        public static readonly DbaseFieldLength PositiveValueMinimumLength = new DbaseFieldLength(3); // 0.0
        public static readonly DbaseFieldLength NegativeValueMinimumLength = new DbaseFieldLength(4); // -0.0

        public static readonly DbaseFieldLength MinimumLength =
            DbaseFieldLength.Min(PositiveValueMinimumLength, NegativeValueMinimumLength);

        public static readonly DbaseDecimalCount MaximumDecimalCount = new DbaseDecimalCount(15);

        private const NumberStyles NumberStyle =
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingSign;

        private NumberFormatInfo Provider { get; }

        private double? _value;

        public DbaseDouble(DbaseField field) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Number)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be number to use it as a double field.", nameof(field));

            if (field.Length < MinimumLength || field.Length > MaximumLength)
                throw new ArgumentException(
                    $"The field {field.Name} 's length ({field.Length}) must be between {MinimumLength} and {MaximumLength}.",
                    nameof(field));

            Provider = new NumberFormatInfo
            {
                NumberDecimalDigits = DbaseDecimalCount.Min(MaximumDecimalCount, field.DecimalCount).ToInt32(),
                NumberDecimalSeparator = "."
            };

            _value = null;
        }

        public DbaseDouble(DbaseField field, double value) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Number)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be number to use it as a double field.", nameof(field));

            if (field.Length < MinimumLength || field.Length > MaximumLength)
                throw new ArgumentException(
                    $"The field {field.Name} 's length ({field.Length}) must be between {MinimumLength} and {MaximumLength}.",
                    nameof(field));

            Provider = new NumberFormatInfo
            {
                NumberDecimalDigits = DbaseDecimalCount.Min(MaximumDecimalCount, field.DecimalCount).ToInt32(),
                NumberDecimalSeparator = "."
            };

            Value = value;
        }

        public bool AcceptsValue(double value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
                return Math.Truncate(value).ToString("F", Provider).Length <= Field.Length.ToInt32();

            var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
            var rounded = Math.Round(value, digits);
            return rounded.ToString("F", Provider).Length <= Field.Length.ToInt32();
        }

        public double Value
        {
            get
            {
                if (!_value.HasValue)
                {
                    throw new FormatException($"The field {Field.Name} can not be null when read as non nullable datatype.");
                }

                return _value.Value;
            }
            set
            {
                if (Field.DecimalCount.ToInt32() == 0)
                {
                    var truncated = Math.Truncate(value);
                    var length = truncated.ToString("F", Provider).Length;
                    if (length > Field.Length.ToInt32())
                    {
                        throw new ArgumentException(
                            $"The length ({length}) of the value ({truncated}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");
                    }

                    _value = truncated;
                }
                else
                {
                    var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
                    var rounded = Math.Round(value, digits);
                    var roundedFormatted = rounded.ToString("F", Provider);
                    var length = roundedFormatted.Length;
                    if (length > Field.Length.ToInt32())
                    {
                        throw new ArgumentException(
                            $"The length ({length}) of the value ({roundedFormatted}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");
                    }

                    _value = double.Parse(roundedFormatted, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, Provider);
                }
            }
        }

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(Field.Length.ToInt32());
                if (read.Length != Field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {Field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {Field.Name.ToString()}."
                    );
                }
                _value = null;
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');

                _value = double.TryParse(unpadded, NumberStyle, Provider, out var parsed)
                    ? (double?) parsed
                    : null;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (_value.HasValue)
            {
                var unpadded = _value.Value.ToString("F", Provider);
                if (unpadded.Length < Field.Length.ToInt32() && Field.DecimalCount.ToInt32() > 0)
                {
                    // Pad with decimal zeros if space left.
                    var parts = unpadded.Split(Provider.NumberDecimalSeparator.Single());
                    if (parts.Length == 2 && parts[1].Length < Field.DecimalCount.ToInt32())
                    {
                        unpadded = string.Concat(
                            unpadded,
                            new string(
                                '0',
                                Field.DecimalCount.ToInt32() - parts[1].Length
                            )
                        );
                    }
                }

                writer.WriteLeftPaddedString(unpadded, Field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', Field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
