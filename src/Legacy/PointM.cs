namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries;

    // Default point does not support Ordinate.M

    // Point uses a ICoordinateSequence as underlying type
    // The default ICoordinateSequenceFactory from NetTopologySuite used to create the underlaying ICoordinateSequence is configured for Ordinate.XYZ

    public class PointM : Point
    {
        public PointM(Coordinate coordinate)
            : base(
                GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory.Create(new[] {coordinate}),
                GeometryConfiguration.GeometryFactory) { }

        public PointM(double x, double y, double z, double m)
            : this(new Coordinate(x, y, z)) => ChangeMeasurement(m);

        public PointM(double x, double y, double z)
            : this(new Coordinate(x, y, z)) { }

        public PointM(double x, double y)
            : this(new Coordinate(x, y)) { }

        public PointM(ICoordinateSequence coordinatesSequence)
            : this(coordinatesSequence.GetCoordinate(0)) { }

        // Values cannot be modified, so let's remove the Setters
        public new double X => base.X;
        public new double Y => base.Y;
        public new double Z => base.Z;
        public new double M => base.M;

        public void ChangeMeasurement(double m)
            => base.M = m;
    }
}
