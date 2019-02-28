namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class AnonymousDbaseRecordEnumeratorWithLongerStreamTests
    {
        private readonly IEnumerator<DbaseRecord> _sut;
        private readonly DisposableBinaryReader _reader;
        private readonly DbaseRecord _record;

        public AnonymousDbaseRecordEnumeratorWithLongerStreamTests()
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
                new DbaseRecordCount(1),
                new FakeDbaseSchema());
            var stream = new MemoryStream();
            long position;
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                header.Write(writer);
                writer.Flush();
                position = stream.Position;
                _record.Write(writer);
                writer.Write(DbaseRecord.EndOfFile);
                writer.Write(fixture.CreateMany<byte>(10).ToArray());
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
            Assert.False(_sut.MoveNext());
        }

        [Fact]
        public void CurrentReturnsExpectedResult()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Current);

            _sut.MoveNext();

            Assert.Equal(_record, _sut.Current, new DbaseRecordEqualityComparer());

            _sut.MoveNext();

            Assert.Throws<InvalidOperationException>(() => _sut.Current);
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
