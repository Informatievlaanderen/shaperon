namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class ByteOffsetTests
    {
        private readonly Fixture _fixture;

        public ByteOffsetTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeByteOffset();
            _fixture.CustomizeByteLength();
            _fixture.CustomizeDbaseFieldLength();
        }

        [Fact]
        public void InitialReturnsExpectedResult()
        {
            Assert.Equal(ByteOffset.Initial, new ByteOffset(0));
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteOffset(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = Math.Abs(_fixture.Create<int>());
            var sut = new ByteOffset(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = Math.Abs(_fixture.Create<int>());
        //     var sut = new ByteOffset(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void PlusByteLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = _fixture.Create<ByteOffset>();

            var result = sut.Plus(value);

            Assert.Equal(new ByteOffset(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseFieldLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = _fixture.Create<ByteOffset>();

            var result = sut.Plus(value);

            Assert.Equal(new ByteOffset(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusByteOffsetOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteOffset>();
            var sut = _fixture.Create<ByteOffset>();

            var result = sut + value;

            Assert.Equal(new ByteOffset(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusByteLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = _fixture.Create<ByteOffset>();

            var result = sut + value;

            Assert.Equal(new ByteOffset(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseFieldLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = _fixture.Create<ByteOffset>();

            var result = sut + value;

            Assert.Equal(new ByteOffset(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = Math.Abs(_fixture.Create<int>());
            var sut = new ByteOffset(value);

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
            ).Verify(typeof(ByteOffset));
        }

        [Fact]
        public void IsEquatableToByteOffset()
        {
            Assert.IsAssignableFrom<IEquatable<ByteOffset>>(_fixture.Create<ByteOffset>());
        }

        [Fact]
        public void IsComparableToByteOffset()
        {
            Assert.IsAssignableFrom<IComparable<ByteOffset>>(_fixture.Create<ByteOffset>());
        }

        [Theory]
        [InlineData(0, int.MaxValue, -1)]
        [InlineData(0, 1, -1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(int.MaxValue, 0, 1)]
        public void CompareToReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void EqualityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut == other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, true)]
        public void InequalityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut != other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanOrEqualOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanOrEqualOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new ByteOffset(left);
            var other = new ByteOffset(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }
    }
}
