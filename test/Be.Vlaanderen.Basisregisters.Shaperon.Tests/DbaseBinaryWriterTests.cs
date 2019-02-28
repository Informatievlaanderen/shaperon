namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Newtonsoft.Json;
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
            _fixture.CustomizeDbaseDateTime();
            _fixture.CustomizeDbaseBoolean();
            _fixture.CustomizeDbaseSingle();
            _fixture.CustomizeDbaseString();
            _fixture.CustomizeDbaseDouble();
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
                    foreach (var expectedRecord in expectedRecords)
                    {
                        sut.Write(expectedRecord);
                    }
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
    }
}
