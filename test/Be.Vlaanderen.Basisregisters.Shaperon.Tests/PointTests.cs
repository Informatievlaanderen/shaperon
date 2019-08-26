namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class PointTests
    {
        private readonly Fixture _fixture;

        public PointTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<double>(customizer => customizer.FromFactory(rnd => rnd.NextDouble()));
        }

        [Fact]
        public void XCanNotBeNaN()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(double.NaN, _fixture.Create<double>()));
        }

        [Fact]
        public void YCanNotBeNaN()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(_fixture.Create<double>(), double.NaN));
        }

        [Fact]
        public void XCanNotBeNegativeInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(double.NegativeInfinity, _fixture.Create<double>()));
        }

        [Fact]
        public void YCanNotBeNegativeInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(_fixture.Create<double>(), double.NegativeInfinity));
        }

        [Fact]
        public void XCanNotBePositiveInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(double.PositiveInfinity, _fixture.Create<double>()));
        }

        [Fact]
        public void YCanNotBePositiveInfinite()
        {
            Assert.Throws<ArgumentException>(
                () => new Point(_fixture.Create<double>(),double.PositiveInfinity));
        }

        [Fact]
        public void XReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new Point(value, _fixture.Create<double>());
            var result = sut.X;
            Assert.Equal(value, result);
        }

        [Fact]
        public void YReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new Point(_fixture.Create<double>(), value);
            var result = sut.Y;
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
            ).Verify(typeof(Point));
        }

        [Fact]
        public void ToStringReturnsExpectedResult()
        {
            var x = _fixture.Create<double>();
            var y = _fixture.Create<double>();
            var sut = new Point(x, y);
            var result = sut.ToString();
            Assert.Equal(
                "{X=" + sut.X.ToString(CultureInfo.InvariantCulture) + ",Y=" +
                sut.Y.ToString(CultureInfo.InvariantCulture) + "}", result);
        }
    }
}
