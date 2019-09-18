namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.Linq;
    using AutoFixture;
    using NetTopologySuite.Geometries;
    using Xunit;

    public class WkbReaderWriterAssumptions
    {
        private readonly Fixture _fixture;

        public WkbReaderWriterAssumptions()
        {
            _fixture = new Fixture();
            _fixture.Customize<CoordinateZM>(customization =>
                customization
                    .FromFactory<int>(value =>
                        new CoordinateZM(
                            _fixture.Create<double>(),
                            _fixture.Create<double>(),
                            _fixture.Create<double>(),
                            _fixture.Create<double>()
                        )
                    )
                    .OmitAutoProperties());
            _fixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        new CoordinateZM(
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>()
                        )
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<LineString>(customization =>
                customization.FromFactory(generator =>
                    new LineString(
                        GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(
                            _fixture
                                .CreateMany<CoordinateZM>(generator.Next(2, 10))
                                .Cast<Coordinate>()
                                .ToArray()
                            ),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<MultiLineString>(customization =>
                customization.FromFactory(generator =>
                    new MultiLineString(
                        _fixture.CreateMany<LineString>(generator.Next(0, 100)).ToArray(),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties());
        }

        [Fact]
        public void PointAsExtendedWellKnownBinaryCanBeWrittenAndRead()
        {
            var sut = _fixture.Create<Point>();

            var writer = new WellKnownBinaryWriter();

            var extendedWellKnownBinary = writer.Write(sut);

            var reader = new WellKnownBinaryReader();

            var result = reader.Read(extendedWellKnownBinary);

            Assert.NotNull(result);
            Assert.Equal(sut, Assert.IsType<Point>(result));
            Assert.Equal(sut.SRID, ((Point) result).SRID);
            Assert.Equal(sut.M, ((Point) result).M);
        }

        [Fact]
        public void MultiLineStringAsExtendedWellKnownBinaryCanBeWrittenAndRead()
        {
            var sut = _fixture.Create<MultiLineString>();

            var writer = new WellKnownBinaryWriter();

            var extendedWellKnownBinary = writer.Write(sut);

            var reader = new WellKnownBinaryReader();

            var result = reader.Read(extendedWellKnownBinary);

            Assert.NotNull(result);
            Assert.Equal(sut, Assert.IsType<MultiLineString>(result));
            Assert.Equal(sut.SRID, ((MultiLineString) result).SRID);
        }
    }
}
