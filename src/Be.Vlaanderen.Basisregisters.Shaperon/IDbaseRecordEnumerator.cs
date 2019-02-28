namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public interface IDbaseRecordEnumerator : IEnumerator<DbaseRecord>
    {
        RecordNumber CurrentRecordNumber { get; }
    }
}