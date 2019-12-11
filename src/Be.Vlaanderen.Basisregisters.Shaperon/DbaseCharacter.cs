namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;

    public class DbaseCharacter : DbaseFieldValue
    {
        private string _value;

        public DbaseCharacter(DbaseField field, string value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a character field.", nameof(field));

            Value = value;
        }

        public bool AcceptsValue(string value)
        {
            if (value != null)
                return value.Length <= Field.Length.ToInt32();

            return true;
        }

        public string Value
        {
            get => _value;
            set
            {
                if (value != null && value.Length > Field.Length.ToInt32())
                    throw new ArgumentException(
                        $"The value length {value.Length} of field {Field.Name} is greater than its field length {Field.Length}.");

                _value = value;
            }
        }

        public bool TryGetValueAsDateTime(out DateTime? value)
        {
            if (Field.Length.ToInt32() == 15
                && Field.DecimalCount.ToInt32() == 0)
            {
                if (Value == null)
                {
                    value = null;
                    return true;
                }

                if (DateTime.TryParseExact(Value,
                    "yyyyMMdd\\THHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                    out var parsed))
                {
                    value = parsed;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsDateTime(DateTime? value)
        {
            if (Field.Length.ToInt32() == 15
                && Field.DecimalCount.ToInt32() == 0)
            {
                if (!value.HasValue)
                {
                    Value = null;
                    return true;
                }

                Value = value.Value.RoundToSeconds().ToString("yyyyMMdd\\THHmmss");
                return true;
            }

            return false;
        }

        public DateTime? ValueAsDateTime
        {
            get
            {
                if (!TryGetValueAsDateTime(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to be exactly 15 characters long, have 0 as decimal count, and its value needs to be null or an actual date time formatted as 'yyyyMMddTHHmmss'.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValueAsDateTime(value))
                {
                    throw new FormatException($"The field {Field.Name} needs to be exactly 15 characters long and have 0 as decimal count.");
                }
            }
        }

        public bool TryGetValueAsDateTimeOffset(out DateTimeOffset? value)
        {
            if (Field.Length.ToInt32() == 25
                && Field.DecimalCount.ToInt32() == 0)
            {
                if (Value == null)
                {
                    value = null;
                    return true;
                }

                if (DateTimeOffset.TryParseExact(Value,
                    "yyyy-MM-dd\\THH:mm:ss%K",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                    out var parsed))
                {
                    value = parsed;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool TrySetValueAsDateTimeOffset(DateTimeOffset? value)
        {
            if (Field.Length.ToInt32() == 25
                && Field.DecimalCount.ToInt32() == 0)
            {
                if (!value.HasValue)
                {
                    Value = null;
                    return true;
                }

                Value = value.Value.RoundToSeconds().ToString("yyyy-MM-ddTHH:mm:ss%K");
                return true;
            }

            return false;
        }

        public DateTimeOffset? ValueAsDateTimeOffset
        {
            get
            {
                if (!TryGetValueAsDateTimeOffset(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} needs to be exactly 25 characters long, have 0 as decimal count, and its value needs to be null or an actual date time with offset formatted as 'yyyy-MM-ddTHH:mm:ss%K'.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValueAsDateTimeOffset(value))
                {
                    throw new FormatException($"The field {Field.Name} needs to be exactly 25 characters long and have 0 as decimal count.");
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
                Value = null;
            }
            else
            {
                Value = reader.ReadRightPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (Value == null)
            {
                writer.Write(new byte[Field.Length.ToInt32()]);
            }
            else
            {
                writer.WriteRightPaddedString(Value, Field.Length.ToInt32(), ' ');
            }
        }

        public override void Accept(IDbaseFieldValueVisitor writer) => writer.Visit(this);
    }
}
