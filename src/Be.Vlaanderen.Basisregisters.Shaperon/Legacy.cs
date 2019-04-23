namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    public abstract partial class DbaseRecord
    {
        [Obsolete("Will be removed in the future. Please use Read(BinaryReader) instead.", false)]
        public void FromBytes(byte[] bytes, Encoding encoding)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input, encoding))
                Read(reader);
        }

        [Obsolete("Will be removed in the future. Please use Write(BinaryWriter) instead.", false)]
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

    public abstract partial class ShapeContent
    {
        [Obsolete("Anonymous support will be deprecated in the future.", false)]
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


        [Obsolete("Anonymous support will be deprecated in the future.", false)]
        public ShapeContent Anonymous()
            => ReferenceEquals(NullShapeContent.Instance, this)
                ? this
                : new AnonymousShapeContent(ShapeType, ToBytes());

        [Obsolete("Will be removed in the future. Please use Read(BinaryReader) instead.", false)]
        public static ShapeContent FromBytes(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input))
                return Read(reader);
        }

        [Obsolete("Will be removed in the future. Please use Read(BinaryReader) instead.", false)]
        public static ShapeContent FromBytes(byte[] bytes, Encoding encoding)
        {
            using (var input = new MemoryStream(bytes))
            using (var reader = new BinaryReader(input, encoding))
                return Read(reader);
        }

        [Obsolete("Will be removed in the future. Please use Write(BinaryWriter) instead.", false)]
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

        [Obsolete("Will be removed in the future. Please use Write(BinaryWriter) instead.", false)]
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

    public partial class PointShapeContent
    {
        internal static ShapeContent ReadAnonymousPointGeometry(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var content = EndianBitConverter
                .GetLittleEndianBytes((int) ShapeType.Point)
                .Concat(reader.ReadBytes(16))
                .ToArray();

            return new AnonymousShapeContent(ShapeType.Point, content);
        }
    }

    public partial class PolygonShapeContent
    {
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
    }

    public partial class PolyLineMShapeContent
    {
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

    }

    public partial class ShapeRecord
    {
        [Obsolete("Will be removed in the future. Please use Write(BinaryWriter) instead.", false)]
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
    }

    [Obsolete("Anonymous support will be deprecated in the future.", false)]
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
