namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Xunit;

    public class GeometryTranslatorTests
    {
        private readonly Fixture _fixture;

        public GeometryTranslatorTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void FromGeometryPointCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.FromGeometryPoint(null));
        }

        [Fact]
        public void ToGeometryPointReturnsExpectedResult()
        {
            _fixture.CustomizePoint();
            var input = _fixture.Create<Point>();
            var result = GeometryTranslator.ToGeometryPoint(input);
            Assert.Equal(input.X, result.X);
            Assert.Equal(input.Y, result.Y);
        }

        [Fact]
        public void FromGeometryPointReturnsExpectedResult()
        {
            _fixture.CustomizeGeometryPoint();
            var input = _fixture.Create<NetTopologySuite.Geometries.Point>();
            var result = GeometryTranslator.FromGeometryPoint(input);
            Assert.Equal(result.X, input.X);
            Assert.Equal(result.Y, input.Y);
        }

        [Fact]
        public void FromGeometryPolygonCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.FromGeometryPolygon(null));
        }

        [Fact]
        public void FromGeometryPolygonReturnsExpectedResult()
        {
            _fixture.CustomizeGeometryPolygon();
            var input = _fixture.Create<NetTopologySuite.Geometries.Polygon>();
            var result = GeometryTranslator.FromGeometryPolygon(input);
            var rings = new List<NetTopologySuite.Geometries.LineString>(new[] {input.ExteriorRing});
            rings.AddRange(input.InteriorRings);

            Assert.Equal(new BoundingBox2D(
                rings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).Min(point => point.X),
                rings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).Min(point => point.Y),
                rings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).Max(point => point.X),
                rings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).Max(point => point.Y)
            ), result.BoundingBox);
            Assert.Equal(rings.Count, result.NumberOfParts);
            Assert.Equal(rings.Sum(ring => ring.NumPoints), result.NumberOfPoints);
            var offset = 0;
            Assert.Equal(
                rings.Select(ring =>
                {
                    var part = offset;
                    offset += ring.NumPoints;
                    return part;
                }).ToArray(),
                result.Parts);
            Assert.Equal(
                rings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).ToArray(),
                result.Points);
        }

        [Fact]
        public void ToGeometryPolygonCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.ToGeometryPolygon(null));
        }

        [Fact]
        public void ToGeometryPolygonReturnsExpectedResult()
        {
            _fixture.CustomizePolygon();
            var input = _fixture.Create<Polygon>();
            var result = GeometryTranslator.ToGeometryPolygon(input);

            Assert.NotNull(result.ExteriorRing);
            Assert.Equal(input.NumberOfParts - 1, result.Holes.Length);
            Assert.Equal(input.NumberOfPoints, result.NumPoints);
            Assert.Equal(input.Points,
                Enumerable
                    .Range(0, result.Shell.NumPoints)
                    .Select(index => new Point(result.Shell.GetPointN(index).X, result.Shell.GetPointN(index).Y))
                    .Concat(
                        result
                            .Holes
                            .SelectMany(hole =>
                                Enumerable
                                    .Range(0, hole.NumPoints)
                                    .Select(index => new Point(hole.GetPointN(index).X, hole.GetPointN(index).Y))
                            )
                    )
                    .ToArray()
            );
        }

        [Fact]
        public void FromGeometryMultiLineStringCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.FromGeometryMultiLineString(null));
        }

        [Fact]
        public void FromGeometryMultiLineStringReturnsExpectedResult()
        {
            _fixture.CustomizeGeometryMultiLineString();
            var input = _fixture.Create<NetTopologySuite.Geometries.MultiLineString>();
            var result = GeometryTranslator.FromGeometryMultiLineString(input);
            var lines = new List<NetTopologySuite.Geometries.LineString>(input.Geometries.Cast<NetTopologySuite.Geometries.LineString>());

            Assert.Equal(new BoundingBox2D(
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => new Point(line.GetPointN(index).X, line.GetPointN(index).Y))).Min(point => point.X),
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => new Point(line.GetPointN(index).X, line.GetPointN(index).Y))).Min(point => point.Y),
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => new Point(line.GetPointN(index).X, line.GetPointN(index).Y))).Max(point => point.X),
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => new Point(line.GetPointN(index).X, line.GetPointN(index).Y))).Max(point => point.Y)
            ), result.BoundingBox);
            Assert.Equal(lines.Count, result.NumberOfParts);
            Assert.Equal(lines.Sum(line => line.NumPoints), result.NumberOfPoints);
            var offset = 0;
            Assert.Equal(
                lines.Select(line =>
                {
                    var part = offset;
                    offset += line.NumPoints;
                    return part;
                }).ToArray(),
                result.Parts);
            Assert.Equal(
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => new Point(line.GetPointN(index).X, line.GetPointN(index).Y))).ToArray(),
                result.Points);
            Assert.Equal(new MeasureRange(
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => line.CoordinateSequence.GetOrdinate(index, NetTopologySuite.Geometries.Ordinate.M))).Min(),
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => line.CoordinateSequence.GetOrdinate(index, NetTopologySuite.Geometries.Ordinate.M))).Max()
            ), result.MeasureRange);
            Assert.Equal(
                lines.SelectMany(line => Enumerable.Range(0, line.NumPoints).Select(index => line.CoordinateSequence.GetOrdinate(index, NetTopologySuite.Geometries.Ordinate.M))),
                result.Measures);
        }

        [Fact]
        public void ToGeometryMultiLineStringCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.ToGeometryMultiLineString(null));
        }

        [Fact]
        public void ToGeometryMultiLineStringReturnsExpectedResult()
        {
            _fixture.CustomizePolyLineM();
            var input = _fixture.Create<PolyLineM>();
            var result = GeometryTranslator.ToGeometryMultiLineString(input);

            Assert.Equal(input.NumberOfParts, result.Geometries.Length);
            Assert.Equal(input.NumberOfPoints, result.NumPoints);
            Assert.Equal(input.Points,
                result
                    .Geometries
                    .SelectMany(geometry =>
                        Enumerable
                            .Range(0, geometry.NumPoints)
                            .Select(index => ((NetTopologySuite.Geometries.LineString) geometry).GetPointN(index))
                    )
                    .Select(point => new Point(point.X, point.Y))
                    .ToArray()
            );
            var lines = new List<NetTopologySuite.Geometries.LineString>(result.Geometries.Cast<NetTopologySuite.Geometries.LineString>());
            Assert.Equal(
                input.Measures,
                lines
                    .SelectMany(line =>
                        Enumerable
                            .Range(0, line.NumPoints)
                            .Select(index => line.CoordinateSequence.GetOrdinate(index, NetTopologySuite.Geometries.Ordinate.M)))
            );
        }

        [Fact]
        public void FromGeometryMultiPolygonCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.FromGeometryMultiPolygon(null));
        }

        [Fact]
        public void FromGeometryMultiPolygonReturnsExpectedResult()
        {
            _fixture.CustomizeGeometryMultiPolygon();
            var input = _fixture.Create<NetTopologySuite.Geometries.MultiPolygon>();
            var result = GeometryTranslator.FromGeometryMultiPolygon(input);

            Assert.Equal(
                new BoundingBox2D(
                    input.EnvelopeInternal.MinX,
                    input.EnvelopeInternal.MinY,
                    input.EnvelopeInternal.MaxX,
                    input.EnvelopeInternal.MaxY
                ), result.BoundingBox);
            Assert.Equal(input.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().Sum(polygon => polygon.Holes.Length + 1),
                result.NumberOfParts);
            Assert.Equal(input.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().Sum(polygon => polygon.Shell.NumPoints + polygon.Holes.Sum(hole => hole.NumPoints)),
                result.NumberOfPoints);
            var linearRings = input.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().SelectMany(polygon => new [] { polygon.Shell}.Concat(polygon.Holes));
            var offset = 0;
            Assert.Equal(
                linearRings.Select(ring =>
                {
                    var part = offset;
                    offset += ring.NumPoints;
                    return part;
                }).ToArray(),
                result.Parts);
            Assert.Equal(
                linearRings.SelectMany(ring => Enumerable.Range(0, ring.NumPoints).Select(index => new Point(ring.GetPointN(index).X, ring.GetPointN(index).Y))).ToArray(),
                result.Points);
        }

        [Fact]
        public void ToGeometryMultiPolygonCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => GeometryTranslator.ToGeometryMultiPolygon(null));
        }

        [Fact]
        public void ToGeometryMultiPolygonReturnsExpectedResult()
        {
            _fixture.CustomizeMultiPolygon();
            var input = _fixture.Create<Polygon>();
            var result = GeometryTranslator.ToGeometryMultiPolygon(input);
            Assert.Equal(input.NumberOfParts, result.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().Sum(polygon => polygon.Holes.Length + 1));
            Assert.Equal(input.NumberOfPoints, result.Geometries.Cast<NetTopologySuite.Geometries.Polygon>().Sum(polygon => polygon.Shell.NumPoints + polygon.Holes.Sum(hole => hole.NumPoints)));
            Assert.Equal(input.BoundingBox, new BoundingBox2D(
                result.EnvelopeInternal.MinX,
                result.EnvelopeInternal.MinY,
                result.EnvelopeInternal.MaxX,
                result.EnvelopeInternal.MaxY
            ));
            var offset = 0;
            var parts = new List<int>();
            foreach (var linearRing in result
                .Geometries
                .Cast<NetTopologySuite.Geometries.Polygon>()
                .SelectMany(polygon => new[] {polygon.Shell}.Concat(polygon.Holes)))
            {
                parts.Add(offset);
                offset += linearRing.NumPoints;
            }
            Assert.Equal(input.Parts, parts);
            Assert.Equal(input.Points,
                result
                    .Geometries
                    .Cast<NetTopologySuite.Geometries.Polygon>()
                    .SelectMany(polygon =>
                        polygon
                            .Shell
                            .Coordinates
                            .Concat(
                                polygon
                                    .Holes
                                    .SelectMany(hole => hole.Coordinates))
                            .Select(point => new Point(point.X, point.Y))
                        ));
        }
    }
}
