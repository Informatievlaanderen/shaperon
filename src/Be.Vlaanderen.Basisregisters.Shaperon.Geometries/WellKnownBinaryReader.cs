namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
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

        public Geometry Read(byte[] data)
        {
            return _wkbReader.Read(data);
        }

        public TGeometry ReadAs<TGeometry>(byte[] value)
            where TGeometry : Geometry => (TGeometry) Read(value);

        public bool TryReadAs<TGeometry>(byte[] value, out TGeometry? geometry)
            where TGeometry : Geometry
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
            where TGeometry : Geometry => Read(value) is TGeometry;
    }
}
