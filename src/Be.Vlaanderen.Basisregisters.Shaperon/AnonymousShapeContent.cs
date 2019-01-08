namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class AnonymousShapeContent : ShapeContent
    {
        internal AnonymousShapeContent(ShapeType shapeType, byte[] content)
        {
            ShapeType = shapeType;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentLength = new ByteLength(content.Length).ToWordLength();
        }

        public byte[] Content { get; }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(Content);
        }
    }
}
