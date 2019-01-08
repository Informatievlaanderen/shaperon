namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class ShapeRecord
    {
        //Rationale: 100 byte file header means first record appears at offset 50 (16-bit word) of the mainfile.
        public static readonly WordOffset InitialOffset = new WordOffset(50);
        public static readonly WordLength HeaderLength = new ByteLength(8).ToWordLength();

        public ShapeRecord(ShapeRecordHeader header, ShapeContent content)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public ShapeRecordHeader Header { get; }
        public ShapeContent Content { get; }

        public WordLength Length => Content.ContentLength.Plus(HeaderLength);

        public static ShapeRecord Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var header = ShapeRecordHeader.Read(reader);
            var content = ShapeContent.ReadFromRecord(reader, header);

            return new ShapeRecord(header, content);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            Header.Write(writer);
            Content.Write(writer);
        }

        public byte[] ToBytes()
        {
            using (var output = new MemoryStream())
            {
                using (var writer = new BinaryWriter(output))
                {
                    Write(writer);
                    writer.Flush();
                }

                return output.ToArray();
            }
        }

        public ShapeIndexRecord IndexAt(WordOffset offset) => new ShapeIndexRecord(offset, Header.ContentLength);
    }
}
