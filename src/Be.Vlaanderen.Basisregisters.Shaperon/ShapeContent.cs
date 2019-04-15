namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Text;

    public abstract class ShapeContent
    {
        public static readonly WordLength ShapeTypeLength = new WordLength(2);

        public ShapeType ShapeType { get; protected set; }

        public WordLength ContentLength { get; protected set; }

        public static ShapeContent Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeType = reader.ReadInt32LittleEndian();
            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            var content = NullShapeContent.Instance;

            switch ((ShapeType) shapeType)
            {
                case ShapeType.NullShape:
                    break;

                case ShapeType.Point:
                    content = PointShapeContent.ReadPointGeometry(reader);
                    break;

                case ShapeType.PolyLineM:
                    content = PolyLineMShapeContent.ReadPolyLineMGeometry(reader);
                    break;

                case ShapeType.Polygon:
                    content = PolygonShapeContent.ReadPolygonGeometry(reader);
                    break;

                default:
                    throw new ShapeRecordContentException($"The Shape Type {shapeType} is currently not suppported.");
            }

            return content;
        }

        public static ShapeContent ReadAnonymous(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var shapeTypeAsBytes = reader.ReadBytes(4);
            var shapeType = EndianBitConverter.ToInt32LittleEndian(shapeTypeAsBytes);
            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            var content = NullShapeContent.Instance;

            switch ((ShapeType) shapeType)
            {
                case ShapeType.NullShape:
                    break;

                case ShapeType.Point:
                    content = PointShapeContent.ReadAnonymousPointGeometry(reader);
                    break;

                case ShapeType.PolyLineM:
                    content = PolyLineMShapeContent.ReadAnonymousPolyLineMGeometry(reader);
                    break;

                case ShapeType.Polygon:
                    content = PolygonShapeContent.ReadAnonymousPolygonGeometry(reader);
                    break;

                default:
                    throw new ShapeRecordContentException($"The Shape Type {shapeType} is currently not suppported.");
            }

            return content;
        }

        internal static ShapeContent ReadFromRecord(BinaryReader reader, ShapeRecordHeader header)
        {
            var shapeType = reader.ReadInt32LittleEndian();
            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeRecordContentException("The Shape Type field does not contain a known type of shape.");

            var content = NullShapeContent.Instance;
            switch ((ShapeType) shapeType)
            {
                case ShapeType.NullShape:
                    break;

                case ShapeType.Point:
                    content = PointShapeContent.ReadPointFromRecord(reader, header);
                    break;

                case ShapeType.PolyLineM:
                    content = PolyLineMShapeContent.ReadPolyLineMFromRecord(reader, header);
                    break;

                case ShapeType.Polygon:
                    content = PolygonShapeContent.ReadPolygonFromRecord(reader, header);
                    break;

                default:
                    throw new ShapeRecordContentException($"The Shape Type {shapeType} is currently not suppported.");
            }

            return content;
        }

        public abstract void Write(BinaryWriter writer);

        public ShapeRecord RecordAs(RecordNumber number)
            => new ShapeRecord(new ShapeRecordHeader(number, ContentLength), this);

        public ShapeContent Anonymous()
            => ReferenceEquals(NullShapeContent.Instance, this)
                ? this
                : new AnonymousShapeContent(ShapeType, ToBytes());

        public static ShapeContent FromBytes(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input))
                return Read(reader);
        }

        public static ShapeContent FromBytes(byte[] bytes, Encoding encoding)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input, encoding))
                return Read(reader);
        }

        public byte[] ToBytes()
        {
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output))
            {
                Write(writer);
                writer.Flush();
                return output.ToArray();
            }
        }

        public byte[] ToBytes(Encoding encoding)
        {
            using (var output = new MemoryStream())
            using (var writer = new BinaryWriter(output, encoding))
            {
                Write(writer);
                writer.Flush();
                return output.ToArray();
            }
        }
    }
}
