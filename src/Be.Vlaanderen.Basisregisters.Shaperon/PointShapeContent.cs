namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class PointShapeContent : ShapeContent
    {
        public static readonly WordLength Length = new WordLength(10);

        public PointShapeContent(Point shape)
        {
            Shape = shape;
            ShapeType = ShapeType.Point;
            ContentLength = Length;
        }

        public Point Shape { get; }

        internal static ShapeContent ReadPointFromRecord(BinaryReader reader, ShapeRecordHeader header)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var x = reader.ReadDoubleLittleEndian();
            var y = reader.ReadDoubleLittleEndian();

            return new PointShapeContent(new Point(x, y));
        }

        public static ShapeContent ReadPoint(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeType = reader.ReadInt32LittleEndian();

            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            if ((ShapeType) shapeType == ShapeType.NullShape)
                return NullShapeContent.Instance;

            if ((ShapeType) shapeType != ShapeType.Point)
                throw new ShapeRecordContentException("The Shape Type field does not indicate a Point shape.");

            return ReadPointGeometry(reader);
        }

        internal static ShapeContent ReadPointGeometry(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return new PointShapeContent(
                new Point(
                    reader.ReadDoubleLittleEndian(),
                    reader.ReadDoubleLittleEndian()));
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteInt32LittleEndian((int) ShapeType); // Shape Type
            writer.WriteDoubleLittleEndian(Shape.X); // X Coordinate
            writer.WriteDoubleLittleEndian(Shape.Y); // Y Coordinate
        }
    }
}
