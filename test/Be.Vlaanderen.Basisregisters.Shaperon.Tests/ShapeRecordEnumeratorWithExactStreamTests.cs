namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class ShapeRecordEnumeratorWithExactStreamTests
    {
        private readonly IEnumerator<ShapeRecord> _sut;
        private readonly DisposableBinaryReader _reader;
        private readonly ShapeRecord _record1;
        private readonly ShapeRecord _record2;

        public ShapeRecordEnumeratorWithExactStreamTests()
        {
            var fixture = new Fixture();
            fixture.CustomizeShapeRecordCount();
            fixture.CustomizeWordLength();

            var content = new PointShapeContent(new Point(1.0, 1.0));
            var number = RecordNumber.Initial;
            _record1 = content.RecordAs(number);
            _record2 = content.RecordAs(number.Next());
            var header = new ShapeFileHeader(
                ShapeFileHeader.Length.Plus(_record1.Length).Plus(_record2.Length),
                ShapeType.Point,
                fixture.Create<BoundingBox3D>());
            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                header.Write(writer);
                _record1.Write(writer);
                _record2.Write(writer);
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
            Assert.True(_sut.MoveNext());
            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void MoveNextRepeatedlyReturnsExpectedResult()
        {
            _sut.MoveNext();
            _sut.MoveNext();
            _sut.MoveNext();

            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            _sut.MoveNext();

            Assert.Equal(_record1, _sut.Current, new ShapeRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Equal(_record2, _sut.Current, new ShapeRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void EnumeratorCurrentReturnsExpectedResult()
        {

            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);

            _sut.MoveNext();

            Assert.Equal(_record1, (ShapeRecord) ((IEnumerator)_sut).Current, new ShapeRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Equal(_record2, (ShapeRecord) ((IEnumerator)_sut).Current, new ShapeRecordEqualityComparer());

            _sut.MoveNext();

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
