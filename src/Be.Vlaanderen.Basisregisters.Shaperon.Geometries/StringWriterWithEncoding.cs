namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.IO;
    using System.Text;

    public sealed class StringWriterWithEncoding : StringWriter
    {
        public override Encoding Encoding { get; }

        public StringWriterWithEncoding(Encoding encoding) => Encoding = encoding;
    }
}
