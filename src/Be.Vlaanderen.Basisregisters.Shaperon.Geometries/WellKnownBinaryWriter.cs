namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;

    public class WellKnownBinaryWriter
    {
        private readonly WKBWriter _wkbWriter;

        public WellKnownBinaryWriter() =>
            _wkbWriter = new WKBWriter(ByteOrder.LittleEndian, true) { HandleOrdinates = Ordinates.XYZM };

        public byte[] Write(Geometry geometry) => _wkbWriter.Write(geometry);
    }
}
