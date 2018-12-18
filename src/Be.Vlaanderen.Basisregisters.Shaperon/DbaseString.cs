using System;
using System.Globalization;
using System.IO;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class DbaseString : DbaseFieldValue
    {
        private string _value;

        public DbaseString(DbaseField field, string value = null) : base(field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (field.FieldType != DbaseFieldType.Character)
            {
                throw new ArgumentException(
                    $"The field {field.Name} 's type must be character to use it as a string field.", nameof(field));
            }

            Value = value;
        }

        public bool AcceptsValue(string value)
        {
            if (value != null)
            {
                return value.Length <= Field.Length.ToInt32();
            }

            return true;
        }

        public string Value
        {
            get => _value;
            set
            {
                if (value != null && value.Length > Field.Length.ToInt32())
                {
                    throw new ArgumentException(
                        $"The value length {value.Length} of field {Field.Name} is greater than its field length {Field.Length}.");
                }

                _value = value;
            }
        }

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.PeekChar() == '\0')
            {
                reader.ReadBytes(Field.Length.ToInt32());
                Value = null;
            }
            else
            {
                Value = reader.ReadRightPaddedString(Field.Length.ToInt32(), ' ');
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (Value == null)
            {
                writer.Write(new byte[Field.Length.ToInt32()]);
            }
            else
            {
                writer.WriteRightPaddedString(Value, Field.Length.ToInt32(), ' ');
            }
        }

        internal DbaseFieldValue TryInferDateTime()
        {
            if (Value != null && Field.Length.ToInt32() == 15 && Field.DecimalCount.ToInt32() == 0 &&
                DateTime.TryParseExact(Value, "yyyyMMdd\\THHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out DateTime parsed))
            {
                return new DbaseDateTime(Field,
                    new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute, parsed.Second,
                        DateTimeKind.Unspecified));
            }

            return this;
        }

        public override void Inspect(IDbaseFieldValueInspector writer)
        {
            writer.Inspect(this);
        }
    }
}
