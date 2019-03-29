namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Text;

    public abstract partial class DbaseRecord
    {
        public const byte EndOfFile = 0x1a;

        public bool IsDeleted { get; set; }

        public DbaseFieldValue[] Values { get; protected set; }

        public void Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var flag = reader.ReadByte();
            if (flag == EndOfFile)
                throw new EndOfStreamException("The end of file marker was reached.");

            if (flag != 0x20 && flag != 0x2A)
                throw new DbaseRecordException(
                    $"The record deleted flag must be either deleted (0x2A) or valid (0x20) but is 0x{flag:X2}");

            IsDeleted = flag == 0x2A;
            ReadValues(reader);
        }

        protected virtual void ReadValues(BinaryReader reader)
        {
            foreach (var value in Values)
                value.Read(reader);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(Convert.ToByte(IsDeleted ? 0x2A : 0x20));
            WriteValues(writer);
        }

        protected virtual void WriteValues(BinaryWriter writer)
        {
            foreach (var value in Values)
                value.Write(writer);
        }
    }
}
