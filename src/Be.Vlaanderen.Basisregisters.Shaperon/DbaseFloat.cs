namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class DbaseFloat : DbaseFieldValue
    {
        // REMARK: Actual max single integer digits is 38, but float dbase type only supports 20.
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = new DbaseIntegerDigits(20);
        public static readonly DbaseFieldLength MaximumLength = new DbaseFieldLength(20);
        public static readonly DbaseFieldLength PositiveValueMinimumLength = new DbaseFieldLength(3); // 0.0
        public static readonly DbaseFieldLength NegativeValueMinimumLength = new DbaseFieldLength(4); // -0.0

        public static readonly DbaseFieldLength MinimumLength =
            DbaseFieldLength.Min(PositiveValueMinimumLength, NegativeValueMinimumLength);

        public static readonly DbaseDecimalCount MaximumDecimalCount = new DbaseDecimalCount(7);

        public const NumberStyles NumberStyle = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                                                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        public const string FixedPointFormatSpecifier = "F";

        private NumberFormatInfo Provider { get; }

        private float? _value;

        public DbaseFloat(DbaseField field, float? value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Float)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be float to use it as a float field.", nameof(field));

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

        public bool AcceptsValue(float? value)
        {
            if (!value.HasValue)
                return true;

            if (Field.DecimalCount.ToInt32() == 0)
                return ((float) Math.Truncate(value.Value)).ToString("F", Provider).Length <= Field.Length.ToInt32();

            var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
            var rounded = (float) Math.Round(value.Value, digits);
            return rounded.ToString("F", Provider).Length <= Field.Length.ToInt32();
        }

        public bool AcceptsValue(int? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                return !value.HasValue || AcceptsValue(value.Value);
            }

            return false;
        }

        public bool AcceptsValue(short? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                return !value.HasValue || AcceptsValue(value.Value);
            }

            return false;
        }

        public bool AcceptsValue(int value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                return FormatAsString(value).Length <= Field.Length.ToInt32();
            }

            return false;
        }

        public bool AcceptsValue(short value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                return FormatAsString(value).Length <= Field.Length.ToInt32();
            }

            return false;
        }

        public float? Value
        {
            get => _value;
            set
            {
                if (value.HasValue)
                {
                    if (Field.DecimalCount.ToInt32() == 0)
                    {
                        var truncated = (float) Math.Truncate(value.Value);
                        var length = truncated.ToString(FixedPointFormatSpecifier, Provider).Length;
                        if (length > Field.Length.ToInt32())
                            throw new ArgumentException(
                                $"The length ({length}) of the value ({truncated}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                        _value = truncated;
                    }
                    else
                    {
                        var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
                        var rounded = (float) Math.Round(value.Value, digits);
                        var roundedFormatted = rounded.ToString(FixedPointFormatSpecifier, Provider);
                        var length = roundedFormatted.Length;

                        if (length > Field.Length.ToInt32())
                            throw new ArgumentException(
                                $"The length ({length}) of the value ({roundedFormatted}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                        _value = float.Parse(roundedFormatted,
                            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, Provider);
                    }
                }
                else
                {
                    _value = default;
                }
            }
        }

        public override void Reset() => _value = default;

        public bool TryGetValueAsInt32(out int value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (_value.HasValue)
                {
                    var truncated = Math.Truncate(_value.Value);
                    if (truncated <= int.MaxValue && truncated >= int.MinValue)
                    {
                        value = Convert.ToInt32(truncated);
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsInt32(int value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                var length = FormatAsString(value).Length;

                if (length > Field.Length.ToInt32())
                    return false;

                _value = value;
                return true;
            }

            return false;
        }

        public int ValueAsInt32
        {
            get
            {
                if (!TryGetValueAsNullableInt32(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count.");
                }

                if (!parsed.HasValue)
                {
                    throw new FormatException($"The field {Field.Name} can not be null when read as non nullable datatype.");
                }

                return parsed.Value;
            }
            set
            {
                if (!TrySetValueAsInt32(value))
                {
                    throw new FormatException(
                        $"The field {Field.Name} needs to have 0 as decimal count and its value can not be longer than {Field.Length}.");
                }
            }
        }

        public bool TryGetValueAsNullableInt32(out int? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (_value.HasValue)
                {
                    var truncated = Math.Truncate(_value.Value);
                    if (truncated <= int.MaxValue && truncated >= int.MinValue)
                    {
                        value = Convert.ToInt32(truncated);
                        return true;
                    }

                    value = default;
                    return false;
                }

                value = default;
                return true;
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsNullableInt32(int? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (value.HasValue)
                {
                    var length = FormatAsString(value.Value).Length;

                    if (length > Field.Length.ToInt32())
                        return false;
                }

                _value = value;
                return true;
            }

            return false;
        }

        public int? ValueAsNullableInt32
        {
            get
            {
                if (!TryGetValueAsNullableInt32(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValueAsNullableInt32(value))
                {
                    throw new FormatException(
                        $"The field {Field.Name} needs to have 0 as decimal count and its value needs to be null or not longer than {Field.Length}.");
                }
            }
        }

        public bool TryGetValueAsInt16(out short value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (_value.HasValue)
                {
                    var truncated = Math.Truncate(_value.Value);
                    if (truncated <= short.MaxValue && truncated >= short.MinValue)
                    {
                        value = Convert.ToInt16(truncated);
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsInt16(short value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                var length = FormatAsString(value).Length;

                if (length > Field.Length.ToInt32())
                    return false;

                _value = value;
                return true;
            }

            return false;
        }

        public short ValueAsInt16
        {
            get
            {
                if (!TryGetValueAsNullableInt16(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count.");
                }

                if (!parsed.HasValue)
                {
                    throw new FormatException($"The field {Field.Name} can not be null when read as non nullable datatype.");
                }

                return parsed.Value;
            }
            set
            {
                if (!TrySetValueAsInt16(value))
                {
                    throw new FormatException(
                        $"The field {Field.Name} needs to have 0 as decimal count and its value can not be longer than {Field.Length}.");
                }
            }
        }

        public bool TryGetValueAsNullableInt16(out short? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (_value.HasValue)
                {
                    var truncated = Math.Truncate(_value.Value);
                    if (truncated <= short.MaxValue && truncated >= short.MinValue)
                    {
                        value = Convert.ToInt16(truncated);
                        return true;
                    }

                    value = default;
                    return false;
                }

                value = default;
                return true;
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsNullableInt16(short? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (value.HasValue)
                {
                    var length = FormatAsString(value.Value).Length;

                    if (length > Field.Length.ToInt32())
                        return false;
                }

                _value = value;
                return true;
            }

            return false;
        }

        public short? ValueAsNullableInt16
        {
            get
            {
                if (!TryGetValueAsNullableInt16(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValueAsNullableInt16(value))
                {
                    throw new FormatException(
                        $"The field {Field.Name} needs to have 0 as decimal count and its value needs to be null or not longer than {Field.Length}.");
                }
            }
        }

        private static string FormatAsString(int value) => value.ToString(CultureInfo.InvariantCulture);

        private static string FormatAsString(short value) => value.ToString(CultureInfo.InvariantCulture);

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Value = reader.ReadAsNullableSingle(Field, Provider);
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableSingle(Field, Provider, Value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => visitor.Visit(this);
    }
}
