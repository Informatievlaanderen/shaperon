namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class DbaseRecordEnumeratorWithExactStreamTests
    {
        private readonly IDbaseRecordEnumerator<FakeDbaseRecord> _sut;
        private readonly DisposableBinaryReader _reader;
        private readonly DbaseRecord _record1;
        private readonly DbaseRecord _record2;

        public DbaseRecordEnumeratorWithExactStreamTests()
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

            _record1 = new FakeDbaseRecord {Id = {Value = fixture.Create<int>()}};
            _record2 = new FakeDbaseRecord {Id = {Value = fixture.Create<int>()}};
            var header = new DbaseFileHeader(
                fixture.Create<DateTime>(),
                DbaseCodePage.Western_European_ANSI,
                new DbaseRecordCount(2),
                new FakeDbaseSchema());
            var stream = new MemoryStream();
            long position;
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                header.Write(writer);
                writer.Flush();
                position = stream.Position;
                _record1.Write(writer);
                _record2.Write(writer);
                writer.Write(DbaseRecord.EndOfFile);
                writer.Flush();
            }

            stream.Position = position;

            _reader = new DisposableBinaryReader(stream, Encoding.UTF8, false);
            _sut = header.CreateDbaseRecordEnumerator<FakeDbaseRecord>(_reader);
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

            Assert.Equal(_record1, _sut.Current, new DbaseRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Equal(_record2, _sut.Current, new DbaseRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void EnumeratorCurrentReturnsExpectedResult()
        {

            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);

            _sut.MoveNext();

            Assert.Equal(_record1, (DbaseRecord) ((IEnumerator)_sut).Current, new DbaseRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Equal(_record2, (DbaseRecord) ((IEnumerator)_sut).Current, new DbaseRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Throws<InvalidOperationException>(() => ((IEnumerator)_sut).Current);
        }


        [Fact]
        public void CurrentRecordNumberReturnsExpectedResult()
        {
            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            _sut.MoveNext();

            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            _sut.MoveNext();

            Assert.Equal(RecordNumber.Initial.Next(), _sut.CurrentRecordNumber);

            _sut.MoveNext();

            Assert.Equal(RecordNumber.Initial.Next(), _sut.CurrentRecordNumber);
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
