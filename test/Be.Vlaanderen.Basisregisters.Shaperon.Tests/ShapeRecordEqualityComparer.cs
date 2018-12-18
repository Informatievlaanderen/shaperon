namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;
    using System.Linq;

    public class ShapeRecordEqualityComparer : IEqualityComparer<ShapeRecord>
    {
        private readonly IEqualityComparer<ShapeContent> _comparer;

        public ShapeRecordEqualityComparer()
        {
            _comparer = new ShapeContentEqualityComparer();
        }

        public bool Equals(ShapeRecord left, ShapeRecord right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            var sameHeader = left.Header.Equals(right.Header);
            var sameLength = left.Length.Equals(right.Length);
            var sameContent = _comparer.Equals(left.Content, right.Content);
            return sameHeader && sameLength && sameContent;
        }

        public int GetHashCode(ShapeRecord instance)
        {
            return instance.Header.GetHashCode()
                   ^ instance.Length.GetHashCode()
                   ^ _comparer.GetHashCode(instance.Content);
        }
    }
}
