namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class SpatialReferenceSystemIdentifierTests
    {
        private readonly Fixture _fixture;

        public SpatialReferenceSystemIdentifierTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void BelgeLambert1972ReturnsExpectedValue()
        {
            Assert.Equal(new SpatialReferenceSystemIdentifier(31370),
                SpatialReferenceSystemIdentifier.BelgeLambert1972);
        }

        [Fact]
        public void EsriBelgeLambert1972ReturnsExpectedValue()
        {
            Assert.Equal(new SpatialReferenceSystemIdentifier(103300),
                SpatialReferenceSystemIdentifier.EsriBelgeLambert1972);
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>();
            var sut = new SpatialReferenceSystemIdentifier(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>();
        //     var sut = new SpatialReferenceSystemIdentifier(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>();
            var sut = new SpatialReferenceSystemIdentifier(value);

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
            ).Verify(typeof(SpatialReferenceSystemIdentifier));
        }

        [Fact]
        public void IsEquatableToSpatialReferenceSystemIdentifier()
        {
            Assert.IsAssignableFrom<IEquatable<SpatialReferenceSystemIdentifier>>(
                _fixture.Create<SpatialReferenceSystemIdentifier>());
        }
    }
}
