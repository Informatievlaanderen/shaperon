namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.Linq;
    using AutoFixture;

    public static class GeometryCustomizations
    {
        public static void CustomizePoint(this IFixture fixture)
        {
            fixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        fixture.Create<double>(), fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
        }

        public static void CustomizeGeometryPoint(this IFixture fixture)
        {
            fixture.Customize<NetTopologySuite.Geometries.Point>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.Point(
                        GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(new [] {
                            new GeoAPI.Geometries.Coordinate(fixture.Create<double>(), fixture.Create<double>())
                        }),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties()
            );
        }

        public static void CustomizeGeometryPointM(this IFixture fixture)
        {
            fixture.Customize<PointM>(customization =>
                customization.FromFactory(generator =>
                    new PointM(
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
        }

        public static void CustomizeGeometryPolygon(this IFixture fixture)
        {
            const int polygonExteriorBufferCoordinate = 50;
            var pointFixture = new Fixture();
            pointFixture.Customize<NetTopologySuite.Geometries.Point>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.Point(
                        generator.Next(polygonExteriorBufferCoordinate - 1),
                        generator.Next(polygonExteriorBufferCoordinate - 1)
                    )
                ).OmitAutoProperties()
            );

            var ringFixture = new Fixture();
            ringFixture.Customize<GeoAPI.Geometries.ILinearRing>(customization =>
                customization.FromFactory(generator =>
                {
                    NetTopologySuite.Geometries.LinearRing ring;
                    do
                    {
                        var coordinates = pointFixture.CreateMany<NetTopologySuite.Geometries.Point>(3)
                            .Select(point => new GeoAPI.Geometries.Coordinate(point.X, point.Y))
                            .ToList();

                        var coordinate = coordinates.First();
                        coordinates.Add(
                            new GeoAPI.Geometries.Coordinate(coordinate.X,
                                coordinate.Y)); //first coordinate must be last

                        ring = new NetTopologySuite.Geometries.LinearRing(
                            new NetTopologySuite.Geometries.Implementation.CoordinateArraySequence(
                                coordinates.ToArray()),
                            GeometryConfiguration.GeometryFactory
                        );
                    } while (!ring.IsRing || !ring.IsValid || !ring.IsClosed);

                    return ring;
                }).OmitAutoProperties()
            );

            fixture.Customize<NetTopologySuite.Geometries.Polygon>(customization =>
                customization.FromFactory(generator =>
                {
                    NetTopologySuite.Geometries.LinearRing exteriorRing;
                    do
                    {
                        var exteriorCoordinates = pointFixture.CreateMany<Point>(3)
                            .Select(point => new GeoAPI.Geometries.Coordinate(point.X + polygonExteriorBufferCoordinate,
                                point.Y + polygonExteriorBufferCoordinate))
                            .ToList();

                        var coordinate = exteriorCoordinates.First();
                        exteriorCoordinates.Add(
                            new GeoAPI.Geometries.Coordinate(coordinate.X,
                                coordinate.Y)); //first coordinate must be last

                        exteriorRing = new NetTopologySuite.Geometries.LinearRing(
                            new NetTopologySuite.Geometries.Implementation.CoordinateArraySequence(exteriorCoordinates
                                .ToArray()),
                            GeometryConfiguration.GeometryFactory
                        );
                    } while (!exteriorRing.IsRing || !exteriorRing.IsValid || !exteriorRing.IsClosed);

                    return new NetTopologySuite.Geometries.Polygon(exteriorRing,
                        ringFixture.CreateMany<GeoAPI.Geometries.ILinearRing>(generator.Next(0, 1)).ToArray(),
                        GeometryConfiguration.GeometryFactory);
                }).OmitAutoProperties()
            );
        }

        public static void CustomizePolygon(this IFixture fixture)
        {
            const int polygonExteriorBufferCoordinate = 50;
            var pointFixture = new Fixture();
            pointFixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        generator.Next(polygonExteriorBufferCoordinate - 1),
                        generator.Next(polygonExteriorBufferCoordinate - 1)
                    )
                ).OmitAutoProperties()
            );

            fixture.Customize<Polygon>(customization =>
                customization.FromFactory(generator =>
                {
                    const int pointsPerRing = 4;
                    var ringCount = generator.Next(1, 10);
                    var points = pointFixture.CreateMany<Point>(ringCount * pointsPerRing).ToArray();
                    var parts = new int[ringCount];
                    var offset = 0;
                    for (var ringIndex = 0; ringIndex < ringCount; ringIndex++)
                    {
                        points[pointsPerRing * ringIndex + pointsPerRing - 1] = points[pointsPerRing * ringIndex]; // end = start
                        parts[ringIndex] = offset;
                        offset += pointsPerRing;
                    }

                    //Exterior ring needs to be bigger than the interior rings
                    for (var index = 0; index < pointsPerRing; index++)
                    {
                        points[index] = new Point(
                            points[index].X + polygonExteriorBufferCoordinate,
                            points[index].Y + polygonExteriorBufferCoordinate
                        );
                    }

                    var boundingBox = new BoundingBox2D(
                        points.Min(p => p.X),
                        points.Min(p => p.Y),
                        points.Max(p => p.X),
                        points.Max(p => p.Y)
                    );
                    return new Polygon(boundingBox, parts, points);
                }).OmitAutoProperties()
            );
        }

        public static void CustomizeGeometryMultiLineString(this IFixture fixture)
        {
            fixture.Customize<PointM>(customization =>
                customization.FromFactory(generator =>
                    new PointM(
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            fixture.Customize<GeoAPI.Geometries.ILineString>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.LineString(
                        new PointSequence(fixture.CreateMany<PointM>(generator.Next(2, 10))),
                        GeometryConfiguration.GeometryFactory)
                ).OmitAutoProperties()
            );
            fixture.Customize<NetTopologySuite.Geometries.MultiLineString>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.MultiLineString(fixture
                            .CreateMany<GeoAPI.Geometries.ILineString>(generator.Next(1, 10)).ToArray(),
                        GeometryConfiguration.GeometryFactory)
                ).OmitAutoProperties()
            );
        }

        public static void CustomizePolyLineM(this IFixture fixture)
        {
            var pointFixture = new Fixture();
            pointFixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );

            fixture.Customize<PolyLineM>(customization =>
                customization.FromFactory(generator =>
                {
                    const int pointsPerLine = 3;
                    var lineCount = generator.Next(1, 10);
                    var points = pointFixture.CreateMany<Point>(lineCount * pointsPerLine).ToArray();
                    var parts = new int[lineCount];
                    var offset = 0;
                    for (var lineIndex = 0; lineIndex < lineCount; lineIndex++)
                    {
                        parts[lineIndex] = offset;
                        offset += pointsPerLine;
                    }
                    var measures = fixture.CreateMany<double>(points.Length).ToArray();
                    var boundingBox = new BoundingBox2D(
                        points.Min(p => p.X),
                        points.Min(p => p.Y),
                        points.Max(p => p.X),
                        points.Max(p => p.Y)
                    );
                    return new PolyLineM(boundingBox, parts, points, measures);
                }).OmitAutoProperties()
            );
        }
    }
}
