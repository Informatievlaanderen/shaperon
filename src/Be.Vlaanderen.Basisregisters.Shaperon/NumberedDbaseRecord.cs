namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class NumberedDbaseRecord
    {
        public NumberedDbaseRecord(RecordNumber number, DbaseRecord record)
        {
            Number = number;
            Record = record ?? throw new ArgumentNullException(nameof(record));
        }
        
        public RecordNumber Number { get; }
        public DbaseRecord Record { get; }
    }
}