namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class AnonymousShapeContentTests
    {
        private readonly Fixture _fixture;
        private readonly int _polygonExteriorBufferCoordinate = 50;

        public AnonymousShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        generator.Next(_polygonExteriorBufferCoordinate),
                        generator.Next(_polygonExteriorBufferCoordinate)
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<PointM>(customization =>
                customization.FromFactory(generator =>
                    new PointM(
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<ILineString>(customization =>
                customization.FromFactory(generator =>
                    new LineString(
                        new PointSequence(_fixture.CreateMany<PointM>(generator.Next(2, 10))),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<MultiLineString>(customization =>
                customization.FromFactory(generator =>
                    new MultiLineString(_fixture.CreateMany<ILineString>(generator.Next(2, 20)).ToArray())
                ).OmitAutoProperties()
            );
            _fixture.Customize<ILinearRing>(customization =>
                customization.FromFactory(generator =>
                {
                    var coordinates = _fixture.CreateMany<Point>(generator.Next(3, 10))
                        .Select(point => new Coordinate(point.X, point.Y))
                        .ToList();

                    var coordinate = coordinates.First();
                    coordinates.Add(new Coordinate(coordinate.X, coordinate.Y)); //first coordinate must be last

                    return new LinearRing(
                        new CoordinateArraySequence(coordinates.ToArray()),
                        GeometryConfiguration.GeometryFactory
                    );
                }).OmitAutoProperties()
            );
            _fixture.Customize<Polygon>(customization =>
                customization.FromFactory(generator =>
                {
                    var exteriorCoordinates = _fixture.CreateMany<Point>(generator.Next(3, 10))
                        .Select(point => new Coordinate(point.X + _polygonExteriorBufferCoordinate, point.Y + _polygonExteriorBufferCoordinate))
                        .ToList();

                    var coordinate = exteriorCoordinates.First();
                    exteriorCoordinates.Add(new Coordinate(coordinate.X, coordinate.Y)); //first coordinate must be last

                    var exteriorRing = new LinearRing(new CoordinateArraySequence(exteriorCoordinates.ToArray()), GeometryConfiguration.GeometryFactory);

                    return new Polygon(exteriorRing,
                        _fixture.CreateMany<ILinearRing>(generator.Next(1, 5)).ToArray(),
                        GeometryConfiguration.GeometryFactory);
                }).OmitAutoProperties()
            );
            _fixture.Customize<ShapeContent>(
                composer => composer.FromFactory(random =>
                    {
                        var content = NullShapeContent.Instance;
                        switch (random.Next() % 4)
                        {
                            // case 0: null shape
                            case 1:
                                content = _fixture.Create<PointShapeContent>();
                                break;
                            case 2:
                                content = _fixture.Create<PolyLineMShapeContent>();
                                break;
                            case 3:
                                content = _fixture.Create<PolygonShapeContent>();
                                break;
                        }

                        return content;
                    }
                )
            );
            _fixture.Customize<AnonymousShapeContent>(
                composer => composer.FromFactory(random =>
                    {
                        AnonymousShapeContent content = null;
                        switch (random.Next() % 3)
                        {
                            case 0:
                                content = (AnonymousShapeContent)_fixture
                                    .Create<PointShapeContent>()
                                    .Anonymous();
                                break;
                            case 1:
                                content = (AnonymousShapeContent)_fixture
                                    .Create<PolyLineMShapeContent>()
                                    .Anonymous();
                                break;
                            case 2:
                                content = (AnonymousShapeContent)_fixture
                                    .Create<PolygonShapeContent>()
                                    .Anonymous();
                                break;
                        }

                        return content;
                    }
                )
            );
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ShapeTypeReturnsExpectedValue()
        {
            var content = _fixture.Create<ShapeContent>();
            var sut = content.Anonymous();
            Assert.Equal(content.ShapeType, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var content = _fixture.Create<ShapeContent>();
            var sut = content.Anonymous();
            Assert.Equal(content.ContentLength, sut.ContentLength);
        }

        [Fact]
        public void ContentReturnsExpectedValue()
        {
            var content = _fixture.Create<ShapeContent>();
            if (content is NullShapeContent)
            {
                Assert.Same(content, content.Anonymous());
            }
            else
            {
                var sut = (AnonymousShapeContent)content.Anonymous();
                Assert.Equal(content.ToBytes(), sut.Content);
            }
        }

        [Fact]
        public void WriteWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<AnonymousShapeContent>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void ToBytesHasExpectedResult()
        {
            var sut = _fixture.Create<AnonymousShapeContent>();

            var result = sut.ToBytes();

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                sut.Write(writer);
                writer.Flush();

                Assert.Equal(stream.ToArray(), result);
            }
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = _fixture.Create<AnonymousShapeContent>();

            var result = sut.ToBytes(Encoding.UTF8);

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                sut.Write(writer);
                writer.Flush();

                Assert.Equal(stream.ToArray(), result);
            }
        }

        [Fact]
        public void ReadCanReadWrittenAnonymousShape()
        {
            var sut = _fixture.Create<AnonymousShapeContent>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = ShapeContent.Read(reader);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                    switch (result)
                    {
                        case PointShapeContent instance:
                            Assert.Equal(sut.Content, instance.ToBytes(Encoding.ASCII));
                            break;
                        case PolyLineMShapeContent instance:
                            Assert.Equal(sut.Content, instance.ToBytes(Encoding.ASCII));
                            break;
                    }
                }
            }
        }
    }
}
