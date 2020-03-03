namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    [Obsolete("Please use DbaseNumber or DbaseFloat instead.")]
    public class DbaseInt32 : DbaseFieldValue
    {
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = new DbaseIntegerDigits(10);

        private int? _value;

        public DbaseInt32(DbaseField field, int? value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Number && field.FieldType != DbaseFieldType.Float)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be either number or float to use it as a long integer field.",
                    nameof(field));

            if (field.DecimalCount.ToInt32() != 0)
                throw new ArgumentException(
                    $"The number field {field.Name}'s decimal count must be 0 to use it as a long integer field.",
                    nameof(field));

            Value = value;
        }

        public bool AcceptsValue(int? value)
        {
            if (value.HasValue)
                return FormatAsString(value.Value).Length <= Field.Length.ToInt32();

            return true;
        }

        public int? Value
        {
            get => _value;
            set
            {
                if (value.HasValue)
                {
                    var length = FormatAsString(value.Value).Length;

                    if (length > Field.Length.ToInt32())
                        throw new ArgumentException(
                            $"The value length {length} of field {Field.Name} is greater than its field length {Field.Length}.");
                }

                _value = value;
            }
        }

        private static string FormatAsString(int value) => value.ToString(CultureInfo.InvariantCulture);

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
                Value = default;
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');

                Value = int.TryParse(unpadded,
                    NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                    CultureInfo.InvariantCulture, out var parsed)
                    ? (int?) parsed
                    : null;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (Value.HasValue)
            {
                var unpadded = FormatAsString(Value.Value);
                writer.WriteLeftPaddedString(unpadded, Field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', Field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        public override void Accept(IDbaseFieldValueVisitor writer) => writer.Visit(this);
    }

    [Obsolete("Please use DbaseNumber or DbaseFloat instead.")]
    public class DbaseInt16 : DbaseFieldValue
    {
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = new DbaseIntegerDigits(5);

        private short? _value;

        public DbaseInt16(DbaseField field, short? value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Number && field.FieldType != DbaseFieldType.Float)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be either number or float to use it as a short integer field.",
                    nameof(field));

            if (field.DecimalCount.ToInt32() != 0)
                throw new ArgumentException(
                    $"The number field {field.Name}'s decimal count must be 0 to use it as a short integer field.",
                    nameof(field));

            Value = value;
        }

        public bool AcceptsValue(short? value)
        {
            if (value.HasValue)
                return FormatAsString(value.Value).Length <= Field.Length.ToInt32();

            return true;
        }

        public short? Value
        {
            get => _value;
            set
            {
                if (value.HasValue)
                {
                    var length = FormatAsString(value.Value).Length;

                    if (length > Field.Length.ToInt32())
                        throw new ArgumentException(
                            $"The value length {length} of field {Field.Name} is greater than its field length {Field.Length}.");
                }

                _value = value;
            }
        }

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
                Value = default;
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');

                Value = short.TryParse(unpadded,
                    NumberStyles.Integer | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                    CultureInfo.InvariantCulture, out var parsed)
                    ? (short?) parsed
                    : null;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (Value.HasValue)
            {
                var unpadded = FormatAsString(Value.Value);
                writer.WriteLeftPaddedString(unpadded, Field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', Field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        public override void Accept(IDbaseFieldValueVisitor writer) => writer.Visit(this);
    }

    [Obsolete("Please use DbaseCharacter instead.")]
    public class DbaseDateTime : DbaseFieldValue
    {
        private DateTime? _value;

        public DbaseDateTime(DbaseField field, DateTime? value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.DateTime && field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be either datetime or character to use it as a datetime field.",
                    nameof(field));

            Value = value;
        }

        public DateTime? Value
        {
            get => _value;
            set => _value = value.RoundToSeconds(); // Reason: due to serialization, precision is only guaranteed up to the second.
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
                Value = default;
            }
            else
            {
                var unpadded = reader.ReadRightPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');
                if (DateTime.TryParseExact(unpadded, "yyyyMMdd\\THHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out var parsed))
                {
                    Value = new DateTime(
                        parsed.Year,
                        parsed.Month,
                        parsed.Day,
                        parsed.Hour,
                        parsed.Minute,
                        parsed.Second,
                        DateTimeKind.Unspecified);
                }
                else
                {
                    Value = default;
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (Value.HasValue)
            {
                var unpadded = Value.Value.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                writer.WriteRightPaddedString(unpadded, Field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', Field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        public override void Accept(IDbaseFieldValueVisitor writer) => writer.Visit(this);
    }

    [Obsolete("Please use DbaseNumber instead.")]
    public class DbaseDecimal : DbaseFieldValue
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

        private decimal? _value;

        public DbaseDecimal(DbaseField field, decimal? value = null) : base(field)
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

        public bool AcceptsValue(decimal? value)
        {
            if (!value.HasValue)
                return true;

            if (Field.DecimalCount.ToInt32() == 0)
                return Math.Truncate(value.Value).ToString("F", Provider).Length <= Field.Length.ToInt32();

            var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
            var rounded = Math.Round(value.Value, digits);
            return rounded.ToString("F", Provider).Length <= Field.Length.ToInt32();

        }

        public decimal? Value
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
                            throw new ArgumentException(
                                $"The length ({length}) of the value ({truncated}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                        _value = truncated;
                    }
                    else
                    {
                        var digits = DbaseDecimalCount.Min(MaximumDecimalCount, Field.DecimalCount).ToInt32();
                        var rounded = Math.Round(value.Value, digits);
                        var roundedFormatted = rounded.ToString("F", Provider);
                        var length = roundedFormatted.Length;

                        if (length > Field.Length.ToInt32())
                            throw new ArgumentException(
                                $"The length ({length}) of the value ({roundedFormatted}) of field {Field.Name} is greater than its field length {Field.Length}, which would result in loss of precision.");

                        _value = decimal.Parse(roundedFormatted, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, Provider);
                    }
                }
                else
                {
                    _value = default;
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
                Value = default;
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');

                Value = decimal.TryParse(unpadded, NumberStyle, Provider, out var parsed)
                    ? (decimal?) parsed
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

        public override void Accept(IDbaseFieldValueVisitor writer) => writer.Visit(this);
    }

    public partial class DbaseField
    {
        [Obsolete("Please use DbaseField.CreateCharacterField instead")]
        public static DbaseField CreateStringField(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Character, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        [Obsolete("Please use DbaseField.CreateNumberField or DbaseField.CreateFloatField instead")]
        public static DbaseField CreateInt32Field(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        [Obsolete("Please use DbaseField.CreateNumberField or DbaseField.CreateFloatField instead")]
        public static DbaseField CreateInt16Field(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        [Obsolete("Please use DbaseField.CreateCharacterField instead")]
        public static DbaseField CreateDateTimeField(DbaseFieldName name)
            => new DbaseField(name, DbaseFieldType.DateTime, ByteOffset.Initial, new DbaseFieldLength(15), new DbaseDecimalCount(0));

        [Obsolete("Please use DbaseField.CreateNumberField instead")]
        public static DbaseField CreateDoubleField(DbaseFieldName name, DbaseFieldLength length, DbaseDecimalCount decimalCount)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, decimalCount);

        [Obsolete("Please use DbaseField.CreateFloatField instead")]
        public static DbaseField CreateSingleField(DbaseFieldName name, DbaseFieldLength length, DbaseDecimalCount decimalCount)
            => new DbaseField(name, DbaseFieldType.Float, ByteOffset.Initial, length, decimalCount);
    }

    public partial interface IDbaseFieldValueVisitor
    {
        [Obsolete("Please use DbaseNumber or DbaseFloat instead.")]
        void Visit(DbaseInt16 value);
        [Obsolete("Please use DbaseNumber or DbaseFloat instead.")]
        void Visit(DbaseInt32 value);
        [Obsolete("Please use DbaseCharacter instead.")]
        void Visit(DbaseDateTime value);
        [Obsolete("Please use DbaseNumber instead.")]
        void Visit(DbaseDecimal value);
    }
}
