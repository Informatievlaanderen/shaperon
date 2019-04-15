namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Xunit;
    using System.IO;
    using System;
    using System.Text;
    using AutoFixture.Idioms;
    using Albedo;

    public class ShapeContentTests
    {
        private readonly Fixture _fixture;

        public ShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeRecordNumber();
            _fixture.CustomizeWordLength();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
        }

        [Fact]
        public void ShapeTypeLengthReturnsExpectedValue()
        {
            Assert.Equal(new WordLength(2), ShapeContent.ShapeTypeLength);
        }

        [Fact]
        public void PropertiesReturnExpectedValue()
        {
            var shapeType = _fixture.Create<ShapeType>();
            var contentLength = _fixture.Create<WordLength>();
            var boundingBox = _fixture.Create<BoundingBox3D>();
            var sut = new ShapeContentUnderTest(
                shapeType,
                contentLength
            );
            Assert.Equal(shapeType, sut.ShapeType);
            Assert.Equal(contentLength, sut.ContentLength);
        }


        [Fact]
        public void ReadReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Methods.Select(() => ShapeContent.Read(null)));
        }

        [Fact]
        public void RecordAsReturnsExpectedResult()
        {
            var number = _fixture.Create<RecordNumber>();
            var sut = new ShapeContentUnderTest(
                _fixture.Create<ShapeType>(),
                _fixture.Create<WordLength>()
            );

            var result = sut.RecordAs(number);

            Assert.Equal(sut, result.Content);
            Assert.Equal(number, result.Header.RecordNumber);
            Assert.Equal(sut.ContentLength, result.Header.ContentLength);
        }

        [Theory]
        [InlineData(ShapeType.PolyLine)]
        [InlineData(ShapeType.MultiPoint)]
        [InlineData(ShapeType.PointZ)]
        [InlineData(ShapeType.PolyLineZ)]
        [InlineData(ShapeType.PolygonZ)]
        [InlineData(ShapeType.MultiPointZ)]
        [InlineData(ShapeType.PointM)]
        [InlineData(ShapeType.PolygonM)]
        [InlineData(ShapeType.MultiPointM)]
        [InlineData(ShapeType.MultiPatch)]
        public void ReadCanNotReadOtherShapeType(ShapeType other)
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
                        () => ShapeContent.Read(reader)
                    );
                }
            }
        }

        private class ShapeContentUnderTest : ShapeContent
        {
            public ShapeContentUnderTest(ShapeType shapeType, WordLength contentLength)
            {
                ShapeType = shapeType;
                ContentLength = contentLength;
            }

            public override void Write(BinaryWriter writer)
            {
                if (writer == null)
                {
                    throw new ArgumentNullException(nameof(writer));
                }
            }
        }
    }
}
