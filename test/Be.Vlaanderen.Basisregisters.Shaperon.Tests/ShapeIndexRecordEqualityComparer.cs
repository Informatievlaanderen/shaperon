namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public class ShapeIndexRecordEqualityComparer : IEqualityComparer<ShapeIndexRecord>
    {
        public bool Equals(ShapeIndexRecord left, ShapeIndexRecord right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            var sameOffset = left.Offset.Equals(right.Offset);
            var sameLength = left.ContentLength.Equals(right.ContentLength);
            return sameOffset && sameLength;
        }

        public int GetHashCode(ShapeIndexRecord instance)
        {
            return instance.Offset.GetHashCode()
                   ^ instance.ContentLength.GetHashCode();
        }
    }
}
