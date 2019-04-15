namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using System;
    using System.Collections.Generic;
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
                .Plus(ByteLength.Int32.Times(shape.NumInteriorRings + 1)) // Parts
                .Plus(ByteLength.Double.Times(shape.NumPoints * 2)) // Points(X,Y)
                .ToWordLength();
        }

        internal static ShapeContent ReadPolygonFromRecord(BinaryReader reader, ShapeRecordHeader header)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            reader.ReadBytes(BoundingBoxByteLength.ToInt32()); // skip BoundingBox
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

            var linearRings = new ILinearRing[numberOfParts];
            var toPointIndex = points.Length;

            for (var partIndex = numberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = parts[partIndex];

                linearRings[partIndex] = new LinearRing(
                    new CoordinateArraySequence(
                        points
                            .Skip(fromPointIndex)
                            .Take(toPointIndex - fromPointIndex)
                            .Select(x => new Coordinate(x.X, x.Y))
                            .ToArray()),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            return new PolygonShapeContent(new Polygon(linearRings[0], linearRings.Skip(1).ToArray()));
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

            reader.ReadBytes(BoundingBoxByteLength.ToInt32()); // skip BoundingBox
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

            var linearRings = new ILinearRing[numberOfParts];
            var toPointIndex = points.Length;

            for (var partIndex = numberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = parts[partIndex];

                linearRings[partIndex] = new LinearRing(
                    new CoordinateArraySequence(
                        points
                            .Skip(fromPointIndex)
                            .Take(toPointIndex - fromPointIndex)
                            .Select(x => new Coordinate(x.X, x.Y))
                            .ToArray()),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            return new PolygonShapeContent(new Polygon(linearRings[0], linearRings.Skip(1).ToArray()));
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

            if (reader.BaseStream.CanSeek && reader.BaseStream.Position != reader.BaseStream.Length)
            {
                content = content
                    .Concat(reader.ReadBytes(numberOfPoints * 8));
            } //else try-catch-EndOfStreamException?? or only support seekable streams?

            return new AnonymousShapeContent(ShapeType.Polygon, content.ToArray());
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            //TODO: If the shape is empty, emit null shape instead?

            writer.WriteInt32LittleEndian((int)ShapeType);

            var boundingBox = Shape.EnvelopeInternal;
            writer.WriteDoubleLittleEndian(boundingBox.MinX);
            writer.WriteDoubleLittleEndian(boundingBox.MinY);
            writer.WriteDoubleLittleEndian(boundingBox.MaxX);
            writer.WriteDoubleLittleEndian(boundingBox.MaxY);
            writer.WriteInt32LittleEndian(Shape.NumInteriorRings + 1);
            writer.WriteInt32LittleEndian(Shape.NumPoints);

            var lineStrings = new List<LineString>
            {
                (LineString) Shape.ExteriorRing
            };

            lineStrings.AddRange(
                Shape
                .InteriorRings
                .Cast<LineString>());

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
        }
    }
}
