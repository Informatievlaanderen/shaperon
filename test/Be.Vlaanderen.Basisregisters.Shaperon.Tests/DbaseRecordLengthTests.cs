namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseRecordLengthTests
    {
        private readonly Fixture _fixture;

        public DbaseRecordLengthTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseRecordLength();
            _fixture.CustomizeDbaseFieldLength();
        }

        [Fact]
        public void InitialReturnsExpectedResult()
        {
            var result = DbaseRecordLength.Initial;

            Assert.Equal(new DbaseRecordLength(1), result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DbaseRecordLength(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseRecordLengthValue();
            var sut = new DbaseRecordLength(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        [Fact]
        public void ToInt16ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseRecordLengthValue();
            var sut = new DbaseRecordLength(value);

            var result = sut.ToInt16();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseRecordLengthValue();
        //     var sut = new DbaseRecordLength(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void PlusDbaseFieldLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = _fixture.Create<DbaseRecordLength>();

            var result = sut.Plus(value);

            Assert.Equal(new DbaseRecordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseFieldLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseFieldLength>();
            var sut = _fixture.Create<DbaseRecordLength>();

            var result = sut + value;

            Assert.Equal(new DbaseRecordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseRecordLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseRecordLength>();
            var sut = _fixture.Create<DbaseRecordLength>();

            var result = sut.Plus(value);

            Assert.Equal(new DbaseRecordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusDbaseRecordLengthOperatorReturnsExpectedValue()
        {
            var value = _fixture.Create<DbaseRecordLength>();
            var sut = _fixture.Create<DbaseRecordLength>();

            var result = sut + value;

            Assert.Equal(new DbaseRecordLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseRecordLengthValue();
            var sut = new DbaseRecordLength(value);

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
            ).Verify(typeof(DbaseRecordLength));
        }

        [Fact]
        public void IsEquatableToDbaseRecordLength()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseRecordLength>>(_fixture.Create<DbaseRecordLength>());
        }

        [Fact]
        public void IsComparableToDbaseRecordLength()
        {
            Assert.IsAssignableFrom<IComparable<DbaseRecordLength>>(_fixture.Create<DbaseRecordLength>());
        }

        [Theory]
        [InlineData(0, int.MaxValue, -1)]
        [InlineData(0, 1, -1)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(int.MaxValue, 0, 1)]
        public void CompareToDbaseRecordLengthReturnsExpectedResult(int left, int right, int expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut.CompareTo(other);

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        [InlineData(1, 0, false)]
        public void EqualityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut == other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, true)]
        [InlineData(1, 0, true)]
        public void InequalityOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut != other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanDbaseRecordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut > other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, false)]
        [InlineData(0, 1, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, true)]
        [InlineData(int.MaxValue, 0, true)]
        public void GreaterThanOrEqualToDbaseRecordLengthOperatorReturnsExpectedValue(int left, int right,
            bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut >= other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanDbaseRecordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut < other;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, int.MaxValue, true)]
        [InlineData(0, 1, true)]
        [InlineData(1, 1, true)]
        [InlineData(1, 0, false)]
        [InlineData(int.MaxValue, 0, false)]
        public void LessThanOrEqualToDbaseRecordLengthOperatorReturnsExpectedValue(int left, int right, bool expected)
        {
            var sut = new DbaseRecordLength(left);
            var other = new DbaseRecordLength(right);

            var result = sut <= other;

            Assert.Equal(expected, result);
        }
    }
}
