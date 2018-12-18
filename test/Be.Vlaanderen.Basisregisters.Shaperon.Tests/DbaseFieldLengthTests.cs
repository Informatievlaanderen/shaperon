namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseFieldLengthTests
    {
        private readonly Fixture _fixture;

        public DbaseFieldLengthTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldLength();
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(0, 1, 1)]
        [InlineData(1, 1, 1)]
        public void MaxReturnsExpectedResult(int left, int right, int expected)
        {
            var result = DbaseFieldLength.Max(new DbaseFieldLength(left), new DbaseFieldLength(right));

            Assert.Equal(new DbaseFieldLength(expected), result);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1, 1, 1)]
        public void MinReturnsExpectedResult(int left, int right, int expected)
        {
            var result = DbaseFieldLength.Min(new DbaseFieldLength(left), new DbaseFieldLength(right));

            Assert.Equal(new DbaseFieldLength(expected), result);
        }

        [Fact]
        public void MinLengthReturnsExpectedResult()
        {
            var result = DbaseFieldLength.MinLength;

            Assert.Equal(new DbaseFieldLength(0), result);
        }

        [Fact]
        public void MaxLengthReturnsExpectedResult()
        {
            var result = DbaseFieldLength.MaxLength;

            Assert.Equal(new DbaseFieldLength(254), result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DbaseFieldLength(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseFieldLengthValue();
            var sut = new DbaseFieldLength(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseFieldLengthValue();
        //     var sut = new DbaseFieldLength(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToByteReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseFieldLengthValue();
            var sut = new DbaseFieldLength(value);

            var result = sut.ToByte();

            Assert.Equal((byte) value, result);
        }

        // [Fact]
        // public void ImplicitConversionToByteReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseFieldLengthValue();
        //     var sut = new DbaseFieldLength(value);

        //     byte result = sut;

        //     Assert.Equal((byte)value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseFieldLengthValue();
            var sut = new DbaseFieldLength(value);

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
            ).Verify(typeof(DbaseFieldLength));
        }

        [Fact]
        public void IsEquatableToDbaseFieldLength()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseFieldLength>>(_fixture.Create<DbaseFieldLength>());
        }

        [Fact]
        public void PlusDbaseFieldLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = new Generator<DbaseFieldLength>(_fixture).First(candidate =>
                candidate.ToInt32() <= (254 - value.ToInt32()));

            var result = sut.Plus(value);

            Assert.Equal(new DbaseFieldLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseFieldLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = new Generator<DbaseFieldLength>(_fixture).First(candidate =>
                candidate.ToInt32() <= (254 - value.ToInt32()));

            var result = sut + value;

            Assert.Equal(new DbaseFieldLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void MinusDbaseFieldLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = new Generator<DbaseFieldLength>(_fixture).First(candidate => candidate >= value);

            var result = sut.Minus(value);

            Assert.Equal(new DbaseFieldLength(sut.ToInt32() - value.ToInt32()), result);
        }

        [Fact]
        public void MinusDbaseFieldLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = new Generator<DbaseFieldLength>(_fixture).First(candidate => candidate >= value);

            var result = sut - value;

            Assert.Equal(new DbaseFieldLength(sut.ToInt32() - value.ToInt32()), result);
        }

        [Fact]
        public void IsComparableToDbaseFieldLength()
        {
            Assert.IsAssignableFrom<IComparable<DbaseFieldLength>>(_fixture.Create<DbaseFieldLength>());
        }

        [Theory]
        [InlineData(0, 254, -1)]
        [InlineData(0, 1, -1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(254, 0, 1)]
        public void CompareToDbaseFieldLengthReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void EqualityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut == other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, true)]
        public void InequalityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut != other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(254, 0, true)]
        public void GreaterThanDbaseFieldLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, true)]
        [InlineData(254, 0, true)]
        public void GreaterThanOrEqualToDbaseFieldLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(254, 0, false)]
        public void LessThanDbaseFieldLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(254, 0, false)]
        public void LessThanOrEqualToDbaseFieldLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseFieldLength(left);
            var other = new DbaseFieldLength(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }
    }
}
