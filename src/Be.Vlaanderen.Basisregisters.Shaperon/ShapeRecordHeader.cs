namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class ShapeRecordHeader
    {
        public ShapeRecordHeader(RecordNumber recordNumber, WordLength contentLength)
        {
            RecordNumber = recordNumber;
            ContentLength = contentLength;
        }

        public RecordNumber RecordNumber { get; }
        public WordLength ContentLength { get; }

        public bool Equals(ShapeRecordHeader other) =>
            other != null
            && RecordNumber.Equals(other.RecordNumber)
            && ContentLength.Equals(other.ContentLength);

        public override bool Equals(object obj) => obj is ShapeRecordHeader other && Equals(other);
        public override int GetHashCode() => RecordNumber.GetHashCode() ^ ContentLength.GetHashCode();

        public static ShapeRecordHeader Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var recordNumber = new RecordNumber(reader.ReadInt32BigEndian());
            var contentLength = new WordLength(reader.ReadInt32BigEndian());
            return new ShapeRecordHeader(recordNumber, contentLength);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteInt32BigEndian(RecordNumber.ToInt32());
            writer.WriteInt32BigEndian(ContentLength.ToInt32());
        }
    }
}
