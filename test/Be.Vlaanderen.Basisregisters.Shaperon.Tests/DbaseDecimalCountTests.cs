using System.Linq;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseDecimalCountTests
    {
        private readonly Fixture _fixture;

        public DbaseDecimalCountTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseDecimalCount();
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(0, 1, 1)]
        [InlineData(1, 1, 1)]
        public void MaxReturnsExpectedResult(int left, int right, int expected)
        {
            var result = DbaseDecimalCount.Max(new DbaseDecimalCount(left), new DbaseDecimalCount(right));

            Assert.Equal(new DbaseDecimalCount(expected), result);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(1, 1, 1)]
        public void MinReturnsExpectedResult(int left, int right, int expected)
        {
            var result = DbaseDecimalCount.Min(new DbaseDecimalCount(left), new DbaseDecimalCount(right));

            Assert.Equal(new DbaseDecimalCount(expected), result);
        }

        [Fact]
        public void PlusReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseDecimalCount>();
            var sut = new Generator<DbaseDecimalCount>(_fixture).First(candidate =>
                candidate.ToInt32() <= (254 - value.ToInt32()));

            var result = sut.Plus(value);

            Assert.Equal(new DbaseDecimalCount(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseDecimalCount>();
            var sut = new Generator<DbaseDecimalCount>(_fixture).First(candidate =>
                candidate.ToInt32() <= (254 - value.ToInt32()));

            var result = sut + value;

            Assert.Equal(new DbaseDecimalCount(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void MinusReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseDecimalCount>();
            var sut = new Generator<DbaseDecimalCount>(_fixture).First(candidate => candidate >= value);

            var result = sut.Minus(value);

            Assert.Equal(new DbaseDecimalCount(sut.ToInt32() - value.ToInt32()), result);
        }

        [Fact]
        public void MinusOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseDecimalCount>();
            var sut = new Generator<DbaseDecimalCount>(_fixture).First(candidate => candidate >= value);

            var result = sut - value;

            Assert.Equal(new DbaseDecimalCount(sut.ToInt32() - value.ToInt32()), result);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DbaseDecimalCount(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
            var sut = new DbaseDecimalCount(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
        //     var sut = new DbaseDecimalCount(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToByteReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
            var sut = new DbaseDecimalCount(value);

            var result = sut.ToByte();

            Assert.Equal((byte) value, result);
        }

        // [Fact]
        // public void ImplicitConversionToByteReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
        //     var sut = new DbaseDecimalCount(value);

        //     byte result = sut;

        //     Assert.Equal((byte)value, result);
        // }

        [Fact]
        public void ToLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
            var sut = new DbaseDecimalCount(value);

            var result = sut.ToLength();

            Assert.Equal(new DbaseFieldLength(sut.ToInt32()), result);
        }

        // [Fact]
        // public void ImplicitConversionToDbaseFieldLengthReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
        //     var sut = new DbaseDecimalCount(value);

        //     DbaseFieldLength result = sut;

        //     Assert.Equal(new DbaseFieldLength(sut.ToInt32()), result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseDecimalCountValue();
            var sut = new DbaseDecimalCount(value);

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
            ).Verify(typeof(DbaseDecimalCount));
        }

        [Fact]
        public void IsEquatableToDbaseDecimalCount()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseDecimalCount>>(_fixture.Create<DbaseDecimalCount>());
        }

        [Fact]
        public void IsComparableToDbaseDecimalCount()
        {
            Assert.IsAssignableFrom<IComparable<DbaseDecimalCount>>(_fixture.Create<DbaseDecimalCount>());
        }

        [Theory]
        [InlineData(0, 254, -1)]
        [InlineData(0, 1, -1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(254, 0, 1)]
        public void CompareToDbaseDecimalCountReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void EqualityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut == other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, true)]
        public void InequalityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut != other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(254, 0, true)]
        public void GreaterThanDbaseDecimalCountOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, true)]
        [InlineData(254, 0, true)]
        public void GreaterThanOrEqualToDbaseDecimalCountOperatorReturnsExpectedValue(int left, int right,
            bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(254, 0, false)]
        public void LessThanDbaseDecimalCountOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 254, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(254, 0, false)]
        public void LessThanOrEqualToDbaseDecimalCountOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseDecimalCount(left);
            var other = new DbaseDecimalCount(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }
    }
}
