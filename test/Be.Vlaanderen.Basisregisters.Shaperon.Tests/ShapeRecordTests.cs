namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using System.IO;
    using System.Text;
    using Xunit;

    public class ShapeRecordTests
    {
        private readonly Fixture _fixture;

        public ShapeRecordTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeRecordNumber();
            _fixture.CustomizeWordLength();
            _fixture.CustomizeWordOffset();
            _fixture.CustomizePoint();
            _fixture.CustomizePolygon();
            _fixture.CustomizePolyLineM();
            _fixture.Customize<ShapeContent>(customization =>
                customization.FromFactory<int>(value =>
                {
                    ShapeContent content = null;
                    switch (value % 4)
                    {
                        case 0:
                            content = NullShapeContent.Instance;
                            break;
                        case 1:
                            content = new PointShapeContent(_fixture.Create<Point>());
                            break;
                        case 2:
                            content = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());
                            break;
                        case 3:
                            content = new PolygonShapeContent(_fixture.Create<Polygon>());
                            break;
                    }

                    return content;
                }));
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void IndexAtReturnsExpectedResult()
        {
            var offset = _fixture.Create<WordOffset>();
            var sut = _fixture.Create<ShapeRecord>();

            var result = sut.IndexAt(offset);

            Assert.Equal(sut.Header.ContentLength, result.ContentLength);
            Assert.Equal(offset, result.Offset);
        }

        [Fact]
        public void InitialOffsetReturnsExpectedValue()
        {
            Assert.Equal(new WordOffset(50), ShapeRecord.InitialOffset);
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => ShapeRecord.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<ShapeRecord>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<ShapeRecord>();

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
                    var result = ShapeRecord.Read(reader);

                    Assert.Equal(sut.Header.RecordNumber, result.Header.RecordNumber);
                    Assert.Equal(sut.Header.ContentLength, result.Header.ContentLength);
                    Assert.Equal(sut.Content.ShapeType, result.Content.ShapeType);
                    Assert.Equal(sut.Content.ContentLength, result.Content.ContentLength);
                }
            }
        }
    }
}
