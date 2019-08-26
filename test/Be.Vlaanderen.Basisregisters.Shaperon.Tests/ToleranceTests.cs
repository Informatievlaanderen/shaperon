namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class ToleranceTests
    {
        private readonly Fixture _fixture;

        public ToleranceTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<double>(customizer => customizer.FromFactory(rnd => rnd.NextDouble()));
        }

        [Fact]
        public void ValueCanNotBeNaN()
        {
            Assert.Throws<ArgumentException>(
                () => new Tolerance(double.NaN));
        }

        [Fact]
        public void ValueCanNotBeNegativeInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Tolerance(double.NegativeInfinity));
        }

        [Fact]
        public void ValueCanNotBePositiveInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Tolerance(double.PositiveInfinity));
        }

        [Fact]
        public void ToDoubleReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new Tolerance(value);
            var result = sut.ToDouble();
            Assert.Equal(value, result);
        }

        [Fact]
        public void ImplicitOperatorToDoubleReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new Tolerance(value);
            double result = sut;
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
            ).Verify(typeof(Tolerance));
        }

        [Fact]
        public void ToStringReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new Tolerance(value);
            var result = sut.ToString();
            Assert.Equal(value.ToString(CultureInfo.InvariantCulture), result);
        }
    }
}
