namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseDateTests
    {
        private readonly Fixture _fixture;

        public DbaseDateTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseDate();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseDate(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotDate()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Date);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseDate(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            new DbaseFieldLength(8),
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseDate>());
        }

        [Fact]
        public void DateTimeHoursMinutesSecondsAndMillisecondsAreRemovedUponConstruction()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Date,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(8),
                new DbaseDecimalCount(0));
            var sut = new DbaseDate(field, new DateTime(1, 1, 1, 1, 1, 1, 1));

            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), sut.Value);
        }

        [Fact]
        public void DateTimeHoursMinutesSecondsAndMillisecondsAreRemovedUponSet()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Date,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(8),
                new DbaseDecimalCount(0));
            var sut = new DbaseDate(field);

            sut.Value = new DateTime(1, 1, 1, 1, 1, 1, 1);

            Assert.Equal(new DateTime(1, 1, 1, 0, 0, 0, 0), sut.Value);
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDate>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDate>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseDate>();
            sut.Value = null;

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseDate(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseDate>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseDate(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseDate>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, 7)).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseDate(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }

        [Fact]
        public void ResetHasExpectedResult()
        {
            var sut = _fixture.Create<DbaseDate>();
            sut.Value = _fixture.Create<DateTime>();

            sut.Reset();

            Assert.Null(sut.Value);
        }
    }
}
