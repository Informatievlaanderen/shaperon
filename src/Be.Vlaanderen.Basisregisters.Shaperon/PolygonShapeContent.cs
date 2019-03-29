namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;

    public class PolygonShapeContent : ShapeContent
    {
        private static readonly ByteLength BoundingBoxByteLength = ByteLength.Double.Times(4); // MinX, MinY, MaxX, MaxY

        private static readonly ByteLength
            ContentHeaderLength =
                ByteLength
                    .Int32
                    .Times(3)
                    .Plus(BoundingBoxByteLength); // ShapeType, NumberOfParts, NumberOfPoints, BoundingBox

        public Polygon Shape { get; }

        public PolygonShapeContent(Polygon shape)
        {
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
            ShapeType = ShapeType.Polygon;

            ContentLength = ContentHeaderLength
                .Plus(ByteLength.Int32.Times(shape.NumberOfParts))
                .Plus(ByteLength.Double.Times(shape.NumberOfPoints).Times(2))
                .ToWordLength();
        }

        internal static ShapeContent ReadPolygonFromRecord(BinaryReader reader, ShapeRecordHeader header)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var box = new BoundingBox2D(
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian());
            var numberOfParts = reader.ReadInt32LittleEndian();
            var numberOfPoints = reader.ReadInt32LittleEndian();

            var parts = new int[numberOfParts];
            for (var partIndex = 0; partIndex < numberOfParts; partIndex++)
                parts[partIndex] = reader.ReadInt32LittleEndian();

            var points = new Point[numberOfPoints];
            for (var pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
                points[pointIndex] = new Point(
                    reader.ReadDoubleLittleEndian(),
                    reader.ReadDoubleLittleEndian());

            return new PolygonShapeContent(new Polygon(box, parts, points));
        }

        public static ShapeContent ReadPolygon(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeTypeAsInt32 = reader.ReadInt32LittleEndian();
            if (!Enum.IsDefined(typeof(ShapeType), shapeTypeAsInt32))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            var shapeType = (ShapeType)shapeTypeAsInt32;

            if (shapeType == ShapeType.NullShape)
                return NullShapeContent.Instance;

            if (shapeType != ShapeType.Polygon)
                throw new ShapeRecordContentException("The Shape Type field does not indicate a Polygon shape.");

            return ReadPolygonGeometry(reader);
        }

        internal static ShapeContent ReadPolygonGeometry(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var box = new BoundingBox2D(
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian(),
                reader.ReadDoubleLittleEndian());
            var numberOfParts = reader.ReadInt32LittleEndian();
            var numberOfPoints = reader.ReadInt32LittleEndian();

            var parts = new int[numberOfParts];
            for (var partIndex = 0; partIndex < numberOfParts; partIndex++)
                parts[partIndex] = reader.ReadInt32LittleEndian();

            var points = new Point[numberOfPoints];
            for (var pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
                points[pointIndex] = new Point(
                    reader.ReadDoubleLittleEndian(),
                    reader.ReadDoubleLittleEndian());

            return new PolygonShapeContent(new Polygon(box, parts, points));
        }

        internal static ShapeContent ReadAnonymousPolygonGeometry(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var content = EndianBitConverter
                .GetLittleEndianBytes((int)ShapeType.Polygon)
                .Concat(reader.ReadBytes(BoundingBoxByteLength.ToInt32()));

            var numberOfPartsAsBytes = reader.ReadBytes(4);
            var numberOfParts = EndianBitConverter.ToInt32LittleEndian(numberOfPartsAsBytes);
            var numberOfPointsAsBytes = reader.ReadBytes(4);
            var numberOfPoints = EndianBitConverter.ToInt32LittleEndian(numberOfPointsAsBytes);

            content = content
                .Concat(numberOfPartsAsBytes)
                .Concat(numberOfPointsAsBytes)
                .Concat(reader.ReadBytes(numberOfParts * 4))
                .Concat(reader.ReadBytes(numberOfPoints * 16));

            return new AnonymousShapeContent(ShapeType.Polygon, content.ToArray());
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            //TODO: If the shape is empty, emit null shape instead?

            writer.WriteInt32LittleEndian((int) ShapeType);

            writer.WriteDoubleLittleEndian(Shape.BoundingBox.XMin);
            writer.WriteDoubleLittleEndian(Shape.BoundingBox.YMin);
            writer.WriteDoubleLittleEndian(Shape.BoundingBox.XMax);
            writer.WriteDoubleLittleEndian(Shape.BoundingBox.YMax);
            writer.WriteInt32LittleEndian(Shape.NumberOfParts);
            writer.WriteInt32LittleEndian(Shape.NumberOfPoints);

            foreach (var part in Shape.Parts)
            {
                writer.WriteInt32LittleEndian(part);
            }

            foreach (var point in Shape.Points)
            {
                writer.WriteDoubleLittleEndian(point.X);
                writer.WriteDoubleLittleEndian(point.Y);
            }
        }
    }
}
