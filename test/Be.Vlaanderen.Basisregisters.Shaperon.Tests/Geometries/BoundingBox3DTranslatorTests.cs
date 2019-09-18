namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using Xunit;

    public class BoundingBox3DTranslatorTests
    {
        [Theory]
        [MemberData(nameof(FromGeometryCases))]
        public void FromGeometryReturnsExpectedResult(Geometry geometry, BoundingBox3D expected)
        {
            var result = BoundingBox3DTranslator.FromGeometry(geometry);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> FromGeometryCases
        {
            get
            {
                var fixture = new Fixture();
                fixture.Customize<Coordinate>(customization =>
                    customization.FromFactory(generator =>
                        new Coordinate(
                            fixture.Create<double>(),
                            fixture.Create<double>()
                        )
                    ).OmitAutoProperties()
                );

                fixture.Customize<CoordinateZM>(customization =>
                    customization.FromFactory(generator =>
                        new CoordinateZM(
                            fixture.Create<double>(),
                            fixture.Create<double>(),
                            fixture.Create<double>(),
                            fixture.Create<double>()
                        )
                    ).OmitAutoProperties()
                );

                fixture.Customize<Point>(customization =>
                    customization.FromFactory(generator =>
                        new Point(
                            new CoordinateZM(
                            fixture.Create<double>(),
                            fixture.Create<double>(),
                            fixture.Create<double>(),
                            fixture.Create<double>()
                            )
                        )
                    ).OmitAutoProperties()
                );

                fixture.Customize<LineString>(customization =>
                    customization.FromFactory(generator =>
                        new LineString(
                            new CoordinateArraySequence(fixture.CreateMany<CoordinateZM>().Cast<Coordinate>().ToArray()),
                            GeometryConfiguration.GeometryFactory)
                    ).OmitAutoProperties()
                );
                fixture.Customize<MultiLineString>(customization =>
                    customization.FromFactory(generator =>
                        new MultiLineString(fixture.CreateMany<LineString>(generator.Next(1, 10)).ToArray())
                    ).OmitAutoProperties()
                );
                var point1 = fixture.Create<Point>();

                yield return new object[]
                {
                    point1,
                    new BoundingBox3D(
                        point1.X,
                        point1.Y,
                        point1.X,
                        point1.Y,
                        point1.Z,
                        point1.Z,
                        point1.M,
                        point1.M
                    )
                };

                var point2 = new Point(
                    new CoordinateZM(
                    fixture.Create<double>(),
                    fixture.Create<double>(),
                    double.NaN,
                    double.NaN
                    )
                );

                yield return new object[]
                {
                    point2,
                    new BoundingBox3D(
                        point2.X,
                        point2.Y,
                        point2.X,
                        point2.Y,
                        double.NaN,
                        double.NaN,
                        double.NaN,
                        double.NaN
                    )
                };


                var linestring1 = fixture.Create<MultiLineString>();

                yield return new object[]
                {
                    linestring1,
                    new BoundingBox3D(
                        linestring1.GetOrdinates(Ordinate.X).Min(),
                        linestring1.GetOrdinates(Ordinate.Y).Min(),
                        linestring1.GetOrdinates(Ordinate.X).Max(),
                        linestring1.GetOrdinates(Ordinate.Y).Max(),
                        linestring1.GetOrdinates(Ordinate.Z).DefaultIfEmpty(double.NaN).Min(),
                        linestring1.GetOrdinates(Ordinate.Z).DefaultIfEmpty(double.NaN).Max(),
                        linestring1.GetOrdinates(Ordinate.M).DefaultIfEmpty(double.NaN).Min(),
                        linestring1.GetOrdinates(Ordinate.M).DefaultIfEmpty(double.NaN).Max()
                    )
                };

                fixture.Customize<LineString>(customization =>
                    customization.FromFactory(generator =>
                        new LineString(
                            new CoordinateArraySequence(
                                fixture
                                    .CreateMany<Coordinate>(generator.Next(2, 10))
                                    .ToArray()
                            ),
                            GeometryConfiguration.GeometryFactory
                        )
                    ).OmitAutoProperties()
                );

                var linestring2 = fixture.Create<MultiLineString>();

                yield return new object[]
                {
                    linestring2,
                    new BoundingBox3D(
                        linestring2.GetOrdinates(Ordinate.X).Min(),
                        linestring2.GetOrdinates(Ordinate.Y).Min(),
                        linestring2.GetOrdinates(Ordinate.X).Max(),
                        linestring2.GetOrdinates(Ordinate.Y).Max(),
                        double.NaN,
                        double.NaN,
                        double.NaN,
                        double.NaN
                    )
                };
            }
        }
    }
}
