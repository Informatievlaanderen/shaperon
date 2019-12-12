namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class NullShapeContent : ShapeContent
    {
        public static readonly WordLength Length = ShapeTypeLength;
        public static readonly ShapeContent Instance = new NullShapeContent();

        private NullShapeContent()
        {
            ShapeType = ShapeType.NullShape;
            ContentLength = Length;
        }

        public static ShapeContent ReadNull(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeType = reader.ReadInt32LittleEndian();

            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            if ((ShapeType) shapeType != ShapeType.NullShape)
                throw new ShapeRecordContentException("The Shape Type field does not indicate a Null shape.");

            return Instance;
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteInt32LittleEndian((int) ShapeType); // Shape Type
        }
    }
}
