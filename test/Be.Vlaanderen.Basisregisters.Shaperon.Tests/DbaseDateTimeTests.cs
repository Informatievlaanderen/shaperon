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
        public void CreateFailsIfFieldIsNotDateTimeOrCharacter()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.DateTime && specimen != DbaseFieldType.Character);
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
        public void DateTimeMillisecondsAreRemovedUponConstruction()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.DateTime,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(15),
                new DbaseDecimalCount(0));
            var sut = new DbaseDateTime(field, new DateTime(1, 1, 1, 1, 1, 1, 1));

            Assert.Equal(new DateTime(1, 1, 1, 1, 1, 1, 0), sut.Value);
        }

        [Fact]
        public void DateTimeMillisecondsAreRemovedUponSet()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.DateTime,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(15),
                new DbaseDecimalCount(0));
            var sut = new DbaseDateTime(field);

            sut.Value = new DateTime(1, 1, 1, 1, 1, 1, 1);

            Assert.Equal(new DateTime(1, 1, 1, 1, 1, 1, 0), sut.Value);
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
            var sut = _fixture.Create<DbaseDateTime>();
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
                    var result = new DbaseDateTime(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
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
    }
}
