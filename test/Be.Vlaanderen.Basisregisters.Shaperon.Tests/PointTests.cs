namespace Be.Vlaanderen.Basisregisters.Shaperon
{
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
