namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class NumberedDbaseRecord<TDbaseRecord> where TDbaseRecord : DbaseRecord
    {
        public NumberedDbaseRecord(RecordNumber number, TDbaseRecord record)
        {
            Number = number;
            Record = record ?? throw new ArgumentNullException(nameof(record));
        }
        
        public RecordNumber Number { get; }
        public TDbaseRecord Record { get; }
    }
}