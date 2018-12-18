using System;
using System.Globalization;
using System.IO;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class DbaseDateTime : DbaseFieldValue
    {
        private DateTime? _value;

        public DbaseDateTime(DbaseField field, DateTime? value = null) : base(field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (field.FieldType != DbaseFieldType.DateTime && field.FieldType != DbaseFieldType.Character)
            {
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be either datetime or character to use it as a datetime field.",
                    nameof(field));
            }

            Value = value;
        }

        public DateTime? Value
        {
            get => _value;
            set => _value =
                value.RoundToSeconds(); //Reason: due to serialization, precision is only guaranteed up to the second.
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
                var unpadded = reader.ReadRightPaddedString(Field.Length.ToInt32(), ' ');
                if (DateTime.TryParseExact(unpadded, "yyyyMMdd\\THHmmss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out DateTime parsed))
                {
                    Value = new DateTime(parsed.Year, parsed.Month, parsed.Day, parsed.Hour, parsed.Minute,
                        parsed.Second, DateTimeKind.Unspecified);
                }
                else
                {
                    Value = null;
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

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

        public override void Inspect(IDbaseFieldValueInspector writer)
        {
            writer.Inspect(this);
        }
    }
}
