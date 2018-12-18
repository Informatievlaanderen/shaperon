using System;
using System.IO;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class ShapeIndexRecord
    {
        public static readonly WordLength Length = new WordLength(4);
        public static readonly WordOffset InitialOffset = new WordOffset(50);

        public ShapeIndexRecord(WordOffset offset, WordLength contentLength)
        {
            Offset = offset;
            ContentLength = contentLength;
        }

        public WordOffset Offset { get; }
        public WordLength ContentLength { get; }

        public static ShapeIndexRecord Read(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var offset = new WordOffset(reader.ReadInt32BigEndian());
            var contentLength = new WordLength(reader.ReadInt32BigEndian());
            return new ShapeIndexRecord(offset, contentLength);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteInt32BigEndian(Offset.ToInt32());
            writer.WriteInt32BigEndian(ContentLength.ToInt32());
        }
    }
}
