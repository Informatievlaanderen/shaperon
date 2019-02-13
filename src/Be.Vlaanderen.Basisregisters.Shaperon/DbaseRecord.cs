namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Text;

    public abstract class DbaseRecord
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
                throw new DbaseRecordException("The end of file was reached unexpectedly.");

            if (flag != 0x20 && flag != 0x2A)
                throw new DbaseRecordException(
                    $"The record deleted flag must be either deleted (0x2A) or valid (0x20) but is 0x{flag:X2}");

            IsDeleted = flag == 0x2A;
            ReadValues(reader);
        }

        protected virtual void ReadValues(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            foreach (var value in Values)
                value.Read(reader);
        }

        public void FromBytes(byte[] bytes, Encoding encoding)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input, encoding))
                Read(reader);
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
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            foreach (var value in Values)
                value.Write(writer);
        }

        public byte[] ToBytes(Encoding encoding)
        {
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output, encoding))
            {
                Write(writer);
                writer.Flush();
                return output.ToArray();
            }
        }
    }
}
