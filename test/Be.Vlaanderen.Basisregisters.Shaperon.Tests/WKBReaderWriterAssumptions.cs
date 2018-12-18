namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using AutoFixture;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using Xunit;

    public class WkbReaderWriterAssumptions
    {
        private readonly Fixture _fixture;

        public WkbReaderWriterAssumptions()
        {
            _fixture = new Fixture();
            _fixture.Customize<Coordinate>(customization =>
                customization
                    .FromFactory<int>(value =>
                        new Coordinate(
                            _fixture.Create<double>(),
                            _fixture.Create<double>(),
                            _fixture.Create<double>()
                        )
                    )
                    .OmitAutoProperties());
            _fixture.Customize<PointM>(customization =>
                customization.FromFactory(generator =>
                    new PointM(
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>(),
                        _fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<ILineString>(customization =>
                customization.FromFactory(generator =>
                    new LineString(
                        new PointSequence(_fixture.CreateMany<PointM>(generator.Next(2, 10))),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties()
            );
            _fixture.Customize<MultiLineString>(customization =>
                customization.FromFactory(generator =>
                    new MultiLineString(
                        _fixture.CreateMany<ILineString>(generator.Next(0, 100)).ToArray(),
                        GeometryConfiguration.GeometryFactory
                    )
                ).OmitAutoProperties());
        }

        [Fact]
        public void PointAsExtendedWellKnownBinaryCanBeWrittenAndRead()
        {
            var sut = _fixture.Create<PointM>();

            var writer = new WellKnownBinaryWriter();

            var extendedWellKnownBinary = writer.Write(sut);

            var reader = new WellKnownBinaryReader();

            var result = reader.Read(extendedWellKnownBinary);

            Assert.NotNull(result);
            Assert.Equal(sut, Assert.IsType<PointM>(result));
            Assert.Equal(sut.SRID, ((PointM) result).SRID);
            Assert.Equal(sut.M, ((PointM) result).M);
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
