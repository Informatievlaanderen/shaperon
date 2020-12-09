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
                polygon.ExteriorRing
            };

            lineStrings.AddRange(polygon.InteriorRings);

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
            var linearRings = new NetTopologySuite.Geometries.LinearRing[polygon.NumberOfParts];
            var toPointIndex = polygon.NumberOfPoints;

            for (var partIndex = polygon.NumberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = polygon.Parts[partIndex];

                linearRings[partIndex] = new NetTopologySuite.Geometries.LinearRing(
                    GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(
                        polygon.Points
                            .Skip(fromPointIndex)
                            .Take(toPointIndex - fromPointIndex)
                            .Select(point => new NetTopologySuite.Geometries.Coordinate(point.X, point.Y))
                            .ToArray()),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            return new NetTopologySuite.Geometries.Polygon(
                linearRings[0],
                linearRings.Length > 1
                    ? linearRings.Skip(1).ToArray()
                    : Array.Empty<NetTopologySuite.Geometries.LinearRing>());
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

            var measures = multiLineString.GetOrdinates(NetTopologySuite.Geometries.Ordinate.M).ToArray();

            return new PolyLineM(boundingBox, parts, points, measures);
        }

        public static NetTopologySuite.Geometries.MultiLineString ToGeometryMultiLineString(PolyLineM polyLineM)
        {
            if (polyLineM == null) throw new ArgumentNullException(nameof(polyLineM));

            var coordinates = Array.ConvertAll(polyLineM.Points, point => new NetTopologySuite.Geometries.Coordinate(point.X, point.Y));
            if (polyLineM.Measures != null)
            {
                for (var measureIndex = 0; measureIndex < polyLineM.Measures.Length; measureIndex++)
                {
                    coordinates[measureIndex] = new NetTopologySuite.Geometries.CoordinateM(coordinates[measureIndex].X, coordinates[measureIndex].Y, polyLineM.Measures[measureIndex]);
                }
            }

            var lines = new NetTopologySuite.Geometries.LineString[polyLineM.NumberOfParts];
            var toCoordinateIndex = coordinates.Length;

            for (var partIndex = polyLineM.NumberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromCoordinateIndex = polyLineM.Parts[partIndex];

                lines[partIndex] = new NetTopologySuite.Geometries.LineString(
                    GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(
                        new ArraySegment<NetTopologySuite.Geometries.Coordinate>(
                                coordinates,
                                fromCoordinateIndex,
                                toCoordinateIndex - fromCoordinateIndex).ToArray()
                    ),
                    GeometryConfiguration.GeometryFactory);

                toCoordinateIndex = fromCoordinateIndex;
            }

            return new NetTopologySuite.Geometries.MultiLineString(lines);
        }

        public static NetTopologySuite.Geometries.MultiPolygon ToGeometryMultiPolygon(Polygon polygon)
        {
            if (polygon == null) throw new ArgumentNullException(nameof(polygon));
            var linearRings = new NetTopologySuite.Geometries.LinearRing[polygon.NumberOfParts];
            var toPointIndex = polygon.NumberOfPoints;

            for (var partIndex = polygon.NumberOfParts - 1; partIndex >= 0; partIndex--)
            {
                var fromPointIndex = polygon.Parts[partIndex];

                linearRings[partIndex] = new NetTopologySuite.Geometries.LinearRing(
                    GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(
                        polygon.Points
                            .Skip(fromPointIndex)
                            .Take(toPointIndex - fromPointIndex)
                            .Select(point => new NetTopologySuite.Geometries.Coordinate(point.X, point.Y))
                            .ToArray()),
                    GeometryConfiguration.GeometryFactory);

                toPointIndex = fromPointIndex;
            }

            var polygons = new List<NetTopologySuite.Geometries.Polygon>();
            var ringIndex = 0;
            while (ringIndex < linearRings.Length)
            {
                var shell = linearRings[ringIndex];
                if (shell.IsCCW)
                {
                    throw new InvalidOperationException("The shell of a polygon must have a clockwise orientation.");
                }
                var holes = new List<NetTopologySuite.Geometries.LinearRing>();
                ringIndex++;
                while (ringIndex < linearRings.Length && linearRings[ringIndex].IsCCW)
                {
                    holes.Add(linearRings[ringIndex]);
                    ringIndex++;
                }

                polygons.Add(new NetTopologySuite.Geometries.Polygon(
                    shell,
                    holes.ToArray(),
                    GeometryConfiguration.GeometryFactory));
            }
            return new NetTopologySuite.Geometries.MultiPolygon(polygons.ToArray(), GeometryConfiguration.GeometryFactory);
        }

        public static Polygon FromGeometryMultiPolygon(NetTopologySuite.Geometries.MultiPolygon multiPolygon)
        {
            if (multiPolygon == null) throw new ArgumentNullException(nameof(multiPolygon));

            var boundingBox = new BoundingBox2D(
                multiPolygon.EnvelopeInternal.MinX,
                multiPolygon.EnvelopeInternal.MinY,
                multiPolygon.EnvelopeInternal.MaxX,
                multiPolygon.EnvelopeInternal.MaxY
            );

            var linearRings = multiPolygon
                .Geometries
                .Cast<NetTopologySuite.Geometries.Polygon>()
                .SelectMany(polygon => new [] { polygon.Shell }.Concat(polygon.Holes))
                .ToArray();

            var offset = 0;
            var parts = new int[linearRings.Length];
            for (var index = 0; index < linearRings.Length; index++)
            {
                parts[index] = offset;
                var line = linearRings[index];
                offset += line.NumPoints;
            }

            var points = linearRings
                .SelectMany(line => line.Coordinates)
                .Select(coordinate => new Point(coordinate.X, coordinate.Y))
                .ToArray();

            return new Polygon(boundingBox, parts, points);
        }
    }
}
