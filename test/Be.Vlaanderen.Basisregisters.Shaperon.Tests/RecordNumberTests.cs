namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class RecordNumberTests
    {
        private readonly Fixture _fixture;

        public RecordNumberTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeRecordNumber();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegativeOrZero(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RecordNumber(value));
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(int.MaxValue - 1, int.MaxValue)]
        public void NextReturnsExpectedValue(int value, int expected)
        {
            var sut = new RecordNumber(value);

            var result = sut.Next();

            Assert.Equal(new RecordNumber(expected), result);
        }

        [Fact]
        public void NextOfIntMaxValueThrows()
        {
            var sut = new RecordNumber(int.MaxValue);

            Assert.Throws<InvalidOperationException>(() => sut.Next());
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsRecordNumberValue();
            var sut = new RecordNumber(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsRecordNumberValue();
        //     var sut = new RecordNumber(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsRecordNumberValue();
            var sut = new RecordNumber(value);

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
            ).Verify(typeof(RecordNumber));
        }

        [Fact]
        public void IsEquatableToRecordNumber()
        {
            Assert.IsAssignableFrom<IEquatable<RecordNumber>>(_fixture.Create<RecordNumber>());
        }
    }
}
