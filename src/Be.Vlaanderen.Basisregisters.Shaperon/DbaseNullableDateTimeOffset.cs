namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;

    public class DbaseNullableDateTimeOffset : DbaseFieldValue
    {
        private string _value;

        public DbaseNullableDateTimeOffset(DbaseField field, DbaseDateTimeOffsetOptions options = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a date time offset field.", nameof(field));

            Options = options ?? DbaseDateTimeOffsetOptions.Default;
            _value = default;
        }

        public DbaseNullableDateTimeOffset(DbaseField field, DateTimeOffset? value, DbaseDateTimeOffsetOptions options = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Character)
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a date time offset field.", nameof(field));

            Options = options ?? DbaseDateTimeOffsetOptions.Default;
            Value = value;
        }

        public bool AcceptsValue(DateTimeOffset? value)
        {
            if (value.HasValue)
            {
                var formatted = value.Value.ToString(Options.DateTimeOffsetFormat);
                if (formatted.Length > Field.Length.ToInt32())
                    return false;
            }

            return true;
        }

        public DbaseDateTimeOffsetOptions Options { get; }

        public bool TryGetValue(out DateTimeOffset? value)
        {
            if (_value == null)
            {
                value = null;
                return true;
            }

            if (DateTimeOffset.TryParseExact(_value,
                Options.DateTimeOffsetFormat,
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

        public bool TrySetValue(DateTimeOffset? value)
        {
            if (!value.HasValue)
            {
                _value = default;
                return true;
            }

            var formatted = value.Value.ToString(Options.DateTimeOffsetFormat);
            if (formatted.Length > Field.Length.ToInt32())
                return false;

            _value = formatted;
            return true;
        }

        public DateTimeOffset? Value
        {
            get
            {
                if (!TryGetValue(out var parsed))
                {
                    throw new FormatException($"The field {Field.Name} its value needs to be null or an actual date time offset formatted as '{Options.DateTimeOffsetFormat}'.");
                }

                return parsed;
            }
            set
            {
                if (!TrySetValue(value))
                {
                    throw new FormatException($"The field {Field.Name} needs to be longer to hold an actual date time offset formatted as '{Options.DateTimeOffsetFormat}'.");
                }
            }
        }

        public override void Reset() => _value = default;

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _value = reader.ReadAsNullableString(Field);
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableString(Field, _value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
