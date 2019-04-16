namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;
    using NetTopologySuite.Geometries;

    public class PolygonShapeContentTests
    {
        private readonly Fixture _fixture;

        public PolygonShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizePolygon();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ReadPolygonReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => PolygonShapeContent.ReadPolygon(null)));
        }

        [Fact]
        public void WriteWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<PolygonShapeContent>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void ShapeTypeReturnsExpectedValue()
        {
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());
            Assert.Equal(ShapeType.Polygon, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var shape = _fixture.Create<Polygon>();
            var sut = new PolygonShapeContent(shape);
            var numberOfParts = shape.InteriorRings.Length + 1;
            var numberOfPoints = shape.NumPoints;
            var contentLength = new WordLength(
                2 + 2 + 2 // shape type, number of parts, number of points,
                + 4 * 4 // bounding box,
                + 2 * numberOfParts // parts
                + 4 * 2 * numberOfPoints // points
            );
            Assert.Equal(contentLength, sut.ContentLength);
        }

        [Fact]
        public void ShapeReturnsExpectedValue()
        {
            var shape = _fixture.Create<Polygon>();
            var sut = new PolygonShapeContent(shape);
            Assert.Same(shape, sut.Shape);
        }

        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());

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
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());

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
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());

            var result = ShapeContent.FromBytes(sut.ToBytes());

            var actual = Assert.IsType<PolygonShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());

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
            var sut = new PolygonShapeContent(_fixture.Create<Polygon>());

            var result = ShapeContent.FromBytes(sut.ToBytes(Encoding.UTF8), Encoding.UTF8);

            var actual = Assert.IsType<PolygonShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ReadCanReadWrittenPolygonShape()
        {
            var shape = _fixture.Create<Polygon>();
            var sut = new PolygonShapeContent(shape);

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
                    var result = (PolygonShapeContent) ShapeContent.Read(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadAnonymousCanReadWrittenPolygonMShape()
        {
            var shape = _fixture.Create<Polygon>();
            var sut = new PolygonShapeContent(shape);

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
        public void ReadPolygonCanReadWrittenPolygonShape()
        {
            var shape = _fixture.Create<Polygon>();
            var sut = new PolygonShapeContent(shape);

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
                    var result = (PolygonShapeContent)PolygonShapeContent.ReadPolygon(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadPolygonCanReadWrittenNullShape()
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
                    var result = PolygonShapeContent.ReadPolygon(reader);

                    Assert.Equal(NullShapeContent.Instance, result);
                }
            }
        }

        [Theory]
        [InlineData(ShapeType.Point)]
        [InlineData(ShapeType.PolyLine)]
        [InlineData(ShapeType.MultiPoint)]
        [InlineData(ShapeType.PointZ)]
        [InlineData(ShapeType.PolyLineZ)]
        [InlineData(ShapeType.PolygonZ)]
        [InlineData(ShapeType.MultiPointZ)]
        [InlineData(ShapeType.PointM)]
        [InlineData(ShapeType.PolyLineM)]
        [InlineData(ShapeType.PolygonM)]
        [InlineData(ShapeType.MultiPointM)]
        [InlineData(ShapeType.MultiPatch)]
        public void ReadPolygonCanNotReadOtherShapeType(ShapeType other)
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
                        () => PolygonShapeContent.ReadPolygon(reader)
                    );
                }
            }
        }
    }
}
