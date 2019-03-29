namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;

    public class PolyLineMShapeContent : ShapeContent
    {
        private static readonly ByteLength MeasureRangeByteLength = ByteLength.Double.Times(2); // Min, Max
        private static readonly ByteLength BoundingBoxByteLength = ByteLength.Double.Times(4); // MinX, MinY, MaxX, MaxY

        private static readonly ByteLength
            ContentHeaderLength =
                ByteLength
                    .Int32
                    .Times(3)
                    .Plus(BoundingBoxByteLength); // ShapeType, NumberOfParts, NumberOfPoints, BoundingBox

        public PolyLineM Shape { get; }

        public PolyLineMShapeContent(PolyLineM shape)
        {
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
            ShapeType = ShapeType.PolyLineM;

            ContentLength = ContentHeaderLength
                .Plus(ByteLength.Int32.Times(shape.NumberOfParts)) // Parts
                .Plus(ByteLength.Double.Times(shape.NumberOfPoints * 2)) // Points(X,Y)
                .Plus(MeasureRangeByteLength)
                .Plus(ByteLength.Double.Times(shape.NumberOfPoints)) // Points(M)
                .ToWordLength();
        }

        internal static ShapeContent ReadPolyLineMFromRecord(BinaryReader reader, ShapeRecordHeader header)
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

            var contentLengthWithoutMeasures = ContentHeaderLength
                .Plus(ByteLength.Int32.Times(numberOfParts)) // Parts
                .Plus(ByteLength.Double.Times(numberOfPoints * 2)); // Points(X,Y)

            double[] measures;
            if (header.ContentLength > contentLengthWithoutMeasures)
            {
                measures = new double[numberOfPoints];
                reader.ReadBytes(MeasureRangeByteLength.ToInt32()); // skip MeasureRange
                for (var measureIndex = 0; measureIndex < numberOfPoints; measureIndex++)
                    measures[measureIndex] = reader.ReadDoubleLittleEndian();
            }
            else
            {
                measures = new double[0];
            }

            return new PolyLineMShapeContent(new PolyLineM(box, parts, points, measures));
        }

        public static ShapeContent ReadPolyLineM(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeTypeAsInt32 = reader.ReadInt32LittleEndian();
            if (!Enum.IsDefined(typeof(ShapeType), shapeTypeAsInt32))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            var shapeType = (ShapeType) shapeTypeAsInt32;

            if (shapeType == ShapeType.NullShape)
                return NullShapeContent.Instance;

            if (shapeType != ShapeType.PolyLineM)
                throw new ShapeRecordContentException("The Shape Type field does not indicate a PolyLineM shape.");

            return ReadPolyLineMGeometry(reader);
        }

        internal static ShapeContent ReadPolyLineMGeometry(BinaryReader reader)
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

            double[] measures;
            if (reader.BaseStream.CanSeek && reader.BaseStream.Position != reader.BaseStream.Length)
            {
                measures = new double[numberOfPoints];
                reader.ReadBytes(MeasureRangeByteLength.ToInt32()); // skip MeasureRange
                for (var measureIndex = 0; measureIndex < numberOfPoints; measureIndex++)
                    measures[measureIndex] = reader.ReadDoubleLittleEndian();
            }
            else
            {
                measures = new double[0];
            }

            return new PolyLineMShapeContent(new PolyLineM(box, parts, points, measures));
        }

        internal static ShapeContent ReadAnonymousPolyLineMGeometry(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var content = EndianBitConverter
                .GetLittleEndianBytes((int) ShapeType.PolyLineM)
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

            if (reader.BaseStream.CanSeek && reader.BaseStream.Position != reader.BaseStream.Length)
            {
                content = content
                    .Concat(reader.ReadBytes(MeasureRangeByteLength.ToInt32()))
                    .Concat(reader.ReadBytes(numberOfPoints * 8));
            } //else try-catch-EndOfStreamException?? or only support seekable streams?

            return new AnonymousShapeContent(ShapeType.PolyLineM, content.ToArray());
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

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

            writer.WriteDoubleLittleEndian(Shape.MeasureRange.Min);
            writer.WriteDoubleLittleEndian(Shape.MeasureRange.Max);
            foreach (var measure in Shape.Measures)
            {
                writer.WriteDoubleLittleEndian(measure);
            }
        }
    }
}
