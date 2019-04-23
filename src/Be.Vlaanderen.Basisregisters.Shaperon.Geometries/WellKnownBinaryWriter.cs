namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using GeoAPI.Geometries;
    using GeoAPI.IO;
    using NetTopologySuite.IO;

    public class WellKnownBinaryWriter
    {
        private readonly WKBWriter _wkbWriter;

        public WellKnownBinaryWriter() =>
            _wkbWriter = new WKBWriter(ByteOrder.LittleEndian, true) { HandleOrdinates = Ordinates.XYZM };

        public byte[] Write(IGeometry geometry) => _wkbWriter.Write(geometry);
    }
}
