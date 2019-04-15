namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Collections.Generic;

    public class ShapeContentEqualityComparer : IEqualityComparer<ShapeContent>
    {
        public bool Equals(ShapeContent left, ShapeContent right)
        {
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;
            if (left is NullShapeContent && right is NullShapeContent)
                return true;
            if (left is PointShapeContent leftPointContent && right is PointShapeContent rightPointContent)
                return Equals(leftPointContent, rightPointContent);
            if (left is PolyLineMShapeContent leftLineContent && right is PolyLineMShapeContent rightLineContent)
                return Equals(leftLineContent, rightLineContent);
            if (left is PolygonShapeContent leftPolygonContent && right is PolygonShapeContent rightPolygonContent)
                return Equals(leftPolygonContent, rightPolygonContent);
            return false;
        }

        private bool Equals(PointShapeContent left, PointShapeContent right)
        {
            var sameContentLength = left.ContentLength.Equals(right.ContentLength);
            var sameShapeType = left.ShapeType.Equals(right.ShapeType);
            var sameShape = left.Shape.Equals(right.Shape);
            return sameContentLength && sameShapeType && sameShape;
        }

        private bool Equals(PolyLineMShapeContent left, PolyLineMShapeContent right)
        {
            var sameContentLength = left.ContentLength.Equals(right.ContentLength);
            var sameShapeType = left.ShapeType.Equals(right.ShapeType);
            var sameShape = left.Shape.Equals(right.Shape);
            return sameContentLength && sameShapeType && sameShape;
        }

        private bool Equals(PolygonShapeContent left, PolygonShapeContent right)
        {
            var sameContentLength = left.ContentLength.Equals(right.ContentLength);
            var sameShapeType = left.ShapeType.Equals(right.ShapeType);
            var sameShape = left.Shape.EqualsTopologically(right.Shape);
            return sameContentLength && sameShapeType && sameShape;
        }

        public int GetHashCode(ShapeContent instance)
        {
            if (instance is NullShapeContent)
                return 0;
            if (instance is PointShapeContent pointContent)
                return pointContent.ContentLength.GetHashCode() ^ pointContent.ShapeType.GetHashCode() ^
                       pointContent.Shape.GetHashCode();
            if (instance is PolyLineMShapeContent lineContent)
                return lineContent.ContentLength.GetHashCode() ^ lineContent.ShapeType.GetHashCode() ^
                       lineContent.Shape.GetHashCode();
            if (instance is PolygonShapeContent polygonContent)
                return polygonContent.ContentLength.GetHashCode() ^ polygonContent.ShapeType.GetHashCode() ^
                       polygonContent.Shape.GetHashCode();
            return -1;
        }
    }
}
