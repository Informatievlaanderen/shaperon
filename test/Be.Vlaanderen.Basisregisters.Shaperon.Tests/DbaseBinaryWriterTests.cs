namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;
    using Xunit.Abstractions;

    public class DbaseBinaryWriterTests
    {
        private readonly Fixture _fixture;

        public DbaseBinaryWriterTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeWordLength();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseField();
            _fixture.CustomizeDbaseCodePage();
            _fixture.CustomizeDbaseRecordCount(100);
            _fixture.CustomizeDbaseSchema();
            _fixture.CustomizeDbaseDate();
            _fixture.CustomizeDbaseLogical();
            _fixture.CustomizeDbaseFloat();
            _fixture.CustomizeDbaseCharacter();
            _fixture.CustomizeDbaseNumber();
            _fixture.Register<Stream>(() => new MemoryStream());
        }

        [Fact]
        public void HeaderOrWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Constructors.Select(() => new DbaseBinaryWriter(null, null)));
        }

        [Fact]
        public void PropertiesReturnExpectedValues()
        {
            var header = _fixture.Create<DbaseFileHeader>();
            using (var writer = new BinaryWriter(new MemoryStream()))
            using (var sut = new DbaseBinaryWriter(header, writer))
            {
                Assert.Same(header, sut.Header);
                Assert.Same(writer, sut.Writer);
            }
        }

        [Fact]
        public void WriteOneRecordCanNotBeNull()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            using (var stream = new MemoryStream())
            {
                using (var sut = new DbaseBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    Assert.Throws<ArgumentNullException>(() => sut.Write((DbaseRecord) null));
                }
            }
        }

        [Fact]
        public void WriteOneHasExpectedResult()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            var expectedRecord = _fixture.GenerateDbaseRecord(expectedHeader.Schema.Fields);
            using (var stream = new MemoryStream())
            {
                using (var sut = new DbaseBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    //Act
                    sut.Write(expectedRecord);
                }

                // Assert
                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var actualHeader = DbaseFileHeader.Read(reader);
                    var actualRecord = new AnonymousDbaseRecord(actualHeader.Schema.Fields);
                    actualRecord.Read(reader);
                    var actualEndOfFile = reader.ReadByte();

                    Assert.Equal(expectedHeader, actualHeader);
                    Assert.Equal(expectedRecord, actualRecord, new DbaseRecordEqualityComparer());
                    Assert.Equal(DbaseRecord.EndOfFile, actualEndOfFile);
                }
            }
        }

        [Fact]
        public void WriteOneWhenDisposedHasExpectedResult()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            var record = _fixture.GenerateDbaseRecord(expectedHeader.Schema.Fields);
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                using (var sut = new DbaseBinaryWriter(expectedHeader, writer))
                {
                    //Act
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => sut.Write(record));
                }
            }
        }

        [Fact]
        public void WriteManyRecordsCanNotBeNull()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            using (var stream = new MemoryStream())
            {
                using (var sut = new DbaseBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    Assert.Throws<ArgumentNullException>(() => sut.Write((IEnumerable<DbaseRecord>) null));
                }
            }
        }

        [Fact]
        public void WriteManyHasExpectedResult()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            var expectedRecords =
                _fixture.GenerateDbaseRecords(expectedHeader.Schema.Fields, expectedHeader.RecordCount);
            using (var stream = new MemoryStream())
            {
                using (var sut = new DbaseBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    //Act
                    sut.Write(expectedRecords);
                }

                // Assert
                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var actualHeader = DbaseFileHeader.Read(reader);
                    var actualRecords = new DbaseRecord[actualHeader.RecordCount.ToInt32()];
                    for (var index = 0; index < actualRecords.Length; index++)
                    {
                        var actualRecord = new AnonymousDbaseRecord(actualHeader.Schema.Fields);
                        actualRecord.Read(reader);
                        actualRecords[index] = actualRecord;
                    }

                    var actualEndOfFile = reader.ReadByte();

                    Assert.Equal(expectedHeader, actualHeader);
                    Assert.Equal(expectedRecords, actualRecords, new DbaseRecordEqualityComparer());
                    Assert.Equal(DbaseRecord.EndOfFile, actualEndOfFile);
                }
            }
        }

        [Fact]
        public void WritManyWhenDisposedHasExpectedResult()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            var records = _fixture.GenerateDbaseRecords(expectedHeader.Schema.Fields, expectedHeader.RecordCount);
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                using (var sut = new DbaseBinaryWriter(expectedHeader, writer))
                {
                    //Act
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => sut.Write(records));
                }
            }
        }

        [Fact]
        public void DisposeHasExpectedResult()
        {
            var expectedHeader = _fixture.Create<DbaseFileHeader>();
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, false))
                using (var sut = new DbaseBinaryWriter(expectedHeader, writer))
                {
                    //Act
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => writer.Write(_fixture.Create<byte>()));
                }
            }
        }
    }
}
