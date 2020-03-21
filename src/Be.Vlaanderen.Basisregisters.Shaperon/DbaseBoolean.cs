namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class DbaseBoolean : DbaseFieldValue
    {
        private bool? _value;

        public DbaseBoolean(DbaseField field) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Logical)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be logical to use it as a boolean field.", nameof(field));

            if (field.DecimalCount.ToInt32() != 0)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s decimal count must be 0 to use it as a boolean field.",
                    nameof(field));

            if (field.Length.ToInt32() != 1)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s length must be 1 to use it as a boolean field.",
                    nameof(field));

            _value = default;
        }

        public DbaseBoolean(DbaseField field, bool value) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Logical)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be logical to use it as a boolean field.", nameof(field));

            if (field.DecimalCount.ToInt32() != 0)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s decimal count must be 0 to use it as a boolean field.",
                    nameof(field));

            if (field.Length.ToInt32() != 1)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s length must be 1 to use it as a boolean field.",
                    nameof(field));

            Value = value;
        }

        public bool HasValue => _value.HasValue;

        public bool Value
        {
            get
            {
                if (!_value.HasValue)
                {
                    throw new FormatException($"The field {Field.Name} can not be null when read as non nullable datatype.");
                }

                return _value.Value;
            }
            set => _value = value;
        }

        public override void Reset() => _value = default;

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _value = reader.ReadAsNullableBoolean();
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableBoolean(_value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
