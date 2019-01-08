namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class DbaseFileHeader
    {
        public const int MaximumFileSize = 1073741824; // 1 GB
        public const byte Terminator = 0x0d;
        public const byte ExpectedDbaseFormat = 0x03;
        public const int HeaderMetaDataSize = 33;
        public const int FieldMetaDataSize = 32;

        public DbaseFileHeader(
            DateTime lastUpdated,
            DbaseCodePage codePage,
            DbaseRecordCount recordCount,
            DbaseSchema schema)
        {
            LastUpdated = lastUpdated.RoundToDay();
            CodePage = codePage;
            RecordCount = recordCount;
            Schema = schema;
        }

        public DateTime LastUpdated { get; }
        public DbaseCodePage CodePage { get; }
        public DbaseRecordCount RecordCount { get; }
        public DbaseSchema Schema { get; }

        public bool Equals(DbaseFileHeader other) =>
            other != null &&
            LastUpdated.Equals(other.LastUpdated) &&
            CodePage.Equals(other.CodePage) &&
            RecordCount.Equals(other.RecordCount) &&
            Schema.Equals(other.Schema);

        public override bool Equals(object obj) =>
            obj is DbaseFileHeader dbaseFileHeader && Equals(dbaseFileHeader);

        public override int GetHashCode() =>
            LastUpdated.GetHashCode() ^
            CodePage.GetHashCode() ^
            RecordCount.GetHashCode() ^
            Schema.GetHashCode();

        public DbaseRecord CreateDbaseRecord() => new AnonymousDbaseRecord(Schema.Fields);

        public static DbaseFileHeader Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if (reader.ReadByte() != ExpectedDbaseFormat)
                throw new DbaseFileHeaderException("The database file type must be 3 (dBase III).");

            var lastUpdated = new DateTime(
                reader.ReadByte() + 1900,
                reader.ReadByte(),
                reader.ReadByte(),
                0,
                0,
                0,
                DateTimeKind.Unspecified);

            var recordCount = new DbaseRecordCount(reader.ReadInt32());
            var headerLength = reader.ReadInt16();
            var fieldCount = (headerLength - HeaderMetaDataSize) / FieldMetaDataSize;

            if (fieldCount > DbaseSchema.MaximumFieldCount)
                throw new DbaseFileHeaderException(
                    $"The database file can not contain more than {DbaseSchema.MaximumFieldCount} fields.");

            var recordLength = new DbaseRecordLength(reader.ReadInt16());
            reader.ReadBytes(17);
            var rawCodePage = reader.ReadByte();

            if (!DbaseCodePage.TryParse(rawCodePage, out var codePage))
                throw new DbaseFileHeaderException($"The database code page {rawCodePage} is not supported.");

            reader.ReadBytes(2);
            var fields = new DbaseField[fieldCount];
            for (var recordFieldIndex = 0; recordFieldIndex < fieldCount; recordFieldIndex++)
                fields[recordFieldIndex] = DbaseField.Read(reader);

            // verify field offsets are aligned
            var offset = ByteOffset.Initial;
            foreach (var field in fields)
            {
                if (field.Offset != offset)
                    throw new DbaseFileHeaderException(
                        $"The field {field.Name} does not have the expected offset {offset} but instead {field.Offset}. Please ensure the offset has been properly set for each field and that the order in which they appear in the record field layout matches their offset.");

                offset = field.Offset.Plus(field.Length);
            }

            var schema = new AnonymousDbaseSchema(fields);
            if (recordLength != schema.Length)
                throw new DbaseFileHeaderException(
                    $"The database file record length ({recordLength}) does not match the total length of all fields ({schema.Length}).");

            if (reader.ReadByte() != Terminator)
                throw new DbaseFileHeaderException("The database file header terminator is missing.");

            // skip to first record
            var bytesToSkip = headerLength - (HeaderMetaDataSize + FieldMetaDataSize * fieldCount);
            reader.ReadBytes(bytesToSkip);

            return new DbaseFileHeader(lastUpdated, codePage, recordCount, schema);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(Convert.ToByte(3));
            writer.Write(Convert.ToByte(LastUpdated.Year - 1900));
            writer.Write(Convert.ToByte(LastUpdated.Month));
            writer.Write(Convert.ToByte(LastUpdated.Day));
            writer.Write(RecordCount.ToInt32());
            var headerLength = HeaderMetaDataSize + FieldMetaDataSize * Schema.Fields.Length;
            writer.Write(Convert.ToInt16(headerLength));
            writer.Write(Schema.Length.ToInt16());
            writer.Write(new byte[17]);
            writer.Write(CodePage.ToByte());
            writer.Write(new byte[2]);

            foreach (var recordField in Schema.Fields)
                recordField.Write(writer);

            writer.Write(Terminator);
        }
    }
}
