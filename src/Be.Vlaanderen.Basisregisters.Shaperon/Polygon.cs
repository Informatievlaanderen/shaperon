namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class Polygon : IEquatable<Polygon>
    {
        public Polygon(
            BoundingBox2D boundingBox,
            int[] parts,
            Point[] points)
        {
            BoundingBox = boundingBox ?? throw new ArgumentNullException(nameof(boundingBox));
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            Points = points ?? throw new ArgumentNullException(nameof(points));
            NumberOfParts = parts.Length;
            NumberOfPoints = points.Length;
        }

        public BoundingBox2D BoundingBox { get; }
        public int NumberOfParts { get; }
        public int NumberOfPoints { get; }
        public int[] Parts { get; }
        public Point[] Points { get; }

        [Pure]
        public bool Equals(Polygon other, Tolerance tolerance) => other != null
                                                               && BoundingBox.Equals(other.BoundingBox, tolerance)
                                                               && NumberOfParts.Equals(other.NumberOfParts)
                                                               && NumberOfPoints.Equals(other.NumberOfPoints)
                                                               && Parts.SequenceEqual(other.Parts)
                                                               && Points.SequenceEqual(other.Points, new TolerantPointEqualityComparer(tolerance));
        public bool Equals(Polygon other) => other != null
                                             && BoundingBox.Equals(other.BoundingBox)
                                             && NumberOfParts.Equals(other.NumberOfParts)
                                             && NumberOfPoints.Equals(other.NumberOfPoints)
                                             && Parts.SequenceEqual(other.Parts)
                                             && Points.SequenceEqual(other.Points);

        public override bool Equals(object obj) => obj is Polygon other && Equals(other);

        public override int GetHashCode()
        {
            var partsHashCode = Parts.Aggregate(0, (current, part) => current ^ part.GetHashCode());
            var pointsHashCode = Points.Aggregate(0, (current, point) => current ^ point.GetHashCode());
            return BoundingBox.GetHashCode() ^ NumberOfParts.GetHashCode() ^ NumberOfPoints.GetHashCode()
                   ^ partsHashCode ^ pointsHashCode;
        }

        private class TolerantPointEqualityComparer : IEqualityComparer<Point>
        {
            private readonly Tolerance _tolerance;

            public TolerantPointEqualityComparer(Tolerance tolerance)
            {
                _tolerance = tolerance;
            }

            public bool Equals(Point x, Point y)
            {
                return x.Equals(y, _tolerance);
            }

            public int GetHashCode(Point obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
