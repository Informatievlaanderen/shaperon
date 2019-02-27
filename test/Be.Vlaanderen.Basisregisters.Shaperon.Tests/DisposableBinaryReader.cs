namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.IO;
    using System.Text;

    public class DisposableBinaryReader : BinaryReader
    {
        public DisposableBinaryReader(Stream input) : base(input)
        {
        }

        public DisposableBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public DisposableBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }

        public bool Disposed { get; private set; }
    }
}