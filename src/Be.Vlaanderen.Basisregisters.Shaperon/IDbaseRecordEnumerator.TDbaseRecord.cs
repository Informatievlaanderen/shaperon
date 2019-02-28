namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public interface IDbaseRecordEnumerator<TDbaseRecord> : IEnumerator<TDbaseRecord> where TDbaseRecord : DbaseRecord
    {
        RecordNumber CurrentRecordNumber { get; }
    }
}