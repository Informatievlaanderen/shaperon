using System;
using System.IO;
using System.Linq;
using System.Text;
using Albedo;
using AutoFixture;
using AutoFixture.Idioms;
using Xunit;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class DbaseBooleanTests
    {
        private readonly Fixture _fixture;

        public DbaseBooleanTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseBoolean();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseBoolean(null)
            );
        }

        [Theory]
        [InlineData('n', false)]
        [InlineData('N', false)]
        [InlineData('f', false)]
        [InlineData('F', false)]
        [InlineData('y', true)]
        [InlineData('Y', true)]
        [InlineData('t', true)]
        [InlineData('T', true)]
        [InlineData('?', null)]
        [InlineData(' ', null)]
        public void CanReadAllValidRepresentations(char representation, bool? value)
        {
            var sut = _fixture.Create<DbaseBoolean>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(Convert.ToByte(representation));
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseBoolean(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseBoolean>();

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
                    var result = new DbaseBoolean(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteMultiple()
        {
            var sut1 = _fixture.Create<DbaseBoolean>();
            var sut2 = _fixture.Create<DbaseBoolean>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut1.Write(writer);
                    sut2.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result1 = new DbaseBoolean(sut1.Field);
                    var result2 = new DbaseBoolean(sut2.Field);
                    result1.Read(reader);
                    result2.Read(reader);

                    Assert.Equal(sut1.Field, result1.Field);
                    Assert.Equal(sut2.Field, result2.Field);
                    Assert.Equal(sut1.Value, result1.Value);
                    Assert.Equal(sut2.Value, result2.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseBoolean>();
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
                    var result = new DbaseBoolean(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseBoolean>();

            using (var stream = new MemoryStream())
            {
                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseBoolean(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }

        [Fact]
        public void CreateFailsIfFieldDecimalCountIsNot0()
        {
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0);

            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseBoolean(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            DbaseFieldType.Logical,
                            _fixture.Create<ByteOffset>(),
                            new DbaseFieldLength(1),
                            decimalCount
                        )
                    )
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotLogical()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Logical);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseBoolean(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            new DbaseFieldLength(1),
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void CreateFailsIfFieldLengthIsNot1()
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() != 1);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseBoolean(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            DbaseFieldType.Logical,
                            _fixture.Create<ByteOffset>(),
                            length,
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }


        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseBoolean>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseBoolean>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseBoolean>().Select(instance => instance.Write(null)));
        }
    }
}
