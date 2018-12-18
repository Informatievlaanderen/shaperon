using System;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class DbaseFileHeaderException : Exception
    {
        public DbaseFileHeaderException(string message) : base(message)
        {
        }
    }
}
