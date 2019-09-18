namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Implementation;

    public static class GeometryConfiguration
    {
        public static readonly GeometryFactory GeometryFactory = new GeometryFactory(
            new PrecisionModel(PrecisionModels.Floating),
            SpatialReferenceSystemIdentifier.BelgeLambert1972.ToInt32(),
            new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYZM));
    }
}
