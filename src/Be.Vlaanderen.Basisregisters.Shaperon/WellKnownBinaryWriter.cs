namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using GeoAPI.Geometries;
    using GeoAPI.IO;
    using NetTopologySuite.IO;

    public class WellKnownBinaryWriter
    {
        private readonly WKBWriter _wkbWriter;

        public WellKnownBinaryWriter()
        {
            _wkbWriter = new WKBWriter(ByteOrder.LittleEndian, true)
            {
                HandleOrdinates = Ordinates.XYZM
            };
        }

        public byte[] Write(IGeometry geometry)
        {
            return _wkbWriter.Write(geometry);
        }
    }
}
