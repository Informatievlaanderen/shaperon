namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;
    using NetTopologySuite.Geometries;
    using GeoAPI.Geometries;
    using System.Linq;

    public class PolyLineMShapeContentTests
    {
        private readonly Fixture _fixture;

        public PolyLineMShapeContentTests()
        {
            _fixture = new Fixture();
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
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ReadPolyLineMReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => PolyLineMShapeContent.ReadPolyLineM(null)));
        }

        [Fact]
        public void WriteWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<PolyLineMShapeContent>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void ShapeTypeReturnsExpectedValue()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());
            Assert.Equal(ShapeType.PolyLineM, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var shape = _fixture.Create<MultiLineString>();
            var sut = new PolyLineMShapeContent(shape);
            var numberOfParts = shape.NumGeometries;
            var numberOfPoints = shape.NumPoints;
            var contentLength = new WordLength(
                2 + 2 + 2 // shape type, number of parts, number of points,
                + 4 * 4 // bounding box,
                + 2 * numberOfParts // parts
                + 4 * 2 * numberOfPoints // points
                + 4 * 2 // measure range
                + 4 * numberOfPoints // measures
            );
            Assert.Equal(contentLength, sut.ContentLength);
        }

        [Fact]
        public void ShapeReturnsExpectedValue()
        {
            var shape = _fixture.Create<MultiLineString>();
            var sut = new PolyLineMShapeContent(shape);
            Assert.Same(shape, sut.Shape);
        }

        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());

            var result = sut.Anonymous();

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                sut.Write(writer);
                writer.Flush();

                Assert.Equal(sut.ShapeType, result.ShapeType);
                Assert.Equal(sut.ContentLength, result.ContentLength);
                Assert.Equal(
                    stream.ToArray(),
                    Assert.IsType<AnonymousShapeContent>(result).Content);
            }
        }


        [Fact]
        public void ToBytesHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());

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
        public void FromBytesHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());

            var result = ShapeContent.FromBytes(sut.ToBytes());

            var actual = Assert.IsType<PolyLineMShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());

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
        public void FromBytesWithEncodingHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<MultiLineString>());

            var result = ShapeContent.FromBytes(sut.ToBytes(Encoding.UTF8), Encoding.UTF8);

            var actual = Assert.IsType<PolyLineMShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ReadCanReadWrittenPolyLineMShape()
        {
            var shape = _fixture.Create<MultiLineString>();
            var sut = new PolyLineMShapeContent(shape);

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
                    var result = (PolyLineMShapeContent) ShapeContent.Read(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadAnonymousCanReadWrittenPolyLineMShape()
        {
            var shape = _fixture.Create<MultiLineString>();
            var sut = new PolyLineMShapeContent(shape);

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
                    var result = (AnonymousShapeContent) ShapeContent.ReadAnonymous(reader);

                    Assert.Equal(stream.ToArray(), result.Content);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadPolyLineMCanReadWrittenPolyLineMShape()
        {
            var shape = _fixture.Create<MultiLineString>();
            var sut = new PolyLineMShapeContent(shape);

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
                    var result = (PolyLineMShapeContent) PolyLineMShapeContent.ReadPolyLineM(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadPolyLineMCanReadWrittenNullShape()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    NullShapeContent.Instance.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = PolyLineMShapeContent.ReadPolyLineM(reader);

                    Assert.Equal(NullShapeContent.Instance, result);
                }
            }
        }

        [Theory]
        [InlineData(ShapeType.Point)]
        [InlineData(ShapeType.PolyLine)]
        [InlineData(ShapeType.Polygon)]
        [InlineData(ShapeType.MultiPoint)]
        [InlineData(ShapeType.PointZ)]
        [InlineData(ShapeType.PolyLineZ)]
        [InlineData(ShapeType.PolygonZ)]
        [InlineData(ShapeType.MultiPointZ)]
        [InlineData(ShapeType.PointM)]
        [InlineData(ShapeType.PolygonM)]
        [InlineData(ShapeType.MultiPointM)]
        [InlineData(ShapeType.MultiPatch)]
        public void ReadPolyLineMCanNotReadOtherShapeType(ShapeType other)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.WriteInt32LittleEndian((int) other);

                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    Assert.Throws<ShapeRecordContentException>(
                        () => PolyLineMShapeContent.ReadPolyLineM(reader)
                    );
                }
            }
        }
    }
}
