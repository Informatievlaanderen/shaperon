namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using GeoAPI.Geometries;
    using NetTopologySuite;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;

    public class WellKnownBinaryReader
    {
        private readonly WKBReader _wkbReader;

        public WellKnownBinaryReader() =>
            _wkbReader = new WKBReader(new NtsGeometryServices(
                GeometryConfiguration.GeometryFactory.CoordinateSequenceFactory,
                GeometryConfiguration.GeometryFactory.PrecisionModel,
                GeometryConfiguration.GeometryFactory.SRID));

        public IGeometry Read(byte[] data)
        {
            var geometry = _wkbReader.Read(data);
            if (geometry is Point point)
                return new PointM(point.X, point.Y, point.Z, point.M);

            return geometry;
        }

        public TGeometry ReadAs<TGeometry>(byte[] value)
            where TGeometry : IGeometry => (TGeometry) Read(value);

        public bool TryReadAs<TGeometry>(byte[] value, out TGeometry geometry)
            where TGeometry : IGeometry
        {
            var parsed = Read(value);
            if (parsed is TGeometry parsedGeometry)
            {
                geometry = parsedGeometry;
                return true;
            }

            geometry = default;
            return false;
        }

        public bool CanBeReadAs<TGeometry>(byte[] value)
            where TGeometry : IGeometry => Read(value) is TGeometry;
    }
}
