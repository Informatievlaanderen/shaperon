namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;

    public static class GeometryConfiguration
    {
        public static readonly IGeometryFactory GeometryFactory = new GeometryFactory(
            new PrecisionModel(PrecisionModels.Floating),
            SpatialReferenceSystemIdentifier.BelgeLambert1972.ToInt32(),
            new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYZM));
    }
}
