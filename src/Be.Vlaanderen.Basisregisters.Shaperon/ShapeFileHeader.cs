namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class ShapeFileHeader
    {
        public const int ExpectedFileCode = 9994;
        public const int ExpectedVersion = 1000;

        public static readonly WordLength Length = new WordLength(50);

        public ShapeFileHeader(WordLength fileWordLength, ShapeType shapeType, BoundingBox3D boundingBox)
        {
            FileLength = fileWordLength;
            ShapeType = shapeType;
            BoundingBox = boundingBox;
        }

        public WordLength FileLength { get; }
        public ShapeType ShapeType { get; }
        public BoundingBox3D BoundingBox { get; }

        public bool Equals(ShapeFileHeader other) =>
            other != null
            && FileLength.Equals(other.FileLength)
            && ShapeType.Equals(other.ShapeType)
            && BoundingBox.Equals(other.BoundingBox);

        public override bool Equals(object obj)
            => obj is ShapeFileHeader other && Equals(other);

        public override int GetHashCode()
            => FileLength.GetHashCode() ^ ShapeType.GetHashCode() ^ BoundingBox.GetHashCode();

        public ShapeFileHeader ForIndex(ShapeRecordCount recordCount)
            => new ShapeFileHeader(
                Length.Plus(ShapeIndexRecord.Length.Times(recordCount.ToInt32())),
                ShapeType,
                BoundingBox);

        public static ShapeFileHeader Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.ReadInt32BigEndian() != 9994)
                throw new ShapeFileHeaderException("The File Code field does not match 9994.");

            reader.ReadBytes(20);

            var fileLength = reader.ReadInt32BigEndian();
            if (reader.ReadInt32LittleEndian() != ExpectedVersion)
                throw new ShapeFileHeaderException("The Version field does not match 1000.");

            var shapeType = reader.ReadInt32LittleEndian();
            if (!Enum.IsDefined(typeof(ShapeType), shapeType))
                throw new ShapeFileHeaderException("The Shape Type field does not contain a known type of shape.");

            var boundingBox = new BoundingBox3D(
                reader.ReadDoubleLittleEndian(), // xMin
                reader.ReadDoubleLittleEndian(), // yMin
                reader.ReadDoubleLittleEndian(), // xMax
                reader.ReadDoubleLittleEndian(), // yMax
                reader.ReadDoubleLittleEndian(), // zMin
                reader.ReadDoubleLittleEndian(), // zMax
                ParseNoData(reader.ReadDoubleLittleEndian()), // mMin
                ParseNoData(reader.ReadDoubleLittleEndian()) // mMax
            );

            return new ShapeFileHeader(new WordLength(fileLength), (ShapeType) shapeType, boundingBox);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteInt32BigEndian(9994);

            for (var index = 0; index < 20; index++)
                writer.Write((byte) 0x0);

            writer.WriteInt32BigEndian(FileLength.ToInt32());
            writer.WriteInt32LittleEndian(1000);
            writer.WriteInt32LittleEndian((int) ShapeType);
            writer.WriteDoubleLittleEndian(BoundingBox.XMin);
            writer.WriteDoubleLittleEndian(BoundingBox.YMin);
            writer.WriteDoubleLittleEndian(BoundingBox.XMax);
            writer.WriteDoubleLittleEndian(BoundingBox.YMax);
            writer.WriteDoubleLittleEndian(BoundingBox.ZMin);
            writer.WriteDoubleLittleEndian(BoundingBox.ZMax);
            writer.WriteDoubleLittleEndian(EscapeNoData(BoundingBox.MMin));
            writer.WriteDoubleLittleEndian(EscapeNoData(BoundingBox.MMax));
        }

        private static double ParseNoData(double value) => value < -10e38 ? double.NaN : value;

        private static double EscapeNoData(double value) => double.IsNaN(value) ? -10e39 : value;
    }
}
