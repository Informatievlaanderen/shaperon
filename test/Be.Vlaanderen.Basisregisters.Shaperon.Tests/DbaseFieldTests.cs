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

    public class DbaseFieldTests
    {
        private readonly Fixture _fixture;

        public DbaseFieldTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeByteOffset();
            _fixture.CustomizeDbaseFieldLength(DbaseNumber.MaximumLength);
            _fixture.CustomizeDbaseDecimalCount(DbaseNumber.MaximumDecimalCount);
            _fixture.CustomizeDbaseField();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void VerifyEquality()
        {
            new CompositeIdiomaticAssertion(
                new EqualsNewObjectAssertion(_fixture),
                new EqualsNullAssertion(_fixture),
                new EqualsSelfAssertion(_fixture),
                new EqualsSuccessiveAssertion(_fixture),
                new GetHashCodeSuccessiveAssertion(_fixture)
            ).Verify(typeof(DbaseField));
        }

        // Field type, length, decimal count validation related tests

        [Fact]
        public void CreateCharacterFieldFailsWhenDecimalCountIsNot0()
        {
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0);

            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Character,
                    _fixture.Create<ByteOffset>(),
                    _fixture.Create<DbaseFieldLength>(),
                    decimalCount));
        }

        [Fact]
        public void CreateCharacterFieldReturnsExpectedWhenDecimalCountIs0()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = _fixture.Create<DbaseFieldLength>();
            var decimalCount = new DbaseDecimalCount(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Character,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Character, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(0), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CreateNumberFieldFailsWhenDecimalCountIsNotZeroAndNotLessThanLengthMinus2(int minus)
        {
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = new DbaseDecimalCount(length.ToInt32()).Minus(new DbaseDecimalCount(minus));
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Number,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Fact]
        public void CreateNumberFieldReturnsExpectedResultWhenDecimalCountIsZeroAndNotLessThanLengthMinus2()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = new DbaseFieldLength(1);
            var decimalCount = new DbaseDecimalCount(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Number,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Number, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(1), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(16)]
        [InlineData(254)]
        public void CreateNumberFieldFailsWhenDecimalCountIsNotZeroAndNotLessThanOrEqualTo15(int decimals)
        {
            var length = _fixture.Create<DbaseFieldLength>();
            var decimalCount = new DbaseDecimalCount(decimals);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Number,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Theory]
        [InlineData(19)]
        [InlineData(254)]
        public void CreateNumberFieldFailsWhenLengthIsNotLessThanOrEqualTo18(int digits)
        {
            var length = new DbaseFieldLength(digits);
            var decimalCount = _fixture.Create<DbaseDecimalCount>();
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Number,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Fact]
        public void CreateNumberFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);
            var positiveIntegerDigits = decimalCount != new DbaseDecimalCount(0)
                ? new DbaseIntegerDigits(length.ToInt32() - 1 - decimalCount.ToInt32())
                : new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Number,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Number, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CreateFloatFieldFailsWhenDecimalCountIsNotZeroAndNotLessThanLengthMinus2(int minus)
        {
            var length = _fixture.GenerateDbaseSingleLength();
            var decimalCount = new DbaseDecimalCount(length.ToInt32()).Minus(new DbaseDecimalCount(minus));
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Float,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Fact]
        public void CreateFloatFieldReturnsExpectedResultWhenDecimalCountIsZeroAndNotLessThanLengthMinus2()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = new DbaseFieldLength(1);
            var decimalCount = new DbaseDecimalCount(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Float,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Float, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(1), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(254)]
        public void CreateFloatFieldFailsWhenDecimalCountIsNotZeroAndNotLessThanOrEqualTo7(int decimals)
        {
            var length = _fixture.Create<DbaseFieldLength>();
            var decimalCount = new DbaseDecimalCount(decimals);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Float,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Theory]
        [InlineData(21)]
        [InlineData(254)]
        public void CreateFloatFieldFailsWhenLengthIsNotLessThanOrEqualTo20(int digits)
        {
            var length = new DbaseFieldLength(digits);
            var decimalCount = _fixture.Create<DbaseDecimalCount>();
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Float,
                    _fixture.Create<ByteOffset>(),
                    length,
                    decimalCount));
        }

        [Fact]
        public void CreateFloatFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = _fixture.GenerateDbaseSingleLengthLessThan(DbaseFloat.MaximumLength);
            var decimalCount = _fixture.GenerateDbaseSingleDecimalCount(length);
            var positiveIntegerDigits = decimalCount != new DbaseDecimalCount(0)
                ? new DbaseIntegerDigits(length.ToInt32() - 1 - decimalCount.ToInt32())
                : new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Float,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Float, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }


        [Fact]
        public void CreateDateFieldFailsWhenLengthIsNot8()
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() != 8);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Date,
                    _fixture.Create<ByteOffset>(),
                    length,
                    new DbaseDecimalCount(0)));
        }

        [Fact]
        public void CreateDateFieldFailsWhenDecimalCountIsNot0()
        {
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Date,
                    _fixture.Create<ByteOffset>(),
                    new DbaseFieldLength(8),
                    decimalCount));
        }

        [Fact]
        public void CreateDateFieldReturnsExpectedResultWhenLengthIs8AndDecimalCountIs0()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = new DbaseFieldLength(8);
            var decimalCount = new DbaseDecimalCount(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Date,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Date, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(0), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        [Fact]
        public void CreateLogicalFieldFailsWhenLengthIsNot1()
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() != 1);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Logical,
                    _fixture.Create<ByteOffset>(),
                    length,
                    new DbaseDecimalCount(0)));
        }

        [Fact]
        public void CreateLogicalFieldFailsWhenDecimalCountIsNot0()
        {
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0);
            Assert.Throws<ArgumentException>(() =>
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Logical,
                    _fixture.Create<ByteOffset>(),
                    new DbaseFieldLength(1),
                    decimalCount));
        }

        [Fact]
        public void CreateLogicalFieldReturnsExpectedResultWhenLengthIs1AndDecimalCountIs0()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var offset = _fixture.Create<ByteOffset>();
            var length = new DbaseFieldLength(1);
            var decimalCount = new DbaseDecimalCount(0);
            var result =
                new DbaseField(
                    name,
                    DbaseFieldType.Logical,
                    offset,
                    length,
                    decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Logical, result.FieldType);
            Assert.Equal(offset, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(0), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        // Factory method tests

        [Fact]
        public void CreateStringFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.Create<DbaseFieldLength>();

            var result = DbaseField.CreateCharacterField(
                name,
                length);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Character, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(new DbaseDecimalCount(0), result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(0), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }


        [Fact]
        public void CreateInt32FieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.Create<DbaseFieldLength>();
            var positiveIntegerDigits = new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);
            var result = DbaseField.CreateInt32Field(
                name,
                length);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Number, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(new DbaseDecimalCount(0), result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }

        [Fact]
        public void CreateInt16FieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.Create<DbaseFieldLength>();
            var positiveIntegerDigits = new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);
            var result = DbaseField.CreateInt16Field(
                name,
                length);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Number, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(new DbaseDecimalCount(0), result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }

        [Fact]
        public void CreateDateFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();

            var result = DbaseField.CreateDateField(name);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Date, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(new DbaseFieldLength(8), result.Length);
            Assert.Equal(new DbaseDecimalCount(0), result.DecimalCount);
            Assert.Equal(new DbaseIntegerDigits(0), result.PositiveIntegerDigits);
            Assert.Equal(new DbaseIntegerDigits(0), result.NegativeIntegerDigits);
        }

        [Fact]
        public void CreateDoubleFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);
            var positiveIntegerDigits = decimalCount != new DbaseDecimalCount(0)
                ? new DbaseIntegerDigits(length.ToInt32() - 1 - decimalCount.ToInt32())
                : new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);

            var result = DbaseField.CreateNumberField(
                name,
                length,
                decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Number, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CreateDoubleFieldThrowsWhenDecimalCountGreaterThanOrEqualToLength(int plus)
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = new DbaseDecimalCount(length.ToInt32()).Plus(new DbaseDecimalCount(plus));

            Assert.Throws<ArgumentException>(() => DbaseField.CreateNumberField(
                name,
                length,
                decimalCount));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateDoubleFieldThrowsWhenDecimalCountGreaterThanZeroAndLengthSmallerThan3(int smallerThan3)
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = new DbaseFieldLength(smallerThan3);
            var decimalCount = new DbaseDecimalCount(1);

            Assert.Throws<ArgumentException>(() => DbaseField.CreateNumberField(
                name,
                length,
                decimalCount));
        }

        [Fact]
        public void CreateSingleFieldReturnsExpectedResult()
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.GenerateDbaseSingleLength();
            var decimalCount = _fixture.GenerateDbaseSingleDecimalCount(length);
            var positiveIntegerDigits = decimalCount != new DbaseDecimalCount(0)
                ? new DbaseIntegerDigits(length.ToInt32() - 1 - decimalCount.ToInt32())
                : new DbaseIntegerDigits(length.ToInt32());
            var negativeIntegerDigits = positiveIntegerDigits != new DbaseIntegerDigits(0)
                ? positiveIntegerDigits.Minus(new DbaseIntegerDigits(1))
                : new DbaseIntegerDigits(0);

            var result = DbaseField.CreateFloatField(
                name,
                length,
                decimalCount);

            Assert.Equal(name, result.Name);
            Assert.Equal(DbaseFieldType.Float, result.FieldType);
            Assert.Equal(ByteOffset.Initial, result.Offset);
            Assert.Equal(length, result.Length);
            Assert.Equal(decimalCount, result.DecimalCount);
            Assert.Equal(positiveIntegerDigits, result.PositiveIntegerDigits);
            Assert.Equal(negativeIntegerDigits, result.NegativeIntegerDigits);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CreateSingleFieldThrowsWhenDecimalCountGreaterThanOrEqualToLength(int plus)
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = _fixture.GenerateDbaseSingleLength();
            var decimalCount = new DbaseDecimalCount(length.ToInt32()).Plus(new DbaseDecimalCount(plus));

            Assert.Throws<ArgumentException>(() => DbaseField.CreateFloatField(
                name,
                length,
                decimalCount));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CreateSingleFieldThrowsWhenDecimalCountGreaterThanZeroAndLengthSmallerThan3(int smallerThan3)
        {
            var name = _fixture.Create<DbaseFieldName>();
            var length = new DbaseFieldLength(smallerThan3);
            var decimalCount = new DbaseDecimalCount(1);

            Assert.Throws<ArgumentException>(() => DbaseField.CreateFloatField(
                name,
                length,
                decimalCount));
        }

        // Field value factory related tests

        [Fact]
        public void CreateCharacterFieldValueReturnsExpectedResult()
        {
            var sut = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Character,
                _fixture.Create<ByteOffset>(),
                _fixture.Create<DbaseFieldLength>(),
                new DbaseDecimalCount(0));

            var result = sut.CreateFieldValue();

            Assert.Equal(sut, result.Field);
            Assert.IsType<DbaseCharacter>(result);
        }

        [Fact]
        public void CreateNumberFieldValueReturnsExpectedResult()
        {
            var length = _fixture.GenerateDbaseDoubleLength();
            var decimalCount = _fixture.GenerateDbaseDoubleDecimalCount(length);
            var sut = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Number,
                _fixture.Create<ByteOffset>(),
                length,
                decimalCount);

            var result = sut.CreateFieldValue();

            Assert.Equal(sut, result.Field);
            Assert.IsType<DbaseNumber>(result);
        }

        [Fact]
        public void CreateFloatFieldValueReturnsExpectedResult()
        {
            var length = _fixture.GenerateDbaseSingleLength();
            var decimalCount = _fixture.GenerateDbaseSingleDecimalCount(length);
            var sut = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Float,
                _fixture.Create<ByteOffset>(),
                length,
                decimalCount);

            var result = sut.CreateFieldValue();

            Assert.Equal(sut, result.Field);
            Assert.IsType<DbaseFloat>(result);
        }

        [Fact]
        public void CreateDateFieldValueReturnsExpectedResult()
        {
            var sut = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.Date,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(8),
                new DbaseDecimalCount(0));

            var result = sut.CreateFieldValue();

            Assert.Equal(sut, result.Field);
            Assert.IsType<DbaseDate>(result);
        }

        [Fact]
        public void CreateLogicalFieldValueReturnsExpectedResult()
        {
            var sut = DbaseField.CreateLogicalField(_fixture.Create<DbaseFieldName>());

            var result = sut.CreateFieldValue();

            Assert.Equal(sut, result.Field);
            Assert.IsType<DbaseLogical>(result);
        }

        [Fact]
        public void AfterReturnsExpectedResult()
        {
            var sut = _fixture.Create<DbaseField>();
            var field = _fixture.Create<DbaseField>();

            var result = sut.After(field);

            Assert.Equal(
                new DbaseField(
                    sut.Name,
                    sut.FieldType,
                    field.Offset.Plus(field.Length),
                    sut.Length,
                    sut.DecimalCount
                ), result);
        }

        [Fact]
        public void AtReturnsExpectedResult()
        {
            var sut = _fixture.Create<DbaseField>();
            var offset = _fixture.Create<ByteOffset>();

            var result = sut.At(offset);

            Assert.Equal(
                new DbaseField(
                    sut.Name,
                    sut.FieldType,
                    offset,
                    sut.Length,
                    sut.DecimalCount
                ), result);
        }

        [Fact]
        public void AfterDoesNotAcceptNull()
        {
            var sut = _fixture.Create<DbaseField>();

            Assert.Throws<ArgumentNullException>(() => sut.After(null));
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => DbaseField.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseField>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseField>();

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
                    var result = DbaseField.Read(reader);

                    Assert.Equal(sut, result);
                }
            }
        }
    }
}
