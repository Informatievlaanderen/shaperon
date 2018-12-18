namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using GeoAPI.Geometries;
    using Xunit;

    public class GeometryConfigurationTests
    {
        [Fact]
        public void ConfiguredGeometryFactoryHasFloatingPrecisionModel()
        {
            var factoryPrecisionModel = GeometryConfiguration.GeometryFactory.PrecisionModel;
            Assert.Equal(PrecisionModels.Floating, factoryPrecisionModel.PrecisionModelType);
        }

        [Fact]
        public void ConfiguredGeometryFactoryHasOrdinatesXyzm()
        {
            var coordinateSequenceFactory = GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory;
            Assert.Equal(Ordinates.XYZM, coordinateSequenceFactory.Ordinates);
        }

        [Fact]
        public void ConfiguredGeometryFactoryUsesBelgeLambert1972AsSpatialReferenceSystemIdentifier()
        {
            Assert.Equal(SpatialReferenceSystemIdentifier.BelgeLambert1972.ToInt32(),
                GeometryConfiguration.GeometryFactory.SRID);
        }
    }
}
