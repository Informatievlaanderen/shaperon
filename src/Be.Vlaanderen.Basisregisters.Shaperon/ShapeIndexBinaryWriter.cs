namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ShapeIndexBinaryWriter : IDisposable
    {
        private bool _disposed;

        public ShapeIndexBinaryWriter(ShapeFileHeader header, BinaryWriter writer)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));

            Header.Write(Writer);
        }

        public ShapeFileHeader Header { get; }

        public BinaryWriter Writer { get; }

        public void Write(ShapeIndexRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            record.Write(Writer);
        }

        public void Write(IEnumerable<ShapeIndexRecord> records)
        {
            if (records == null)
                throw new ArgumentNullException(nameof(records));

            foreach (var record in records)
                record.Write(Writer);
        }

        public void Dispose()
        {
            if (!_disposed)
                return;

            Writer.Flush();
            Writer.Dispose();

            _disposed = true;
        }
    }
}
