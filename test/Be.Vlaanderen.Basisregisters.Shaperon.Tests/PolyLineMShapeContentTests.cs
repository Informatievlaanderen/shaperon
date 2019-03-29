namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;
    using System.Linq;

    public partial class PolyLineMShapeContentTests
    {
        private readonly Fixture _fixture;

        public PolyLineMShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizePoint();
            _fixture.CustomizePolyLineM();
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
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());
            Assert.Equal(ShapeType.PolyLineM, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var shape = _fixture.Create<PolyLineM>();
            var sut = new PolyLineMShapeContent(shape);
            var numberOfParts = shape.NumberOfParts;
            var numberOfPoints = shape.NumberOfPoints;
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
            var shape = _fixture.Create<PolyLineM>();
            var sut = new PolyLineMShapeContent(shape);
            Assert.Same(shape, sut.Shape);
        }

        [Fact]
        public void ReadCanReadWrittenPolyLineMShape()
        {
            var shape = _fixture.Create<PolyLineM>();
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
        public void ReadPolyLineMCanReadWrittenPolyLineMShape()
        {
            var shape = _fixture.Create<PolyLineM>();
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
