namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class DbaseLogical : DbaseFieldValue
    {
        private const byte Bytet = (byte) 't';
        private const byte ByteT = (byte) 'T';
        private const byte Bytey = (byte) 'y';
        private const byte ByteY = (byte) 'Y';
        private const byte Bytef = (byte) 'f';
        private const byte ByteF = (byte) 'F';
        private const byte Byten = (byte) 'n';
        private const byte ByteN = (byte) 'N';
        private const byte ByteUnknown = (byte) '?';

        public DbaseLogical(DbaseField field, bool? value = null) : base(field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (field.FieldType != DbaseFieldType.Logical)
                throw new ArgumentException(
                    $"The field {field.Name}'s type must be logical to use it as a logical field.", nameof(field));

            if (field.DecimalCount.ToInt32() != 0)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s decimal count must be 0 to use it as a logical field.",
                    nameof(field));

            if (field.Length.ToInt32() != 1)
                throw new ArgumentException(
                    $"The logical field {field.Name}'s length must be 1 to use it as a logical field.",
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
                case Bytet:
                case ByteT:
                case Bytey:
                case ByteY:
                    Value = true;
                    break;

                case Bytef:
                case ByteF:
                case Byten:
                case ByteN:
                    Value = false;
                    break;

                default:
                    Value = default;
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
                    ? ByteT
                    : ByteF
                : ByteUnknown;

        public override void Accept(IDbaseFieldValueVisitor visitor) => visitor.Visit(this);
    }
}
