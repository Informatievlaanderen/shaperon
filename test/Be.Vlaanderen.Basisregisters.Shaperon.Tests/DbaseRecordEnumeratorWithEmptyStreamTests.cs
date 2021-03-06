namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class DbaseRecordEnumeratorWithEmptyStreamTests
    {
        private readonly IDbaseRecordEnumerator<FakeDbaseRecord> _sut;
        private readonly DisposableBinaryReader _reader;

        public DbaseRecordEnumeratorWithEmptyStreamTests()
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
            _reader = new DisposableBinaryReader(new MemoryStream(), Encoding.UTF8, false);
            _sut = header.CreateDbaseRecordEnumerator<FakeDbaseRecord>(_reader);
        }

        [Fact]
        public void MoveNextReturnsExpectedResult()
        {
            Assert.Throws<EndOfStreamException>(() => _sut.MoveNext());
        }

        [Fact]
        public void MoveNextRepeatedlyReturnsExpectedResult()
        {
            Assert.Throws<EndOfStreamException>(() => _sut.MoveNext());

            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            Assert.Throws<EndOfStreamException>(() => _sut.MoveNext());

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void EnumeratorCurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);

            Assert.Throws<EndOfStreamException>(() => _sut.MoveNext());

            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);
        }

        [Fact]
        public void CurrentRecordNumberReturnsExpectedResult()
        {
            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            Assert.Throws<EndOfStreamException>(() => _sut.MoveNext());

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

        private class FakeDbaseSchema : DbaseSchema
        {
            public FakeDbaseSchema()
            {
                Fields = new[]
                {
                    DbaseField.CreateNumberField(new DbaseFieldName(nameof(Id)), new DbaseFieldLength(4), new DbaseDecimalCount(0))
                };
            }

            public DbaseField Id => Fields[0];
        }

        private class FakeDbaseRecord : DbaseRecord
        {
            private static readonly FakeDbaseSchema Schema = new FakeDbaseSchema();

            public FakeDbaseRecord()
            {
                Id = new DbaseNumber(Schema.Id);

                Values = new DbaseFieldValue[] {Id};
            }

            public DbaseNumber Id { get; }
        }
    }
}
