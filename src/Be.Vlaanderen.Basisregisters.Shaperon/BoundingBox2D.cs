namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class BoundingBox2D
    {
        public static readonly BoundingBox2D Empty = new BoundingBox2D(0.0, 0.0, 0.0, 0.0);

        public BoundingBox2D(
            double xMin,
            double yMin,
            double xMax,
            double yMax)
        {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
        }

        public double XMin { get; }
        public double YMin { get; }
        public double XMax { get; }
        public double YMax { get; }

        public override bool Equals(object obj) => obj is BoundingBox2D box && Equals(box);

        public bool Equals(BoundingBox2D other) =>
            other != null &&
            (double.IsNaN(XMin) && double.IsNaN(other.XMin) || XMin.Equals(other.XMin)) &&
            (double.IsNaN(YMin) && double.IsNaN(other.YMin) || YMin.Equals(other.YMin)) &&
            (double.IsNaN(XMax) && double.IsNaN(other.XMax) || XMax.Equals(other.XMax)) &&
            (double.IsNaN(YMax) && double.IsNaN(other.YMax) || YMax.Equals(other.YMax));

        public bool Equals(BoundingBox2D other, double tolerance) =>
            other != null &&
            (double.IsNaN(XMin) && double.IsNaN(other.XMin) || Math.Abs(XMin - other.XMin) < tolerance) &&
            (double.IsNaN(YMin) && double.IsNaN(other.YMin) || Math.Abs(YMin - other.YMin) < tolerance) &&
            (double.IsNaN(XMax) && double.IsNaN(other.XMax) || Math.Abs(XMax - other.XMax) < tolerance) &&
            (double.IsNaN(YMax) && double.IsNaN(other.YMax) || Math.Abs(YMax - other.YMax) < tolerance);

        public override int GetHashCode() =>
            XMin.GetHashCode() ^
            YMin.GetHashCode() ^
            XMax.GetHashCode() ^
            YMax.GetHashCode();

        public BoundingBox2D ExpandWith(BoundingBox2D other)
        {
            return new BoundingBox2D(
                Math.Min(XMin, other.XMin),
                Math.Min(YMin, other.YMin),
                Math.Max(XMax, other.XMax),
                Math.Max(YMax, other.YMax));
        }
    }
}
