namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;
    using Xunit.Abstractions;

    public partial class NullShapeContentTests
    {
        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = NullShapeContent.Instance;

            var result = sut.Anonymous();

            Assert.Same(sut, result);
        }

        [Fact]
        public void ReadAnonymousCanReadWriteNullShape()
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
                    var result = (NullShapeContent) ShapeContent.ReadAnonymous(reader);

                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ToBytesHasExpectedResult()
        {
            var sut = NullShapeContent.Instance;

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
            var content = NullShapeContent.Instance;

            var result = ShapeContent.FromBytes(content.ToBytes());

            Assert.Same(content, result);
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = NullShapeContent.Instance;

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
            var content = NullShapeContent.Instance;

            var result = ShapeContent.FromBytes(content.ToBytes(Encoding.UTF8), Encoding.UTF8);

            Assert.Same(content, result);
        }
    }

    public partial class PolyLineMShapeContentTests
    {
        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());

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
        public void ReadAnonymousCanReadWrittenPolyLineMShape()
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
                    var result = (AnonymousShapeContent) ShapeContent.ReadAnonymous(reader);

                    Assert.Equal(stream.ToArray(), result.Content);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ToBytesHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());

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
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());

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
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());

            var result = ShapeContent.FromBytes(sut.ToBytes(Encoding.UTF8), Encoding.UTF8);

            var actual = Assert.IsType<PolyLineMShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void FromBytesHasExpectedResult()
        {
            var sut = new PolyLineMShapeContent(_fixture.Create<PolyLineM>());

            var result = ShapeContent.FromBytes(sut.ToBytes());

            var actual = Assert.IsType<PolyLineMShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }
    }

    public partial class PolygonShapeContentTests
    {
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
        public void ReadAnonymousCanReadWrittenPolygonShape()
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
    }

    public partial class PointShapeContentTests
    {

        [Fact]
        public void AnonymousHasExpectedResult()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());

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
        public void ReadAnonymousCanReadWrittenPointShape()
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
                    var result = (AnonymousShapeContent) ShapeContent.ReadAnonymous(reader);

                    Assert.Equal(stream.ToArray(), result.Content);
                    Assert.Equal(sut.ShapeType, result.ShapeType);
                    Assert.Equal(sut.ContentLength, result.ContentLength);
                }
            }
        }

        [Fact]
        public void ToBytesHasExpectedResult()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());

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
            var sut = new PointShapeContent(_fixture.Create<Point>());

            var result = ShapeContent.FromBytes(sut.ToBytes());

            var actual = Assert.IsType<PointShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }

        [Fact]
        public void ToBytesWithEncodingHasExpectedResult()
        {
            var sut = new PointShapeContent(_fixture.Create<Point>());

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
            var sut = new PointShapeContent(_fixture.Create<Point>());

            var result = ShapeContent.FromBytes(sut.ToBytes(Encoding.UTF8), Encoding.UTF8);

            var actual = Assert.IsType<PointShapeContent>(result);
            Assert.Equal(sut.Shape, actual.Shape);
            Assert.Equal(sut.ShapeType, actual.ShapeType);
            Assert.Equal(sut.ContentLength, actual.ContentLength);
        }
    }

    public class AnonymousShapeContentTests
    {
        private readonly Fixture _fixture;

        public AnonymousShapeContentTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizePoint();
            _fixture.CustomizePolygon();
            _fixture.CustomizePolyLineM();
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

        public class DbaseInt32Tests
    {
        private readonly Fixture _fixture;

        private ITestOutputHelper _out;

        public DbaseInt32Tests(ITestOutputHelper @out)
        {
            _out = @out ?? throw new ArgumentNullException(nameof(@out));
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseInt32();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseInt32(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotNumberOrFloat()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Number && specimen != DbaseFieldType.Float);
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() > 0);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseInt32(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            length,
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void CreateFailsIfFieldDecimalCountIsNot0()
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen.ToInt32() > 1);
            var decimalCount = new Generator<DbaseDecimalCount>(_fixture)
                .First(specimen => specimen.ToInt32() != 0 && specimen.ToInt32() < length.ToInt32());
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseInt32(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            _fixture.GenerateDbaseInt32FieldType(),
                            _fixture.Create<ByteOffset>(),
                            length,
                            decimalCount
                        )
                    )
            );
        }

        [Fact]
        public void LengthOfValueBeingSetCanNotExceedFieldLength()
        {
            var maxLength = new DbaseFieldLength(
                int.MaxValue.ToString(CultureInfo.InvariantCulture).Length - 1
                // because it's impossible to create a value longer than this (we need the test to generate a longer value)
            );
            var length = _fixture.GenerateDbaseInt32LengthLessThan(maxLength);
            _out.WriteLine("Length used is: {0}", length.ToString());

            var sut =
                new DbaseInt32(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        _fixture.GenerateDbaseInt32FieldType(),
                        _fixture.Create<ByteOffset>(),
                        length,
                        new DbaseDecimalCount(0)
                    )
                );

            var value = Enumerable
                .Range(0, sut.Field.Length.ToInt32())
                .Aggregate(1, (current, _) => current * 10);
            _out.WriteLine("Value used is: {0}", value.ToString());

            Assert.Throws<ArgumentException>(() => sut.Value = value);
        }

        [Fact]
        public void LengthOfNegativeValueBeingSetCanNotExceedFieldLength()
        {
            var maxLength = new DbaseFieldLength(
                int.MinValue.ToString(CultureInfo.InvariantCulture).Length - 1
                // because it's impossible to create a value longer than this (we need the test to generate a longer value)
            );
            var length = _fixture.GenerateDbaseInt32LengthLessThan(maxLength);

            var sut =
                new DbaseInt32(
                    new DbaseField(
                        _fixture.Create<DbaseFieldName>(),
                        _fixture.GenerateDbaseInt32FieldType(),
                        _fixture.Create<ByteOffset>(),
                        length,
                        new DbaseDecimalCount(0)
                    )
                );

            var value = Enumerable
                .Range(0, sut.Field.Length.ToInt32())
                .Aggregate(-1, (current, _) => current * 10);

            Assert.Throws<ArgumentException>(() => sut.Value = value);
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseInt32>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseInt32>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseInt32>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseInt32>();
            sut.Value = null;

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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteNegative()
        {
            var value = Math.Abs(_fixture.Create<int>()) * -1;
            var sut = new DbaseInt32(
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    _fixture.GenerateDbaseInt32FieldType(),
                    ByteOffset.Initial,
                    new DbaseFieldLength(value.ToString(CultureInfo.InvariantCulture).Length),
                    new DbaseDecimalCount(0)
                ),
                value
            );

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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseInt32>();

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
                    var result = new DbaseInt32(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseInt32>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, sut.Field.Length.ToInt32())).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseInt32(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }
    }

    public class DbaseDateTimeTests
    {
        private readonly Fixture _fixture;

        public DbaseDateTimeTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseDateTime();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseDateTime(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotDateTimeOrCharacter()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.DateTime && specimen != DbaseFieldType.Character);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseDateTime(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            new DbaseFieldLength(15),
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseDateTime>());
        }

        [Fact]
        public void DateTimeMillisecondsAreRemovedUponConstruction()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.DateTime,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(15),
                new DbaseDecimalCount(0));
            var sut = new DbaseDateTime(field, new DateTime(1, 1, 1, 1, 1, 1, 1));

            Assert.Equal(new DateTime(1, 1, 1, 1, 1, 1, 0), sut.Value);
        }

        [Fact]
        public void DateTimeMillisecondsAreRemovedUponSet()
        {
            var field = new DbaseField(
                _fixture.Create<DbaseFieldName>(),
                DbaseFieldType.DateTime,
                _fixture.Create<ByteOffset>(),
                new DbaseFieldLength(15),
                new DbaseDecimalCount(0));
            var sut = new DbaseDateTime(field);

            sut.Value = new DateTime(1, 1, 1, 1, 1, 1, 1);

            Assert.Equal(new DateTime(1, 1, 1, 1, 1, 1, 0), sut.Value);
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDateTime>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseDateTime>().Select(instance => instance.Write(null)));
        }

        [Fact]
        public void CanReadWriteNull()
        {
            var sut = _fixture.Create<DbaseDateTime>();
            sut.Value = null;

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
                    var result = new DbaseDateTime(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseDateTime>();

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
                    var result = new DbaseDateTime(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseDateTime>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, 14)).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseDateTime(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }
    }
}
