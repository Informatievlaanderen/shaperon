namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseRecordCountTests
    {
        private readonly Fixture _fixture;

        public DbaseRecordCountTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseRecordCount();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DbaseRecordCount(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseRecordCountValue();
            var sut = new DbaseRecordCount(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsDbaseRecordCountValue();
        //     var sut = new DbaseRecordCount(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsDbaseRecordCountValue();
            var sut = new DbaseRecordCount(value);

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
            ).Verify(typeof(DbaseRecordCount));
        }

        [Fact]
        public void IsEquatableToDbaseRecordCount()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseRecordCount>>(_fixture.Create<DbaseRecordCount>());
        }
    }
}
