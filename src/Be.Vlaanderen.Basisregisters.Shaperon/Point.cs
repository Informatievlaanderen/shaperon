namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;

    public readonly struct Point : IEquatable<Point>
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(Point other, double tolerance) =>
            Math.Abs(X - other.X) < tolerance && Math.Abs(Y - other.Y) < tolerance;
        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Point other && Equals(other);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
        public override string ToString() => "{X=" + X.ToString(CultureInfo.InvariantCulture) + ",Y=" + Y.ToString(CultureInfo.InvariantCulture) + "}";
    }
}
