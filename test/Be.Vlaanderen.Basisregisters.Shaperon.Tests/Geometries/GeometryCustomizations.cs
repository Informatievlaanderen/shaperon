namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.Collections.Generic;
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
                            new NetTopologySuite.Geometries.Coordinate(fixture.Create<double>(), fixture.Create<double>())
                        }),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties()
            );
        }

//        public static void CustomizeGeometryPointM(this IFixture fixture)
//        {
//            fixture.Customize<PointM>(customization =>
//                customization.FromFactory(generator =>
//                    new PointM(
//                        fixture.Create<double>(),
//                        fixture.Create<double>(),
//                        fixture.Create<double>(),
//                        fixture.Create<double>()
//                    )
//                ).OmitAutoProperties()
//            );
//        }

        public static void CustomizeGeometryPolygon(this IFixture fixture)
        {
            const int polygonExteriorBufferCoordinate = 50;
            var coordinateFixture = new Fixture();
            coordinateFixture.Customize<NetTopologySuite.Geometries.Coordinate>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.Coordinate(
                        generator.Next(polygonExteriorBufferCoordinate - 1),
                        generator.Next(polygonExteriorBufferCoordinate - 1)
                    )
                ).OmitAutoProperties()
            );

            var ringFixture = new Fixture();
            ringFixture.Customize<NetTopologySuite.Geometries.LinearRing>(customization =>
                customization.FromFactory(generator =>
                {
                    NetTopologySuite.Geometries.LinearRing ring;
                    do
                    {
                        var coordinates = coordinateFixture
                            .CreateMany<NetTopologySuite.Geometries.Coordinate>(3)
                            .ToList();

                        var coordinate = coordinates[0];
                        coordinates.Add(coordinate.Copy()); //first coordinate must be last

                        ring = new NetTopologySuite.Geometries.LinearRing(
                            GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(coordinates.ToArray()),
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
                        var exteriorCoordinates = coordinateFixture.CreateMany<NetTopologySuite.Geometries.Coordinate>(3)
                            .Select(point => new NetTopologySuite.Geometries.Coordinate(point.X + polygonExteriorBufferCoordinate,
                                point.Y + polygonExteriorBufferCoordinate))
                            .ToList();

                        var coordinate = exteriorCoordinates[0];
                        exteriorCoordinates.Add(coordinate.Copy()); //first coordinate must be last

                        exteriorRing = new NetTopologySuite.Geometries.LinearRing(
                            new NetTopologySuite.Geometries.Implementation.CoordinateArraySequence(exteriorCoordinates
                                .ToArray()),
                            GeometryConfiguration.GeometryFactory
                        );
                    } while (!exteriorRing.IsRing || !exteriorRing.IsValid || !exteriorRing.IsClosed);

                    return new NetTopologySuite.Geometries.Polygon(exteriorRing,
                        ringFixture.CreateMany<NetTopologySuite.Geometries.LinearRing>(generator.Next(0, 1)).ToArray(),
                        GeometryConfiguration.GeometryFactory);
                }).OmitAutoProperties()
            );
        }

        public static void CustomizeGeometryMultiPolygon(this IFixture fixture)
        {
            fixture.Customize<NetTopologySuite.Geometries.MultiPolygon>(customization =>
                customization.FromFactory(generator =>
                {
                    var polygonCount = generator.Next(1, 4);
                    var polygons = new NetTopologySuite.Geometries.Polygon[polygonCount];
                    for (var polygonIndex = 0; polygonIndex < polygonCount; polygonIndex++)
                    {
                        var offsetX = 10.0 * polygonIndex;
                        var offsetY = 10.0 * polygonIndex;

                        var shell = new NetTopologySuite.Geometries.LinearRing(
                            new []
                            {
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY + 5.0).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX + 5.0, offsetY + 5.0).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX + 5.0, offsetY).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY).Coordinate
                            });

                        var holes = new[] // points are enumerated counter clock wise
                        {
                            new NetTopologySuite.Geometries.LinearRing(
                                new[]
                                {
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 2.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 2.0, offsetY + 3.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 3.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 2.0).Coordinate
                                }),
                            new NetTopologySuite.Geometries.LinearRing(
                                new[]
                                {
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 1.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 4.0, offsetY + 1.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 4.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 1.0).Coordinate
                                })
                        };
                        polygons[polygonIndex] = new NetTopologySuite.Geometries.Polygon(shell, holes, GeometryConfiguration.GeometryFactory);
                    }
                    return new NetTopologySuite.Geometries.MultiPolygon(polygons);
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

        public static void CustomizeMultiPolygon(this IFixture fixture)
        {
            fixture.Customize<Polygon>(customization =>
                customization.FromFactory(generator =>
                {
                    var polygonCount = generator.Next(1, 4);
                    var polygons = new NetTopologySuite.Geometries.Polygon[polygonCount];
                    for (var polygonIndex = 0; polygonIndex < polygonCount; polygonIndex++)
                    {
                        var offsetX = 10.0 * polygonIndex;
                        var offsetY = 10.0 * polygonIndex;

                        var shell = new NetTopologySuite.Geometries.LinearRing(
                            new []
                            {
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY + 5.0).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX + 5.0, offsetY + 5.0).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX + 5.0, offsetY).Coordinate,
                                new NetTopologySuite.Geometries.Point(offsetX, offsetY).Coordinate
                            });

                        var holes = new[] // points are enumerated counter clock wise
                        {
                            new NetTopologySuite.Geometries.LinearRing(
                                new[]
                                {
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 2.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 2.0, offsetY + 3.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 3.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 1.0, offsetY + 2.0).Coordinate
                                }),
                            new NetTopologySuite.Geometries.LinearRing(
                                new[]
                                {
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 1.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 4.0, offsetY + 1.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 4.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 2.0).Coordinate,
                                    new NetTopologySuite.Geometries.Point(offsetX + 3.0, offsetY + 1.0).Coordinate
                                })
                        };
                        polygons[polygonIndex] = new NetTopologySuite.Geometries.Polygon(shell, holes);
                    }

                    var linearRings = polygons
                        .SelectMany(polygon => new[] {polygon.Shell}.Concat(polygon.Holes))
                        .ToArray();
                    var parts = new int[linearRings.Length];
                    var points = new Point[polygons.Sum(polygon => polygon.Shell.NumPoints + polygon.Holes.Sum(hole => hole.NumPoints))];
                    var offset = 0;
                    for (var ringIndex = 0; ringIndex < linearRings.Length; ringIndex++)
                    {
                        var linearRing = linearRings[ringIndex];
                        parts[ringIndex] = offset;
                        for (var pointIndex = 0; pointIndex < linearRing.NumPoints; pointIndex++)
                        {
                            var point = linearRing.GetPointN(pointIndex);
                            points[offset + pointIndex] = new Point(point.X, point.Y);
                        }
                        offset += linearRing.NumPoints;
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
            fixture.Customize<NetTopologySuite.Geometries.CoordinateZM>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.CoordinateZM(
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            fixture.Customize<NetTopologySuite.Geometries.LineString>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.LineString(
                        GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(
                            fixture.CreateMany<NetTopologySuite.Geometries.CoordinateZM>(generator.Next(2, 10))
                                .Cast<NetTopologySuite.Geometries.Coordinate>()
                                .ToArray()
                        ),
                        GeometryConfiguration.GeometryFactory)
                ).OmitAutoProperties()
            );
            fixture.Customize<NetTopologySuite.Geometries.MultiLineString>(customization =>
                customization.FromFactory(generator =>
                    new NetTopologySuite.Geometries.MultiLineString(fixture
                            .CreateMany<NetTopologySuite.Geometries.LineString>(generator.Next(1, 10)).ToArray(),
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
