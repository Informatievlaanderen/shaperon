namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;

    public partial class PointShapeContentTests
    {
        private readonly Fixture _fixture;

        public PointShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizePoint();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ReadPointReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => PointShapeContent.ReadPoint(null)));
        }

        [Fact]
        public void WriteWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<PointShapeContent>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void LengthReturnsExpectedValue()
        {
            Assert.Equal(new WordLength(10), PointShapeContent.Length);
        }

        [Fact]
        public void ShapeTypeReturnsExpectedValue()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());
            Assert.Equal(ShapeType.Point, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());
            Assert.Equal(new WordLength(10), sut.ContentLength);
        }

        [Fact]
        public void ShapeReturnsExpectedValue()
        {
            var shape = _fixture.Create<Point>();
            var sut = new PointShapeContent(shape);
            Assert.Equal(shape, sut.Shape);
        }

        [Fact]
        public void ReadCanReadWrittenPointShape()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());

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
                    var result = (PointShapeContent) ShapeContent.Read(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadPointCanReadWrittenPointShape()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());

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
                    var result = (PointShapeContent) PointShapeContent.ReadPoint(reader);

                    Assert.Equal(sut.Shape, result.Shape);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadPointCanReadWrittenNullShape()
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
                    var result = PointShapeContent.ReadPoint(reader);

                    Assert.Equal(NullShapeContent.Instance, result);
                }
            }
        }

        [Theory]
        [InlineData(ShapeType.PolyLine)]
        [InlineData(ShapeType.Polygon)]
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
        public void ReadPointCanNotReadOtherShapeType(ShapeType other)
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
                        () => PointShapeContent.ReadPoint(reader)
                    );
                }
            }
        }
    }
}
