namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class WordLengthTests
    {
        private readonly Fixture _fixture;

        public WordLengthTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeWordLength();
            _fixture.CustomizeByteLength();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WordLength(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = Math.Abs(_fixture.Create<int>());
            var sut = new WordLength(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = Math.Abs(_fixture.Create<int>());
        //     var sut = new WordLength(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void PlusByteLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = _fixture.Create<WordLength>();

            var result = sut.Plus(value);

            Assert.Equal(new WordLength(value.ToInt32() / 2 + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusByteLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = _fixture.Create<WordLength>();

            var result = sut + value;

            Assert.Equal(new WordLength(value.ToInt32() / 2 + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusWordLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<WordLength>();
            var sut = _fixture.Create<WordLength>();

            var result = sut.Plus(value);

            Assert.Equal(new WordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusWordLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<WordLength>();
            var sut = _fixture.Create<WordLength>();

            var result = sut + value;

            Assert.Equal(new WordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void MinusByteLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = new Generator<WordLength>(_fixture)
                .First(candidate => candidate.ToInt32() >= (value.ToInt32() / 2));

            var result = sut.Minus(value);

            Assert.Equal(new WordLength(sut.ToInt32() - (value.ToInt32() / 2)), result);
        }

        [Fact]
        public void MinusByteLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = new Generator<WordLength>(_fixture)
                .First(candidate => candidate.ToInt32() >= (value.ToInt32() / 2));

            var result = sut - value;

            Assert.Equal(new WordLength(sut.ToInt32() - (value.ToInt32() / 2)), result);
        }

        [Fact]
        public void MinusWordLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<WordLength>();
            var sut = new Generator<WordLength>(_fixture)
                .First(candidate => candidate.ToInt32() >= value.ToInt32());

            var result = sut.Minus(value);

            Assert.Equal(new WordLength(sut.ToInt32() - value.ToInt32()), result);
        }

        [Fact]
        public void MinusWordLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<WordLength>();
            var sut = new Generator<WordLength>(_fixture)
                .First(candidate => candidate.ToInt32() >= value.ToInt32());

            var result = sut - value;

            Assert.Equal(new WordLength(sut.ToInt32() - value.ToInt32()), result);
        }

        [Fact]
        public void ToByteLengthReturnsExpectedValue()
        {
            var sut = _fixture.Create<WordLength>();

            var result = sut.ToByteLength();

            Assert.Equal(new ByteLength(sut.ToInt32() * 2), result);
        }

        [Fact]
        public void TimesReturnsExpectedResult()
        {
            var times = _fixture.Create<int>();
            var sut = _fixture.Create<WordLength>();

            var result = sut.Times(times);

            Assert.Equal(new WordLength(sut.ToInt32() * times), result);
        }


        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = Math.Abs(_fixture.Create<int>());
            var sut = new WordLength(value);

            var result = sut.ToString();

            Assert.Equal(value.ToString(), result);
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
            ).Verify(typeof(WordLength));
        }

        [Fact]
        public void IsEquatableToWordLength()
        {
            Assert.IsAssignableFrom<IEquatable<WordLength>>(_fixture.Create<WordLength>());
        }

        [Fact]
        public void IsComparableToWordLength()
        {
            Assert.IsAssignableFrom<IComparable<WordLength>>(_fixture.Create<WordLength>());
        }

        [Theory]
        [InlineData(0, int.MaxValue, -1)]
        [InlineData(0, 1, -1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(int.MaxValue, 0, 1)]
        public void CompareToWordLengthReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void EqualityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut == other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, true)]
        public void InequalityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut != other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanWordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanOrEqualToWordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanWordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanOrEqualToWordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new WordLength(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsComparableToByteLength()
        {
            Assert.IsAssignableFrom<IComparable<ByteLength>>(_fixture.Create<WordLength>());
        }

        [Theory]
        [InlineData(0, int.MaxValue - 1, -1)]
        [InlineData(0, 2, -1)]
        [InlineData(1, 2, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(int.MaxValue, 0, 1)]
        public void CompareToByteLengthReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new WordLength(left);
            var other = new ByteLength(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue - 1, false)]
        [InlineData(0, 2, false)]
        [InlineData(1, 2, false)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanByteLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new ByteLength(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue - 1, false)]
        [InlineData(0, 2, false)]
        [InlineData(1, 2, true)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanOrEqualToByteLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new ByteLength(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue - 1, true)]
        [InlineData(0, 2, true)]
        [InlineData(1, 2, false)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanBytedLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new ByteLength(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue - 1, true)]
        [InlineData(0, 2, true)]
        [InlineData(1, 2, true)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanOrEqualToByteLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new WordLength(left);
            var other = new ByteLength(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }
    }
}
