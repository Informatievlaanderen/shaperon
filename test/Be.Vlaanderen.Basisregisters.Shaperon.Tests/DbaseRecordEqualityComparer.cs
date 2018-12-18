namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;
    using System.Linq;

    public class DbaseRecordEqualityComparer : IEqualityComparer<DbaseRecord>
    {
        private readonly IEqualityComparer<DbaseFieldValue> _comparer;

        public DbaseRecordEqualityComparer()
        {
            _comparer = new DbaseFieldValueEqualityComparer();
        }

        public bool Equals(DbaseRecord left, DbaseRecord right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            var sameDeleted = left.IsDeleted.Equals(right.IsDeleted);
            var sameLength = left.Values.Length.Equals(right.Values.Length);
            var sameValues = Enumerable
                .Range(0, left.Values.Length)
                .Aggregate(
                    true,
                    (current, index) => current && _comparer.Equals(left.Values[index], right.Values[index])
                );
            return sameDeleted && sameLength && sameValues;
        }

        public int GetHashCode(DbaseRecord instance)
        {
            return instance.Values.Aggregate(
                instance.IsDeleted.GetHashCode(),
                (current, value) => current ^ _comparer.GetHashCode(value)
            );
        }
    }
}
