namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
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
