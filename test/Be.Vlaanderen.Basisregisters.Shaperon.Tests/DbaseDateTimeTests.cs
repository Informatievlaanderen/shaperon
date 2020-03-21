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

    public class DbaseDateTimeTests
    {
        private readonly Fixture _fixture;

        public DbaseDateTimeTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseDateTime();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseDateTime(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotCharacter()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Character);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseDateTime(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            new DbaseFieldLength(15),
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseDateTime>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDateTime>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDateTime>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWriteNull()
        {
            _fixture.CustomizeDbaseDateTimeWithoutValue();
            var sut = _fixture.Create<DbaseDateTime>();

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
                    var result = new DbaseDateTime(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Throws<FormatException>(() => sut.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseDateTime>();

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
                    var result = new DbaseDateTime(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseDateTime>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, 14)).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseDateTime(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }

        [Fact]
        public void HasValueReturnsExpectedResultWhenWithValue()
        {
            var sut = _fixture.Create<DbaseDateTime>();
            Assert.True(sut.HasValue);
        }

        [Fact]
        public void HasValueReturnsExpectedResultWhenWithoutValue()
        {
            _fixture.CustomizeDbaseDateTimeWithoutValue();
            var sut = _fixture.Create<DbaseDateTime>();
            Assert.False(sut.HasValue);
        }

        [Fact]
        public void ResetHasExpectedResult()
        {
            var sut = _fixture.Create<DbaseDateTime>();
            sut.Value = _fixture.Create<DateTime>();

            sut.Reset();

            Assert.False(sut.HasValue);
            Assert.Throws<FormatException>(() => sut.Value);
        }
    }
}
