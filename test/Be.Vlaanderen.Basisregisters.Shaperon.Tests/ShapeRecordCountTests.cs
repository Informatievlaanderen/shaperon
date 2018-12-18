namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class ShapeRecordCountTests
    {
        private readonly Fixture _fixture;

        public ShapeRecordCountTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeShapeRecordCount();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShapeRecordCount(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsShapeRecordCountValue();
            var sut = new ShapeRecordCount(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsShapeRecordCountValue();
        //     var sut = new ShapeRecordCount(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsShapeRecordCountValue();
            var sut = new ShapeRecordCount(value);

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
            ).Verify(typeof(ShapeRecordCount));
        }

        [Fact]
        public void IsEquatableToShapeRecordCount()
        {
            Assert.IsAssignableFrom<IEquatable<ShapeRecordCount>>(_fixture.Create<ShapeRecordCount>());
        }
    }
}
