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

    public class DbaseNullableDoubleTests
    {
        private readonly Fixture _fixture;

        public DbaseNullableDoubleTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength(DbaseNullableDouble.MaximumLength);
            _fixture.CustomizeDbaseDecimalCount(DbaseNullableDouble.MaximumDecimalCount);
            _fixture.CustomizeDbaseNullableDouble();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void MaximumDecimalCountReturnsExpectedValue()
        {
            Assert.Equal(new DbaseDecimalCount(15), DbaseNullableDouble.MaximumDecimalCount);
        }

        [Fact]
        public void MaximumIntegerDigitsReturnsExpectedValue()
        {
            Assert.Equal(new DbaseIntegerDigits(18), DbaseNullableDouble.MaximumIntegerDigits);
        }

        [Fact]
        public void MaximumLengthReturnsExpectedValue()
        {
            Assert.Equal(new DbaseFieldLength(18), DbaseNullableDouble.MaximumLength);
        }

        [Fact]
        public void PositiveValueMinimumLengthReturnsExpectedValue()
        {
            Assert.Equal(new DbaseFieldLength(3), DbaseNullableDouble.PositiveValueMinimumLength);
        }

        [Fact]
        public void NegativeValueMinimumLengthReturnsExpectedValue()
        {
            Assert.Equal(new DbaseFieldLength(4), DbaseNullableDouble.NegativeValueMinimumLength);
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseNullableDouble(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotNumber()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Number);
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseNullableDouble(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            length,
                            decimalCount
                        )
                    )
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(19)]
        [InlineData(254)]
        public void CreateFailsIfFieldLengthIsOutOfRange(int outOfRange)
        {
            var length = new DbaseFieldLength(outOfRange);
            var decimalCount = new DbaseDecimalCount(0);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseNullableDouble(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            DbaseFieldType.Number,
                            _fixture.Create<ByteOffset>(),
                            length,
                            decimalCount
                        )
                    )
            );
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseNullableDouble>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseNullableDouble>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseNullableDouble>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void LengthOfPositiveValueBeingSetCanNotExceedFieldLength()
        {
            var length = DbaseNullableDouble.MaximumLength;
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);

            var sut =
                new DbaseNullableDouble(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        DbaseFieldType.Number,
                        _fixture.Create<ByteOffset>(),
                        length,
                        decimalCount
                    )
                );

            Assert.Throws<ArgumentException>(() => sut.Value = double.MaxValue);
        }

        [Fact]
        public void LengthOfNegativeValueBeingSetCanNotExceedFieldLength()
        {
            var length = DbaseNullableDouble.MaximumLength;
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);

            var sut =
                new DbaseNullableDouble(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        DbaseFieldType.Number,
                        _fixture.Create<ByteOffset>(),
                        length,
                        decimalCount
                    )
                );

            Assert.Throws<ArgumentException>(() => sut.Value = double.MinValue);
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseNullableDouble>();
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
                    var result = new DbaseNullableDouble(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteNegative()
        {
            using (var random = new PooledRandom())
            {
                var sut = new DbaseNullableDouble(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        DbaseFieldType.Number,
                        _fixture.Create<ByteOffset>(),
                        DbaseNullableDouble.NegativeValueMinimumLength,
                        new DbaseDecimalCount(1)
                    )
                );
                sut.Value =
                    new DbaseFieldNumberGenerator(random)
                        .GenerateAcceptableValue(sut);

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
                        var result = new DbaseNullableDouble(sut.Field);
                        result.Read(reader);

                        Assert.Equal(sut, result, new DbaseFieldValueEqualityComparer());
                    }
                }
            }
        }

        [Fact]
        public void CanReadWriteWithMaxDecimalCount()
        {
            var length = DbaseNullableDouble.MaximumLength;
            var decimalCount = DbaseNullableDouble.MaximumDecimalCount;
            var sut =
                new DbaseNullableDouble(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        DbaseFieldType.Number,
                        _fixture.Create<ByteOffset>(),
                        length,
                        decimalCount
                    )
                );

            using (var random = new PooledRandom())
            {
                sut.Value =
                    new DbaseFieldNumberGenerator(random)
                        .GenerateAcceptableValue(sut);

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
                        var result = new DbaseNullableDouble(sut.Field);
                        result.Read(reader);

                        Assert.Equal(sut.Field, result.Field);
                        Assert.Equal(sut.Value, result.Value);
                    }
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseNullableDouble>();

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
                    var result = new DbaseNullableDouble(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseNullableDouble>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, sut.Field.Length.ToInt32())).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseNullableDouble(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }

        [Fact]
        public void WritesExcessDecimalsAsZero()
        {
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);
            var sut = new DbaseNullableDouble(
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Number,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount
                ), 0.0);

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                if (decimalCount.ToInt32() == 0)
                {
                    Assert.Equal(
                        "0".PadLeft(length.ToInt32()),
                        Encoding.ASCII.GetString(stream.ToArray()));
                }
                else
                {
                    Assert.Equal(
                        new string(' ', length.ToInt32() - decimalCount.ToInt32() - 2)
                        + "0."
                        + new string('0', decimalCount.ToInt32()),
                        Encoding.ASCII.GetString(stream.ToArray())
                    );
                }
            }
        }
    }
}
