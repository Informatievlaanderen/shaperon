namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class DbaseSingle : DbaseFieldValue
    {
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = DbaseFloat.MaximumIntegerDigits;
        public static readonly DbaseFieldLength MaximumLength = DbaseFloat.MaximumLength;
        public static readonly DbaseFieldLength MinimumLength = DbaseFloat.MinimumLength;
        public static readonly DbaseFieldLength PositiveValueMinimumLength = DbaseFloat.PositiveValueMinimumLength;
        public static readonly DbaseFieldLength NegativeValueMinimumLength = DbaseFloat.NegativeValueMinimumLength;
        public static readonly DbaseDecimalCount MaximumDecimalCount = DbaseFloat.MaximumDecimalCount;

        private NumberFormatInfo Provider { get; }

        private float? _value;

        public DbaseSingle(DbaseField field) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Float)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be float to use it as a single field.", nameof(field));

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

        public DbaseSingle(DbaseField field, float value) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Float)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be float to use it as a single field.", nameof(field));

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

        public bool AcceptsValue(float value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
                return ((float) Math.Truncate(value)).ToString("F", Provider).Length <= Field.Length.ToInt32();

            var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
            var rounded = (float) Math.Round(value, digits);
            return rounded.ToString("F", Provider).Length <= Field.Length.ToInt32();
        }

        public float Value
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
                    var truncated = (float) Math.Truncate(value);
                    var length = truncated.ToString("F", Provider).Length;
                    if (length > Field.Length.ToInt32())
                        throw new ArgumentException(
                            $"The length ({length}) of the value ({truncated}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                    _value = truncated;
                }
                else
                {
                    var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
                    var rounded = (float) Math.Round(value, digits);
                    var roundedFormatted = rounded.ToString("F", Provider);
                    var length = roundedFormatted.Length;

                    if (length > Field.Length.ToInt32())
                        throw new ArgumentException(
                            $"The length ({length}) of the value ({roundedFormatted}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                    _value = float.Parse(roundedFormatted,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, Provider);
                }
            }
        }

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _value = reader.ReadAsNullableSingle(Field, Provider);
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableSingle(Field, Provider, _value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
