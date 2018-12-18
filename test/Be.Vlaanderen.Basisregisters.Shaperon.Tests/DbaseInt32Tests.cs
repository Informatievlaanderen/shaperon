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
    using Xunit;
    using Xunit.Abstractions;

    public class DbaseInt32Tests
    {
        private readonly Fixture _fixture;

        private ITestOutputHelper _out;

        public DbaseInt32Tests(ITestOutputHelper @out)
        {
            _out = @out ?? throw new ArgumentNullException(nameof(@out));
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseInt32();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseInt32(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotNumberOrFloat()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Number && specimen != DbaseFieldType.Float);
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() > 0);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseInt32(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            length,
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void CreateFailsIfFieldDecimalCountIsNot0()
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() > 1);
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0 && specimen.ToInt32() < length.ToInt32());
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseInt32(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            _fixture.GenerateDbaseInt32FieldType(),
                            _fixture.Create<ByteOffset>(),
                            length,
                            decimalCount
                        )
                    )
            );
        }

        [Fact]
        public void LengthOfValueBeingSetCanNotExceedFieldLength()
        {
            var maxLength = new DbaseFieldLength(
                int.MaxValue.ToString(CultureInfo.InvariantCulture).Length - 1
                // because it's impossible to create a value longer than this (we need the test to generate a longer value)
            );
            var length = _fixture.GenerateDbaseInt32LengthLessThan(maxLength);
            _out.WriteLine("Length used is: {0}", length.ToString());

            var sut =
                new DbaseInt32(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        _fixture.GenerateDbaseInt32FieldType(),
                        _fixture.Create<ByteOffset>(),
                        length,
                        new DbaseDecimalCount(0)
                    )
                );

            var value = Enumerable
                .Range(0, sut.Field.Length.ToInt32())
                .Aggregate(1, (current, _) => current * 10);
            _out.WriteLine("Value used is: {0}", value.ToString());

            Assert.Throws<ArgumentException>(() => sut.Value = value);
        }

        [Fact]
        public void LengthOfNegativeValueBeingSetCanNotExceedFieldLength()
        {
            var maxLength = new DbaseFieldLength(
                int.MinValue.ToString(CultureInfo.InvariantCulture).Length - 1
                // because it's impossible to create a value longer than this (we need the test to generate a longer value)
            );
            var length = _fixture.GenerateDbaseInt32LengthLessThan(maxLength);

            var sut =
                new DbaseInt32(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        _fixture.GenerateDbaseInt32FieldType(),
                        _fixture.Create<ByteOffset>(),
                        length,
                        new DbaseDecimalCount(0)
                    )
                );

            var value = Enumerable
                .Range(0, sut.Field.Length.ToInt32())
                .Aggregate(-1, (current, _) => current * 10);

            Assert.Throws<ArgumentException>(() => sut.Value = value);
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseInt32>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseInt32>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseInt32>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseInt32>();
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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteNegative()
        {
            var value = Math.Abs(_fixture.Create<int>()) * -1;
            var sut = new DbaseInt32(
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    _fixture.GenerateDbaseInt32FieldType(),
                    ByteOffset.Initial,
                    new DbaseFieldLength(value.ToString(CultureInfo.InvariantCulture).Length),
                    new DbaseDecimalCount(0)
                ),
                value
            );

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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseInt32>();

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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }
    }
}
