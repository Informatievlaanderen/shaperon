namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;

    public class DbaseNullableInt16 : DbaseFieldValue
    {
        public static readonly DbaseIntegerDigits MaximumIntegerDigits = DbaseInt16.MaximumIntegerDigits;

        private short? _value;

        public DbaseNullableInt16(DbaseField field, short? value = null) : base(field)
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
                return DbaseInt16.FormatAsString(value.Value).Length <= Field.Length.ToInt32();

            return true;
        }

        public short? Value
        {
            get => _value;
            set
            {
                if (value.HasValue)
                {
                    var length = DbaseInt16.FormatAsString(value.Value).Length;

                    if (length > Field.Length.ToInt32())
                        throw new FormatException(
                            $"The value length {length} of field {Field.Name} is greater than its field length {Field.Length}.");
                }

                _value = value;
            }
        }

        public override void Reset() => _value = default;

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Value = reader.ReadAsNullableInt16(Field);
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableInt16(Field, Value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
