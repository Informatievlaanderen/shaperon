namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;

    public class DbaseDateTime : DbaseFieldValue
    {
        private string _value;

        public DbaseDateTime(DbaseField field, DbaseDateTimeOptions options = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a date time field.", nameof(field));

            Options = options ?? DbaseDateTimeOptions.Default;
            _value = default;
        }

        public DbaseDateTime(DbaseField field, DateTime value, DbaseDateTimeOptions options = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a date time field.", nameof(field));

            Options = options ?? DbaseDateTimeOptions.Default;
            Value = value;
        }

        public bool AcceptsValue(DateTime value)
        {
            var formatted = value.ToString(Options.DateTimeFormat);
            if (formatted.Length > Field.Length.ToInt32())
                return false;

            return true;
        }

        public DbaseDateTimeOptions Options { get; }

        private bool TryGetValue(out DateTime value)
        {
            if (_value == null)
            {
                value = default;
                return false;
            }

            if (DateTime.TryParseExact(_value,
                Options.DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite,
                out var parsed))
            {
                value = parsed;
                return true;
            }

            value = default;
            return false;
        }

        private bool TrySetValue(DateTime value)
        {
            var formatted = value.ToString(Options.DateTimeFormat);
            if (formatted.Length > Field.Length.ToInt32())
                return false;

            _value = formatted;
            return true;
        }

        public DateTime Value
        {
            get
            {
                if (!TryGetValue(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} its value needs to be null or an actual date time formatted as '{Options.DateTimeFormat}'.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValue(value))
                {
                    throw new FormatException($"The field {Field.Name} needs to be longer to hold an actual date time formatted as '{Options.DateTimeFormat}'.");
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
                _value = default;
            }
            else
            {
                _value = reader.ReadRightPaddedString(Field.Name.ToString(), Field.Length.ToInt32(), ' ');
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (_value == null)
            {
                writer.Write(new byte[Field.Length.ToInt32()]);
            }
            else
            {
                writer.WriteRightPaddedString(_value, Field.Length.ToInt32(), ' ');
            }
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
