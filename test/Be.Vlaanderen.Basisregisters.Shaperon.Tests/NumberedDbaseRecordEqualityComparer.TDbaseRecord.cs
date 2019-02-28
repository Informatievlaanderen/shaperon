namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public class NumberedDbaseRecordEqualityComparer<TDbaseRecord> : IEqualityComparer<NumberedDbaseRecord<TDbaseRecord>> where TDbaseRecord: DbaseRecord, new()
    {
        private readonly IEqualityComparer<DbaseRecord> _comparer;

        public NumberedDbaseRecordEqualityComparer()
        {
            _comparer = new DbaseRecordEqualityComparer();
        }

        public bool Equals(NumberedDbaseRecord<TDbaseRecord> left, NumberedDbaseRecord<TDbaseRecord> right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            return left.Number.Equals(right.Number) && _comparer.Equals(left.Record, right.Record);
        }

        public int GetHashCode(NumberedDbaseRecord<TDbaseRecord> instance)
        {
            return instance.Number.GetHashCode() ^ _comparer.GetHashCode(instance.Record);
        }
    }
}