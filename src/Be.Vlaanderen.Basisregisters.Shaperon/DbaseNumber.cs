namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class DbaseNumber : DbaseFieldValue
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

        public DbaseNumber(DbaseField field, double? value = null) : base(field)
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

        public bool AcceptsValue(double? value)
        {
            if (!value.HasValue)
                return true;

            if (Field.DecimalCount.ToInt32() == 0)
                return Math.Truncate(value.Value).ToString("F", Provider).Length <= Field.Length.ToInt32();

            var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
            var rounded = Math.Round(value.Value, digits);
            return rounded.ToString("F", Provider).Length <= Field.Length.ToInt32();
        }

        public bool AcceptsValue(int? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (value.HasValue)
                    return FormatAsString(value.Value).Length <= Field.Length.ToInt32();

                return true;
            }

            return false;
        }

        public bool AcceptsValue(short? value)
        {
            if (Field.DecimalCount.ToInt32() == 0)
            {
                if (value.HasValue)
                    return FormatAsString(value.Value).Length <= Field.Length.ToInt32();

                return true;
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

        public double? Value
        {
            get => _value;
            set
            {
                if (value.HasValue)
                {
                    if (Field.DecimalCount.ToInt32() == 0)
                    {
                        var truncated = Math.Truncate(value.Value);
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
                        var rounded = Math.Round(value.Value, digits);
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
                else
                {
                    _value = null;
                }
            }
        }

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
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count and its value can not be longer than {Field.Length}.");
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

                value = null;
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
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count and its value needs to be null or not longer than {Field.Length}.");
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
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count and its value can not be longer than {Field.Length}.");
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

                value = null;
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
                    throw new FormatException($"The field {Field.Name} needs to have 0 as decimal count and its value needs to be null or not longer than {Field.Length}.");
                }
            }
        }

        private static string FormatAsString(int value) => value.ToString(CultureInfo.InvariantCulture);

        private static string FormatAsString(short value) => value.ToString(CultureInfo.InvariantCulture);

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
                Value = null;
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');

                Value = double.TryParse(unpadded, NumberStyle, Provider, out var parsed)
                    ? (double?) parsed
                    : null;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (Value.HasValue)
            {
                var unpadded = Value.Value.ToString("F", Provider);
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

        public override void Accept(IDbaseFieldValueVisitor visitor) => visitor.Visit(this);
    }
}
