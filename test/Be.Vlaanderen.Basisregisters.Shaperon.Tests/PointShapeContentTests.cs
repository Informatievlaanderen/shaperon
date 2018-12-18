namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;

    public class PointShapeContentTests
    {
        private readonly Fixture _fixture;

        public PointShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<PointM>(customization =>
                customization.FromFactory(generator =>
                    new PointM(_fixture.Create<double>(), _fixture.Create<double>())
                ).OmitAutoProperties()
            );
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
            var sut = new PointShapeContent(_fixture.Create<PointM>());
            Assert.Equal(ShapeType.Point, sut.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());
            Assert.Equal(new WordLength(10), sut.ContentLength);
        }

        [Fact]
        public void ShapeReturnsExpectedValue()
        {
            var shape = _fixture.Create<PointM>();
            var sut = new PointShapeContent(shape);
            Assert.Same(shape, sut.Shape);
        }

        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
            var sut = new PointShapeContent(_fixture.Create<PointM>());

            var result = ShapeContent.FromBytes(sut.ToBytes());

            var actual = Assert.IsType<PointShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
            var sut = new PointShapeContent(_fixture.Create<PointM>());

            var result = ShapeContent.FromBytes(sut.ToBytes(Encoding.UTF8), Encoding.UTF8);

            var actual = Assert.IsType<PointShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ReadCanReadWrittenPointShape()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
        public void ReadAnonymousCanReadWrittenPointShape()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
        public void ReadPointCanReadWrittenPointShape()
        {
            var sut = new PointShapeContent(_fixture.Create<PointM>());

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
