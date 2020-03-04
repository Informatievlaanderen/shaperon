namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class DbaseDouble : DbaseFieldValue
    {
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = DbaseNumber.MaximumIntegerDigits;
        public static readonly DbaseFieldLength MaximumLength = DbaseNumber.MaximumLength;
        public static readonly DbaseFieldLength MinimumLength = DbaseNumber.MinimumLength;
        public static readonly DbaseFieldLength PositiveValueMinimumLength = DbaseNumber.PositiveValueMinimumLength;
        public static readonly DbaseFieldLength NegativeValueMinimumLength = DbaseNumber.NegativeValueMinimumLength;
        public static readonly DbaseDecimalCount MaximumDecimalCount = DbaseNumber.MaximumDecimalCount;

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

            _value = default;
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
                    var length = truncated.ToString(DbaseNumber.FixedPointFormatSpecifier, Provider).Length;
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
                    var roundedFormatted = rounded.ToString(DbaseNumber.FixedPointFormatSpecifier, Provider);
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

            _value = reader.ReadAsNullableDouble(Field, Provider);
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableDouble(Field, Provider, _value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
