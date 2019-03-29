namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class ShapeBinaryWriterTests
    {
        private readonly Fixture _fixture;

        public ShapeBinaryWriterTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeShapeRecordCount(100);
            _fixture.CustomizeRecordNumber();
            _fixture.CustomizeWordLength();
            _fixture.CustomizeWordOffset();
            _fixture.Customize<ShapeType>(customization =>
                customization.FromFactory(generator =>
                    {
                        var result = ShapeType.NullShape;
                        switch (generator.Next() % 4)
                        {
                            //case 0:
                            case 1:
                                result = ShapeType.Point;
                                break;
                            case 2:
                                result = ShapeType.PolyLineM;
                                break;
                            case 3:
                                result = ShapeType.Polygon;
                                break;
                        }

                        return result;
                    }
                ).OmitAutoProperties()
            );
            _fixture.CustomizePoint();
            _fixture.CustomizePolygon();
            _fixture.CustomizePolyLineM();
            _fixture.Customize<ShapeContent>(customization =>
                customization.FromFactory(generator =>
                {
                    ShapeContent content = null;
                    switch (generator.Next() % 4)
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
            _fixture.Register<Stream>(() => new MemoryStream());
        }

        [Fact]
        public void HeaderOrWriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(Constructors.Select(() => new ShapeBinaryWriter(null, null)));
        }

        [Fact]
        public void PropertiesReturnExpectedValues()
        {
            var header = _fixture.Create<ShapeFileHeader>();
            using (var writer = new BinaryWriter(new MemoryStream()))
            using (var sut = new ShapeBinaryWriter(header, writer))
            {
                Assert.Same(header, sut.Header);
                Assert.Same(writer, sut.Writer);
            }
        }

        [Fact]
        public void WriteOneRecordCanNotBeNull()
        {
            var expectedHeader = new ShapeFileHeader(
                ShapeFileHeader.Length,
                _fixture.Create<ShapeType>(),
                BoundingBox3D.Empty
            );
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    Assert.Throws<ArgumentNullException>(() => sut.Write((ShapeRecord) null));
                }
            }
        }

        [Fact]
        public void WriteOneHasExpectedResult()
        {
            var shapeType = _fixture.Create<ShapeType>();
            var expectedRecord = GenerateOneRecord(shapeType);
            var length = ShapeFileHeader.Length.Plus(expectedRecord.Length);
            var boundingBox = BoundingBox3D.Empty;
            if (expectedRecord.Content is PointShapeContent pointContent)
                boundingBox = BoundingBox3D.FromGeometry(pointContent.Shape);
            if (expectedRecord.Content is PolyLineMShapeContent lineContent)
                boundingBox = BoundingBox3D.FromGeometry(lineContent.Shape);
            if (expectedRecord.Content is PolygonShapeContent polygonContent)
                boundingBox = BoundingBox3D.FromGeometry(polygonContent.Shape);
            var expectedHeader = new ShapeFileHeader(length, shapeType, boundingBox);
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    //Act
                    sut.Write(expectedRecord);
                }

                // Assert
                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var actualHeader = ShapeFileHeader.Read(reader);
                    var actualRecord = ShapeRecord.Read(reader);

                    Assert.Equal(expectedHeader, actualHeader);
                    Assert.Equal(expectedRecord, actualRecord, new ShapeRecordEqualityComparer());
                }
            }
        }

        [Fact]
        public void WriteOneOnDisposedInstanceHasExpectedResult()
        {
            var shapeType = _fixture.Create<ShapeType>();
            var expectedRecord = GenerateOneRecord(shapeType);
            var length = ShapeFileHeader.Length.Plus(expectedRecord.Length);
            var boundingBox = BoundingBox3D.Empty;
            if (expectedRecord.Content is PointShapeContent pointContent)
                boundingBox = BoundingBox3D.FromGeometry(pointContent.Shape);
            if (expectedRecord.Content is PolyLineMShapeContent lineContent)
                boundingBox = BoundingBox3D.FromGeometry(lineContent.Shape);
            var expectedHeader = new ShapeFileHeader(length, shapeType, boundingBox);
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => sut.Write(expectedRecord));
                }
            }
        }

        [Fact]
        public void WriteManyRecordsCanNotBeNull()
        {
            var expectedHeader = new ShapeFileHeader(
                ShapeFileHeader.Length,
                _fixture.Create<ShapeType>(),
                BoundingBox3D.Empty
            );
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    Assert.Throws<ArgumentNullException>(() => sut.Write((IEnumerable<ShapeRecord>) null));
                }
            }
        }

        [Fact]
        public void WriteManyHasExpectedResult()
        {
            var shapeType = _fixture.Create<ShapeType>();
            var recordCount = _fixture.Create<ShapeRecordCount>();
            var expectedRecords = GenerateManyRecords(shapeType, recordCount);
            var length = expectedRecords.Aggregate(ShapeFileHeader.Length,
                (result, current) => result.Plus(current.Length));
            var boundingBox = expectedRecords.Aggregate(BoundingBox3D.Empty,
                (result, current) =>
                {
                    if (current.Content is PointShapeContent pointContent)
                        return result.ExpandWith(BoundingBox3D.FromGeometry(pointContent.Shape));
                    if (current.Content is PolyLineMShapeContent lineContent)
                        return result.ExpandWith(BoundingBox3D.FromGeometry(lineContent.Shape));
                    if (current.Content is PolygonShapeContent polygonContent)
                        return result.ExpandWith(BoundingBox3D.FromGeometry(polygonContent.Shape));
                    return result;
                });
            var expectedHeader = new ShapeFileHeader(length, shapeType, boundingBox);
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    //Act
                    sut.Write(expectedRecords);
                }

                // Assert
                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var actualHeader = ShapeFileHeader.Read(reader);
                    var actualRecords = new List<ShapeRecord>();
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        actualRecords.Add(ShapeRecord.Read(reader));
                    }

                    Assert.Equal(expectedHeader, actualHeader);
                    Assert.Equal(expectedRecords, actualRecords, new ShapeRecordEqualityComparer());
                }
            }
        }

        [Fact]
        public void WriteManyOnDisposedInstanceHasExpectedResult()
        {
            var shapeType = _fixture.Create<ShapeType>();
            var recordCount = _fixture.Create<ShapeRecordCount>();
            var expectedRecords = GenerateManyRecords(shapeType, recordCount);
            var length = expectedRecords.Aggregate(ShapeFileHeader.Length,
                (result, current) => result.Plus(current.Length));
            var boundingBox = expectedRecords.Aggregate(BoundingBox3D.Empty,
                (result, current) =>
                {
                    if (current.Content is PointShapeContent pointContent)
                        return result.ExpandWith(BoundingBox3D.FromGeometry(pointContent.Shape));
                    if (current.Content is PolyLineMShapeContent lineContent)
                        return result.ExpandWith(BoundingBox3D.FromGeometry(lineContent.Shape));
                    return result;
                });
            var expectedHeader = new ShapeFileHeader(length, shapeType, boundingBox);
            using (var stream = new MemoryStream())
            {
                using (var sut = new ShapeBinaryWriter(expectedHeader, new BinaryWriter(stream, Encoding.ASCII, true)))
                {
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => sut.Write(expectedRecords));
                }
            }
        }


        [Fact]
        public void DisposeHasExpectedResult()
        {
            var expectedHeader = new ShapeFileHeader(
                ShapeFileHeader.Length,
                _fixture.Create<ShapeType>(),
                BoundingBox3D.Empty
            );

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, false))
                using (var sut = new ShapeBinaryWriter(expectedHeader, writer))
                {
                    sut.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => writer.Write(_fixture.Create<byte>()));
                }
            }
        }

        private ShapeRecord GenerateOneRecord(ShapeType typeOfShape)
        {
            ShapeContent content = null;
            switch (typeOfShape)
            {
                case ShapeType.Point:
                    content = new PointShapeContent(_fixture.Create<Point>());
                    break;
                case ShapeType.PolyLineM:
                    content = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());
                    break;
                case ShapeType.Polygon:
                    content = new PolygonShapeContent(_fixture.Create<Polygon>());
                    break;
                //case ShapeType.NullShape:
                default:
                    content = NullShapeContent.Instance;
                    break;
            }

            return content.RecordAs(RecordNumber.Initial);
        }

        private ShapeRecord[] GenerateManyRecords(ShapeType typeOfShape, ShapeRecordCount count)
        {
            var records = new ShapeRecord[count.ToInt32()];
            var number = RecordNumber.Initial;
            for (var index = 0; index < records.Length; index++)
            {
                ShapeContent content = null;
                switch (typeOfShape)
                {
                    case ShapeType.Point:
                        content = new PointShapeContent(_fixture.Create<Point>());
                        break;
                    case ShapeType.PolyLineM:
                        content = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());
                        break;
                    case ShapeType.Polygon:
                        content = new PolygonShapeContent(_fixture.Create<Polygon>());
                        break;
                    //case ShapeType.NullShape:
                    default:
                        content = NullShapeContent.Instance;
                        break;
                }

                records[index] = content.RecordAs(number);
                number = number.Next();
            }

            return records;
        }
    }
}
