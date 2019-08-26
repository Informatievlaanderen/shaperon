namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class PolyLineM : IEquatable<PolyLineM>
    {
        public PolyLineM(
            BoundingBox2D boundingBox,
            int[] parts,
            Point[] points,
            double[] measures)
        {
            BoundingBox = boundingBox ?? throw new ArgumentNullException(nameof(boundingBox));
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            Points = points ?? throw new ArgumentNullException(nameof(points));
            Measures = measures ?? throw new ArgumentNullException(nameof(measures));
            NumberOfParts = parts.Length;
            NumberOfPoints = points.Length;
            MeasureRange = MeasureRange.FromMeasures(measures);
        }

        public BoundingBox2D BoundingBox { get; }
        public int NumberOfParts { get; }
        public int NumberOfPoints { get; }
        public int[] Parts { get; }
        public Point[] Points { get; }
        public MeasureRange MeasureRange { get; }
        public double[] Measures { get; }

        [Pure]
        public bool Equals(PolyLineM other, Tolerance tolerance) => other != null
                                               && BoundingBox.Equals(other.BoundingBox, tolerance)
                                               && NumberOfParts.Equals(other.NumberOfParts)
                                               && NumberOfPoints.Equals(other.NumberOfPoints)
                                               && Parts.SequenceEqual(other.Parts)
                                               && Points.SequenceEqual(other.Points, new TolerantPointEqualityComparer(tolerance))
                                               && MeasureRange.Equals(other.MeasureRange, tolerance)
                                               && Measures.SequenceEqual(other.Measures, new TolerantDoubleEqualityComparer(tolerance));
        public bool Equals(PolyLineM other) => other != null
                                               && BoundingBox.Equals(other.BoundingBox)
                                               && NumberOfParts.Equals(other.NumberOfParts)
                                               && NumberOfPoints.Equals(other.NumberOfPoints)
                                               && Parts.SequenceEqual(other.Parts)
                                               && Points.SequenceEqual(other.Points)
                                               && MeasureRange.Equals(other.MeasureRange)
                                               && Measures.SequenceEqual(other.Measures);

        public override bool Equals(object obj) => obj is PolyLineM other && Equals(other);

        public override int GetHashCode()
        {
            var partsHashCode = Parts.Aggregate(0, (current, part) => current ^ part.GetHashCode());
            var pointsHashCode = Points.Aggregate(0, (current, point) => current ^ point.GetHashCode());
            var measuresHashCode = Measures.Aggregate(0, (current, measure) => current ^ measure.GetHashCode());
            return BoundingBox.GetHashCode() ^ NumberOfParts.GetHashCode() ^ NumberOfPoints.GetHashCode()
                   ^ partsHashCode ^ pointsHashCode ^ MeasureRange.GetHashCode() ^ measuresHashCode;
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

        private class TolerantDoubleEqualityComparer : IEqualityComparer<double>
        {
            private readonly double _tolerance;

            public TolerantDoubleEqualityComparer(Tolerance tolerance)
            {
                _tolerance = tolerance.ToDouble();
            }

            public bool Equals(double x, double y)
            {
                return double.IsNaN(x) && double.IsNaN(y)
                       || double.IsNegativeInfinity(x) && double.IsNegativeInfinity(y)
                       || double.IsPositiveInfinity(x) && double.IsPositiveInfinity(y)
                       || Math.Abs(x - y) < _tolerance;
            }

            public int GetHashCode(double obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
