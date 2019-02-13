namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class DbaseRecordException : Exception
    {
        public DbaseRecordException(string message) : base(message)
        {
        }
    }
}