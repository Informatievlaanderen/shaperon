namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Albedo;
    using AutoFixture.Idioms;
    using Xunit;
    using System.IO;
    using System.Text;

    public partial class NullShapeContentTests
    {
        private readonly Fixture _fixture;

        public NullShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeRecordNumber();
            _fixture.Register(() => (NullShapeContent) NullShapeContent.Instance);
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ReadReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => NullShapeContent.ReadNull(null)));
        }

        [Fact]
        public void WriteWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<NullShapeContent>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void LengthReturnsExpectedValue()
        {
            Assert.Equal(new WordLength(2), NullShapeContent.Length);
        }

        [Fact]
        public void ShapeTypeReturnsExpectedValue()
        {
            Assert.Equal(ShapeType.NullShape, NullShapeContent.Instance.ShapeType);
        }

        [Fact]
        public void ContentLengthReturnsExpectedValue()
        {
            Assert.Equal(new WordLength(2), NullShapeContent.Instance.ContentLength);
        }

        [Fact]
        public void ReadCanReadWriteNullShape()
        {
            var sut = NullShapeContent.Instance;

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
                    var result = (NullShapeContent) ShapeContent.Read(reader);

                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ReadNullCanReadWriteNullShape()
        {
            var sut = NullShapeContent.Instance;

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
                    var result = (NullShapeContent) NullShapeContent.ReadNull(reader);

                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
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
        [InlineData(ShapeType.PolyLineM)]
        [InlineData(ShapeType.PolygonM)]
        [InlineData(ShapeType.MultiPointM)]
        [InlineData(ShapeType.MultiPatch)]
        public void ReadNullCanNotReadOtherShapeType(ShapeType other)
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
                        () => NullShapeContent.ReadNull(reader)
                    );
                }
            }
        }
    }
}
