namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class DbaseFileHeaderException : Exception
    {
        public DbaseFileHeaderException(string message) : base(message) { }

        public DbaseFileHeaderException(string message, Exception exception) : base(message, exception) { }
    }
}
