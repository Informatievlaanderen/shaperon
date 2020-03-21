namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class DbaseNullableBoolean : DbaseFieldValue
    {
        public DbaseNullableBoolean(DbaseField field, bool? value = null) : base(field)
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

        public bool? Value { get; set; }

        public override void Reset() => Value = default;

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Value = reader.ReadAsNullableBoolean();
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteAsNullableBoolean(Value);
        }

        public override void Accept(IDbaseFieldValueVisitor visitor) =>
            (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
