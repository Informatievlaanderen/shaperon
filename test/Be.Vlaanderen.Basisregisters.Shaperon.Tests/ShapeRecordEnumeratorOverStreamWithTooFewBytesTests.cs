namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class ShapeRecordEnumeratorOverStreamWithTooFewBytesTests
    {
        private readonly IEnumerator<ShapeRecord> _sut;
        private readonly DisposableBinaryReader _reader;
        private readonly ShapeRecord _record;

        public ShapeRecordEnumeratorOverStreamWithTooFewBytesTests()
        {
            var fixture = new Fixture();
            fixture.CustomizeShapeRecordCount();
            fixture.CustomizeWordLength();

            var content = new PointShapeContent(new Point(1.0, 1.0));
            var number = RecordNumber.Initial;
            _record = content.RecordAs(number);
            var header = new ShapeFileHeader(
                ShapeFileHeader.Length.Plus(_record.Length.Times(2)),
                ShapeType.Point,
                fixture.Create<BoundingBox3D>());
            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                header.Write(writer);
                _record.Write(writer);
                writer.Write(fixture.CreateMany<byte>(_record.Length.ToByteLength().ToInt32() / 2).ToArray());
                writer.Flush();
            }

            stream.Position = 100;

            _reader = new DisposableBinaryReader(stream, Encoding.UTF8, false);
            _sut = header.CreateShapeRecordEnumerator(_reader);
        }

        [Fact]
        public void MoveNextReturnsExpectedResult()
        {
            Assert.True(_sut.MoveNext());
            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());
        }

        [Fact]
        public void MoveNextRepeatedlyReturnsExpectedResult()
        {
            _sut.MoveNext();
            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());

            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            _sut.MoveNext();

            Assert.Equal(_record, _sut.Current, new ShapeRecordEqualityComparer());

            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void EnumeratorCurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);

            _sut.MoveNext();

            Assert.Equal(_record, (ShapeRecord) ((IEnumerator)_sut).Current, new ShapeRecordEqualityComparer());

            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());

            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);
        }

        [Fact]
        public void ResetReturnsExpectedResult()
        {
            Assert.Throws<NotSupportedException>(() => _sut.Reset());
        }

        [Fact]
        public void DisposeHasExpectedResult()
        {
            Assert.False(_reader.Disposed);
            _sut.Dispose();
            Assert.True(_reader.Disposed);
        }
    }
}
