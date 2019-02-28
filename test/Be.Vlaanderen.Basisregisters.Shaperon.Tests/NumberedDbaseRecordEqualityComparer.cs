namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public class NumberedDbaseRecordEqualityComparer : IEqualityComparer<NumberedDbaseRecord>
    {
        private readonly IEqualityComparer<DbaseRecord> _comparer;

        public NumberedDbaseRecordEqualityComparer()
        {
            _comparer = new DbaseRecordEqualityComparer();
        }

        public bool Equals(NumberedDbaseRecord left, NumberedDbaseRecord right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            return left.Number.Equals(right.Number) && _comparer.Equals(left.Record, right.Record);
        }

        public int GetHashCode(NumberedDbaseRecord instance)
        {
            return instance.Number.GetHashCode() ^ _comparer.GetHashCode(instance.Record);
        }
    }
}