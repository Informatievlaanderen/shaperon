namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using Xunit;

    public class AnonymousDbaseRecordTests
    {
        private readonly Fixture _fixture;

        public AnonymousDbaseRecordTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseField();
        }

        [Fact]
        public void ConstructionUsingFieldsHasExpectedResult()
        {
            var fields = _fixture.CreateMany<DbaseField>().ToArray();
            var expectedValues = Array.ConvertAll(fields, field => field.CreateFieldValue());

            var sut = new AnonymousDbaseRecord(fields);

            Assert.False(sut.IsDeleted);
            Assert.Equal(expectedValues, sut.Values, new DbaseFieldValueEqualityComparer());
        }

        [Fact]
        public void ConstructionUsingValuesHasExpectedResult()
        {
            var fields = _fixture.CreateMany<DbaseField>().ToArray();
            var values = Array.ConvertAll(fields, field => field.CreateFieldValue());

            var sut = new AnonymousDbaseRecord(values);

            Assert.False(sut.IsDeleted);
            Assert.Same(values, sut.Values);
        }

        [Fact]
        public void ToBytesReturnsExpectedResult()
        {
            var fields = _fixture.CreateMany<DbaseField>().ToArray();
            var values = Array.ConvertAll(fields, field => field.CreateFieldValue());

            var sut = new AnonymousDbaseRecord(values);

            var result = sut.ToBytes(Encoding.UTF8);

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                sut.Write(writer);
                writer.Flush();

                Assert.Equal(stream.ToArray(), result);
            }
        }

        [Fact]
        public void FromBytesReturnsExpectedResult()
        {
            var fields = _fixture.CreateMany<DbaseField>().ToArray();

            var record = new AnonymousDbaseRecord(fields);
            var bytes = record.ToBytes(Encoding.UTF8);
            var sut = new AnonymousDbaseRecord(fields);
            sut.FromBytes(bytes, Encoding.UTF8);

            Assert.False(sut.IsDeleted);
            Assert.Equal(record.Values, sut.Values, new DbaseFieldValueEqualityComparer());
        }

        [Fact]
        public void ReadingCharacterWithDateTimeValueHasExpectedResult()
        {
            var value = _fixture.Create<DateTime>();
            var fields = new[]
            {
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.DateTime,
                    ByteOffset.Initial,
                    new DbaseFieldLength(15),
                    new DbaseDecimalCount(0)
                )
            };
            var values = new DbaseFieldValue[]
            {
                new DbaseDateTime(
                    fields[0],
                    value
                )
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    new AnonymousDbaseRecord(values).Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    fields[0] = new DbaseField(
                        fields[0].Name,
                        DbaseFieldType.Character,
                        ByteOffset.Initial,
                        new DbaseFieldLength(15),
                        new DbaseDecimalCount(0)
                    );

                    var sut = new AnonymousDbaseRecord(fields);

                    //Act
                    sut.Read(reader);

                    Assert.False(sut.IsDeleted);
                    Assert.Equal(new DbaseFieldValue[]
                    {
                        new DbaseDateTime(
                            fields[0],
                            value
                        )
                    }, sut.Values, new DbaseFieldValueEqualityComparer());
                }
            }
        }

        [Fact]
        public void ReadingCharacterWithDateTimeValueNullHasExpectedResult()
        {
            var fields = new[]
            {
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.DateTime,
                    ByteOffset.Initial,
                    new DbaseFieldLength(15),
                    new DbaseDecimalCount(0)
                )
            };
            var values = new DbaseFieldValue[]
            {
                new DbaseDateTime(
                    fields[0],
                    null
                )
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    new AnonymousDbaseRecord(values).Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    fields[0] = new DbaseField(
                        fields[0].Name,
                        DbaseFieldType.Character,
                        ByteOffset.Initial,
                        new DbaseFieldLength(15),
                        new DbaseDecimalCount(0)
                    );

                    var sut = new AnonymousDbaseRecord(fields);

                    //Act
                    sut.Read(reader);

                    Assert.False(sut.IsDeleted);
                    Assert.Equal(new DbaseFieldValue[]
                    {
                        new DbaseString(
                            fields[0],
                            ""
                        )
                    }, sut.Values, new DbaseFieldValueEqualityComparer());
                }
            }
        }

        [Fact]
        public void ReadingEndOfFileHasExpectedResult()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(DbaseRecord.EndOfFile);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var sut = new AnonymousDbaseRecord(new DbaseField[0]);

                    //Act
                    Assert.Throws<DbaseRecordException>(() => sut.Read(reader));
                }
            }
        }

        [Fact]
        public void ReadingUnacceptableIsDeletedFlagHasExpectedResult()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    var unacceptable = new Generator<byte>(_fixture).First(candidate => candidate != 0x20 && candidate != 0x2A);
                    writer.Write(unacceptable);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var sut = new AnonymousDbaseRecord(new DbaseField[0]);

                    //Act
                    Assert.Throws<DbaseRecordException>(() => sut.Read(reader));
                }
            }
        }
    }
}
