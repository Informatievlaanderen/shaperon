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

        public override void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var charValue = reader.ReadByte();

            switch (charValue)
            {
                case DbaseLogicalBytes.Bytet:
                case DbaseLogicalBytes.ByteT:
                case DbaseLogicalBytes.Bytey:
                case DbaseLogicalBytes.ByteY:
                    Value = true;
                    break;

                case DbaseLogicalBytes.Bytef:
                case DbaseLogicalBytes.ByteF:
                case DbaseLogicalBytes.Byten:
                case DbaseLogicalBytes.ByteN:
                    Value = false;
                    break;

                default:
                    Value = null;
                    break;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var value = FormatAsByte(Value);
            writer.Write(value);
        }

        private static byte FormatAsByte(bool? value) =>
            value.HasValue
                ? value.Value
                    ? DbaseLogicalBytes.ByteT
                    : DbaseLogicalBytes.ByteF
                : DbaseLogicalBytes.ByteUnknown;

        public override void Accept(IDbaseFieldValueVisitor visitor) => (visitor as ITypedDbaseFieldValueVisitor)?.Visit(this);
    }
}
