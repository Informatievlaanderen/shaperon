namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class AnonymousDbaseRecordEnumeratorWithShorterStreamTests
    {
        private readonly IDbaseRecordEnumerator _sut;
        private readonly DisposableBinaryReader _reader;
        private readonly DbaseRecord _record;

        public AnonymousDbaseRecordEnumeratorWithShorterStreamTests()
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

            _record = new FakeDbaseRecord {Id = {Value = fixture.Create<int>()}};
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
                _record.Write(writer);
                writer.Write(fixture.CreateMany<byte>(2).ToArray());
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
            Assert.True(_sut.MoveNext());
            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            _sut.MoveNext();

            Assert.Equal(_record, _sut.Current, new DbaseRecordEqualityComparer());

            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
        }

        [Fact]
        public void CurrentRecordNumberReturnsExpectedResult()
        {
            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            _sut.MoveNext();

            Assert.Equal(RecordNumber.Initial, _sut.CurrentRecordNumber);

            Assert.ThrowsAny<Exception>(() => _sut.MoveNext());

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
                    DbaseField.CreateInt32Field(new DbaseFieldName(nameof(Id)), new DbaseFieldLength(4))
                };
            }

            public DbaseField Id => Fields[0];
        }

        private class FakeDbaseRecord : DbaseRecord
        {
            private static readonly FakeDbaseSchema Schema = new FakeDbaseSchema();

            public FakeDbaseRecord()
            {
                Id = new DbaseInt32(Schema.Id);

                Values = new DbaseFieldValue[] {Id};
            }

            public DbaseInt32 Id { get; }
        }
    }
}
