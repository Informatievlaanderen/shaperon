namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;

    public class PolyLineMShapeContent : ShapeContent
    {
        private static readonly ByteLength MeasureRangeByteLength = ByteLength.Double.Times(2); // Min, Max
        private static readonly ByteLength BoundingBoxByteLength = ByteLength.Double.Times(4); // MinX, MinY, MaxX, MaxY

        private static readonly ByteLength
            ContentHeaderLength =
                ByteLength.Int32.Times(3)
                    .Plus(BoundingBoxByteLength); // ShapeType, NumberOfParts, NumberOfPoints, BoundingBox

        public MultiLineString Shape { get; }

        public PolyLineMShapeContent(MultiLineString shape)
        {
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
            ShapeType = ShapeType.PolyLineM;

            var numberOfParts = shape.NumGeometries;
            var numberOfPoints = shape.NumPoints;

            ContentLength = ContentHeaderLength
                .Plus(ByteLength.Int32.Times(numberOfParts)) // Parts
                .Plus(ByteLength.Double.Times(numberOfPoints * 2)) // Points(X,Y)
                .Plus(MeasureRangeByteLength)
                .Plus(ByteLength.Double.Times(numberOfPoints)) // Points(M)
                .ToWordLength();
        }

        internal static ShapeContent ReadPolyLineMFromRecord(BinaryReader reader, ShapeRecordHeader header)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            reader.ReadBytes(BoundingBoxByteLength.ToInt32()); // skip BoundingBox
            var numberOfParts = reader.ReadInt32LittleEndian();
            var numberOfPoints = reader.ReadInt32LittleEndian();
            var parts = new Int32[numberOfParts];
            for (var partIndex = 0; partIndex < numberOfParts; partIndex++)
            {
                parts[partIndex] = reader.ReadInt32LittleEndian();
            }

            var points = new PointM[numberOfPoints];
            for (var pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
            {
                points[pointIndex] = new PointM(
                    reader.ReadDoubleLittleEndian(),
                    reader.ReadDoubleLittleEndian()
                );
            }

            var contentLengthWithoutMeasures = ContentHeaderLength
                .Plus(ByteLength.Int32.Times(numberOfParts)) // Parts
                .Plus(ByteLength.Double.Times(numberOfPoints * 2)); // Points(X,Y)

            if (header.ContentLength > contentLengthWithoutMeasures)
            {
                reader.ReadBytes(MeasureRangeByteLength.ToInt32()); // skip MeasureRange
                for (var measureIndex = 0; measureIndex < numberOfPoints; measureIndex++)
                {
                    points[measureIndex].ChangeMeasurement(reader.ReadDoubleLittleEndian()); // Points[i].M
                }
            }

            var lines = new ILineString[numberOfParts];
            var toPointIndex = points.Length;
            for (var partIndex = numberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = parts[partIndex];
                lines[partIndex] = new LineString(
                    new PointSequence(new ArraySegment<PointM>(points, fromPointIndex, toPointIndex - fromPointIndex)),
                    GeometryConfiguration.GeometryFactory
                );
                toPointIndex = fromPointIndex;
            }

            return new PolyLineMShapeContent(new MultiLineString(lines));
        }

        public static ShapeContent ReadPolyLineM(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

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
            {
                throw new ArgumentNullException(nameof(reader));
            }

            reader.ReadBytes(BoundingBoxByteLength.ToInt32()); // skip BoundingBox
            var numberOfParts = reader.ReadInt32LittleEndian();
            var numberOfPoints = reader.ReadInt32LittleEndian();
            var parts = new Int32[numberOfParts];
            for (var partIndex = 0; partIndex < numberOfParts; partIndex++)
            {
                parts[partIndex] = reader.ReadInt32LittleEndian();
            }

            var points = new PointM[numberOfPoints];
            for (var pointIndex = 0; pointIndex < numberOfPoints; pointIndex++)
            {
                points[pointIndex] = new PointM(
                    reader.ReadDoubleLittleEndian(),
                    reader.ReadDoubleLittleEndian()
                );
            }

            if (reader.BaseStream.CanSeek && reader.BaseStream.Position != reader.BaseStream.Length)
            {
                reader.ReadBytes(MeasureRangeByteLength.ToInt32()); // skip MeasureRange
                for (var measureIndex = 0; measureIndex < numberOfPoints; measureIndex++)
                {
                    points[measureIndex].ChangeMeasurement(reader.ReadDoubleLittleEndian()); // Points[i].M
                }
            } //else try-catch-EndOfStreamException?? or only support seekable streams?

            var lines = new ILineString[numberOfParts];
            var toPointIndex = points.Length;
            for (var partIndex = numberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = parts[partIndex];
                lines[partIndex] = new LineString(
                    new PointSequence(new ArraySegment<PointM>(points, fromPointIndex, toPointIndex - fromPointIndex)),
                    GeometryConfiguration.GeometryFactory
                );
                toPointIndex = fromPointIndex;
            }

            return new PolyLineMShapeContent(new MultiLineString(lines));
        }

        internal static ShapeContent ReadAnonymousPolyLineMGeometry(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            IEnumerable<byte> content = EndianBitConverter
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
            {
                throw new ArgumentNullException(nameof(writer));
            }

            //TODO: If the shape is empty, emit null shape instead?

            writer.WriteInt32LittleEndian((int) ShapeType);

            var boundingBox = Shape.EnvelopeInternal;
            writer.WriteDoubleLittleEndian(boundingBox.MinX);
            writer.WriteDoubleLittleEndian(boundingBox.MinY);
            writer.WriteDoubleLittleEndian(boundingBox.MaxX);
            writer.WriteDoubleLittleEndian(boundingBox.MaxY);
            writer.WriteInt32LittleEndian(Shape.NumGeometries);
            writer.WriteInt32LittleEndian(Shape.NumPoints);

            var lineStrings = Shape
                .Geometries
                .Cast<LineString>()
                .ToArray();

            var offset = 0;
            foreach (var line in lineStrings)
            {
                writer.WriteInt32LittleEndian(offset);
                offset += line.NumPoints;
            }

            foreach (var point in lineStrings.SelectMany(line => line.Coordinates))
            {
                writer.WriteDoubleLittleEndian(point.X);
                writer.WriteDoubleLittleEndian(point.Y);
            }

            var measures = Shape.GetOrdinates(Ordinate.M).ToArray();
            var validMeasureValues = measures
                .Where(number => false == double.IsNaN(number))
                .ToArray();
            writer.WriteDoubleLittleEndian(validMeasureValues.DefaultIfEmpty(double.PositiveInfinity)
                .Min()); // Measure.Min
            writer.WriteDoubleLittleEndian(validMeasureValues.DefaultIfEmpty(double.NegativeInfinity)
                .Max()); // Measure.Max
            foreach (var measure in measures)
            {
                writer.WriteDoubleLittleEndian(measure); // Points[i].M
            }
        }
    }
}
