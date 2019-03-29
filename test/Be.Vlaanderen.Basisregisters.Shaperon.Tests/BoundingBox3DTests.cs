namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class BoundingBox3DTests
    {
        private readonly Fixture _fixture;

        public BoundingBox3DTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void EmptyReturnsExpectedResult()
        {
            Assert.Equal(new BoundingBox3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0), BoundingBox3D.Empty);
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
            ).Verify(typeof(BoundingBox3D));
        }

        [Theory]
        [MemberData(nameof(ExpandWithCases))]
        public void ExpandWithReturnsExpectedResult(BoundingBox3D sut, BoundingBox3D other, BoundingBox3D expected)
        {
            var result = sut.ExpandWith(other);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> ExpandWithCases
        {
            get
            {
                var fixture = new Fixture();
                var sut = new BoundingBox3D(
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    fixture.Create<double>()
                );
                var bigger = new BoundingBox3D(
                    sut.XMin - 1,
                    sut.YMin - 1,
                    sut.XMax + 1,
                    sut.YMax + 1,
                    sut.ZMin - 1,
                    sut.ZMax + 1,
                    sut.MMin - 1,
                    sut.MMax + 1
                );
                var smaller = new BoundingBox3D(
                    sut.XMin + 1,
                    sut.YMin + 1,
                    sut.XMax - 1,
                    sut.YMax - 1,
                    sut.ZMin + 1,
                    sut.ZMax - 1,
                    sut.MMin + 1,
                    sut.MMax - 1
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

        [Fact]
        public void FromPointGeometryReturnsExpectedResult()
        {
            var geometry = _fixture.Create<Point>();

            var result = BoundingBox3D.FromGeometry(geometry);

            Assert.Equal(new BoundingBox3D(
                geometry.X,
                geometry.Y,
                geometry.X,
                geometry.Y,
                double.NaN,
                double.NaN,
                double.NaN,
                double.NaN
            ), result);
        }

        [Fact]
        public void FromPolyLineMGeometryReturnsExpectedResult()
        {
            var geometry = _fixture.Create<PolyLineM>();

            var result = BoundingBox3D.FromGeometry(geometry);

            Assert.Equal(new BoundingBox3D(
                geometry.Points.Min(point => point.X),
                geometry.Points.Min(point => point.Y),
                geometry.Points.Max(point => point.X),
                geometry.Points.Max(point => point.Y),
                double.NaN,
                double.NaN,
                geometry.MeasureRange.Min,
                geometry.MeasureRange.Max
            ), result);
        }
    }
}
