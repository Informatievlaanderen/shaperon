namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class ShapeRecordEnumeratorOverStreamWithoutBytesTests
    {
        private readonly IEnumerator<ShapeRecord> _sut;
        private readonly DisposableBinaryReader _reader;

        public ShapeRecordEnumeratorOverStreamWithoutBytesTests()
        {
            var fixture = new Fixture();
            fixture.CustomizeShapeRecordCount();
            fixture.CustomizeWordLength();

            var header = new ShapeFileHeader(
                fixture.Create<WordLength>(),
                fixture.Create<ShapeType>(),
                fixture.Create<BoundingBox3D>());
            _reader = new DisposableBinaryReader(new MemoryStream(), Encoding.UTF8, false);
            _sut = header.CreateShapeRecordEnumerator(_reader);
        }

        [Fact]
        public void MoveNextReturnsExpectedResult()
        {
            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void MoveNextRepeatedlyReturnsExpectedResult()
        {
            _sut.MoveNext();

            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            _sut.MoveNext();

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void EnumeratorCurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);

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
