namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class AnonymousDbaseRecordEnumeratorOverStreamWithEndOfFileTests
    {
        private readonly IDbaseRecordEnumerator _sut;
        private readonly DisposableBinaryReader _reader;

        public AnonymousDbaseRecordEnumeratorOverStreamWithEndOfFileTests()
        {
            var fixture = new Fixture();
            fixture.CustomizeWordLength();
            fixture.CustomizeDbaseFieldName();
            fixture.CustomizeDbaseFieldLength();
            fixture.CustomizeDbaseDecimalCount();
            fixture.CustomizeDbaseField();
            fixture.CustomizeDbaseCodePage();
            fixture.CustomizeDbaseRecordCount();
            fixture.CustomizeDbaseSchema();

            var header = fixture.Create<DbaseFileHeader>();
            var stream = new MemoryStream();
            long position;
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                header.Write(writer);
                writer.Flush();
                position = stream.Position;
                writer.Write(DbaseRecord.EndOfFile);
                writer.Flush();
            }

            stream.Position = position;
            _reader = new DisposableBinaryReader(stream, Encoding.UTF8, false);
            _sut = header.CreateAnonymousDbaseRecordEnumerator(_reader);
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
        public void CurrentRecordNumberReturnsExpectedResult()
        {
            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            _sut.MoveNext();

            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);
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
