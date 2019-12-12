namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class BoundingBox2DTests
    {
        private readonly Fixture _fixture;

        public BoundingBox2DTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void EmptyReturnsExpectedResult()
        {
            Assert.Equal(new BoundingBox2D(0.0, 0.0, 0.0, 0.0), BoundingBox2D.Empty);
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
            ).Verify(typeof(BoundingBox2D));
        }

        [Theory]
        [MemberData(nameof(ExpandWithCases))]
        public void ExpandWithReturnsExpectedResult(BoundingBox2D sut, BoundingBox2D other, BoundingBox2D expected)
        {
            var result = sut.ExpandWith(other);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ExpandWithCases
        {
            get
            {
                var fixture = new Fixture();
                var sut = new BoundingBox2D(
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>()
                );
                var bigger = new BoundingBox2D(
                    sut.XMin - 1,
                    sut.YMin - 1,
                    sut.XMax + 1,
                    sut.YMax + 1
                );
                var smaller = new BoundingBox2D(
                    sut.XMin + 1,
                    sut.YMin + 1,
                    sut.XMax - 1,
                    sut.YMax - 1
                );

                yield return new object[]
                {
                    sut,
                    bigger,
                    bigger
                };

                yield return new object[]
                {
                    sut,
                    smaller,
                    sut
                };

                // more cases we can think of but this is a good start
            }
        }
    }
}
