namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GeometryTranslator
    {
        public static Point FromGeometryPoint(NetTopologySuite.Geometries.Point point)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            return new Point(point.X, point.Y);
        }

        public static NetTopologySuite.Geometries.Point ToGeometryPoint(Point point)
        {
            return new NetTopologySuite.Geometries.Point(point.X, point.Y);
        }

        public static Point FromGeometryPointM(PointM point)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            return new Point(point.X, point.Y);
        }

        public static PointM ToGeometryPointM(Point point)
        {
            return new PointM(point.X, point.Y);
        }

        public static Polygon FromGeometryPolygon(NetTopologySuite.Geometries.Polygon polygon)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon));

            var boundingBox = new BoundingBox2D(
                polygon.EnvelopeInternal.MinX,
                polygon.EnvelopeInternal.MinY,
                polygon.EnvelopeInternal.MaxX,
                polygon.EnvelopeInternal.MaxY
            );

            var lineStrings = new List<NetTopologySuite.Geometries.LineString>
            {
                (NetTopologySuite.Geometries.LineString) polygon.ExteriorRing
            };

            lineStrings.AddRange(
                polygon
                .InteriorRings
                .Cast<NetTopologySuite.Geometries.LineString>());

            var offset = 0;
            var parts = new int[lineStrings.Count];
            for(var index = 0; index < lineStrings.Count; index++)
            {
                parts[index] = offset;
                var line = lineStrings[index];
                offset += line.NumPoints;
            }

            var points = lineStrings
                .SelectMany(line => line.Coordinates)
                .Select(coordinate => new Point(coordinate.X, coordinate.Y))
                .ToArray();

            return new Polygon(boundingBox, parts, points);
        }

        public static NetTopologySuite.Geometries.Polygon ToGeometryPolygon(Polygon polygon)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon));
            var linearRings = new GeoAPI.Geometries.ILinearRing[polygon.NumberOfParts];
            var toPointIndex = polygon.NumberOfPoints;

            for (var partIndex = polygon.NumberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = polygon.Parts[partIndex];

                linearRings[partIndex] = new NetTopologySuite.Geometries.LinearRing(
                    new NetTopologySuite.Geometries.Implementation.CoordinateArraySequence(
                        polygon.Points
                            .Skip(fromPointIndex)
                            .Take(toPointIndex - fromPointIndex)
                            .Select(x => new GeoAPI.Geometries.Coordinate(x.X, x.Y))
                            .ToArray()),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            return new NetTopologySuite.Geometries.Polygon(linearRings[0], linearRings.Skip(1).ToArray());
        }

        public static PolyLineM FromGeometryMultiLineString(NetTopologySuite.Geometries.MultiLineString multiLineString)
        {
            if (multiLineString == null) throw new ArgumentNullException(nameof(multiLineString));

            var boundingBox = new BoundingBox2D(
                multiLineString.EnvelopeInternal.MinX,
                multiLineString.EnvelopeInternal.MinY,
                multiLineString.EnvelopeInternal.MaxX,
                multiLineString.EnvelopeInternal.MaxY
            );

            var lineStrings = multiLineString
                .Geometries
                .Cast<NetTopologySuite.Geometries.LineString>()
                .ToArray();

            var offset = 0;
            var parts = new int[lineStrings.Length];
            for (var index = 0; index < lineStrings.Length; index++)
            {
                parts[index] = offset;
                var line = lineStrings[index];
                offset += line.NumPoints;
            }

            var points = lineStrings
                .SelectMany(line => line.Coordinates)
                .Select(coordinate => new Point(coordinate.X, coordinate.Y))
                .ToArray();

            var measures = multiLineString.GetOrdinates(GeoAPI.Geometries.Ordinate.M).ToArray();

            return new PolyLineM(boundingBox, parts, points, measures);
        }

        public static NetTopologySuite.Geometries.MultiLineString ToGeometryMultiLineString(PolyLineM polyLineM)
        {
            if (polyLineM == null) throw new ArgumentNullException(nameof(polyLineM));

            var points = Array.ConvertAll(polyLineM.Points, point => new PointM(point.X, point.Y));
            if (polyLineM.Measures != null)
            {
                for (var measureIndex = 0; measureIndex < polyLineM.Measures.Length; measureIndex++)
                {
                    points[measureIndex].ChangeMeasurement(polyLineM.Measures[measureIndex]);
                }
            }

            var lines = new GeoAPI.Geometries.ILineString[polyLineM.NumberOfParts];
            var toPointIndex = points.Length;

            for (var partIndex = polyLineM.NumberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = polyLineM.Parts[partIndex];

                lines[partIndex] = new NetTopologySuite.Geometries.LineString(
                    new PointSequence(new ArraySegment<PointM>(points, fromPointIndex, toPointIndex - fromPointIndex)),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            return new NetTopologySuite.Geometries.MultiLineString(lines);
        }
    }
}
