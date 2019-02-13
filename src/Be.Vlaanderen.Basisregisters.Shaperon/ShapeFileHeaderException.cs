namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class ShapeFileHeaderException : Exception
    {
        public ShapeFileHeaderException(string message) : base(message) { }

        public ShapeFileHeaderException(string message, Exception exception) : base(message, exception) { }
    }
}
