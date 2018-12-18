namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseFieldNameTests
    {
        private readonly Fixture _fixture;

        public DbaseFieldNameTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ValueCanNotBeNullOrEmpty(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new DbaseFieldName(value));
        }

        [Theory]
        [InlineData(12)]
        [InlineData(256)]
        public void ValueCanNotExceed11Chars(int length)
        {
            var value = new string('a', length);
            Assert.Throws<ArgumentException>(() => new DbaseFieldName(value));
        }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = new string('a', _fixture.Create<int>().AsDbaseFieldNameLength());
            var sut = new DbaseFieldName(value);

            var result = sut.ToString();

            Assert.Equal(value, result);
        }

        [Fact]
        public void ImplicitConversionToStringReturnsExpectedValue()
        {
            var value = new string('a', _fixture.Create<int>().AsDbaseFieldNameLength());
            var sut = new DbaseFieldName(value);

            string result = sut;

            Assert.Equal(value, result);
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
            ).Verify(typeof(DbaseFieldName));
        }

        [Fact]
        public void IsEquatableToDbaseFieldName()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseFieldName>>(_fixture.Create<DbaseFieldName>());
        }
    }
}
