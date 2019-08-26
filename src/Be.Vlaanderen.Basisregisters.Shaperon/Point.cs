namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public readonly struct Point : IEquatable<Point>
    {
        public Point(double x, double y)
        {
            if(double.IsNaN(x))
                throw new ArgumentException("X can not be NaN.", nameof(x));

            if(double.IsNegativeInfinity(x))
                throw new ArgumentException("X can not be negative infinity.", nameof(x));

            if(double.IsPositiveInfinity(x))
                throw new ArgumentException("X can not be positive infinity.", nameof(x));

            if(double.IsNaN(y))
                throw new ArgumentException("Y can not be NaN.", nameof(y));

            if(double.IsNegativeInfinity(y))
                throw new ArgumentException("Y can not be negative infinity.", nameof(y));

            if(double.IsPositiveInfinity(y))
                throw new ArgumentException("Y can not be positive infinity.", nameof(y));

            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        [Pure]
        public bool Equals(Point other, Tolerance tolerance) =>
            Math.Abs(X - other.X) < tolerance.ToDouble() && Math.Abs(Y - other.Y) < tolerance.ToDouble();
        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Point other && Equals(other);
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
        public override string ToString() => "{X=" + X.ToString(CultureInfo.InvariantCulture) + ",Y=" + Y.ToString(CultureInfo.InvariantCulture) + "}";
    }
}
