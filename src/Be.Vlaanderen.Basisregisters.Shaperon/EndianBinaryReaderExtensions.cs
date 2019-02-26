namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public static class EndianBinaryReaderExtensions
    {
        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (bytes.Length != 4)
            {
                throw new EndOfStreamException($"Unable to read beyond the end of the stream. Expected to be able to read 4 bytes but could only read {bytes.Length} bytes.");
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static double ReadDoubleBigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            if (bytes.Length != 8)
            {
                throw new EndOfStreamException($"Unable to read beyond the end of the stream. Expected to be able to read 8 bytes but could only read {bytes.Length} bytes.");
            }
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }

        public static int ReadInt32LittleEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (bytes.Length != 4)
            {
                throw new EndOfStreamException($"Unable to read beyond the end of the stream. Expected to be able to read 4 bytes but could only read {bytes.Length} bytes.");
            }
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        public static double ReadDoubleLittleEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(8);
            if (bytes.Length != 8)
            {
                throw new EndOfStreamException($"Unable to read beyond the end of the stream. Expected to be able to read 8 bytes but could only read {bytes.Length} bytes.");
            }
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToDouble(bytes, 0);
        }
    }
}
