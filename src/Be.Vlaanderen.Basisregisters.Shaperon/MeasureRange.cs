namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    public readonly struct MeasureRange : IEquatable<MeasureRange>
    {
        public static readonly MeasureRange Empty = new MeasureRange(double.PositiveInfinity, double.NegativeInfinity);

        public static MeasureRange FromMeasures(double[] measures)
        {
            if (measures == null) throw new ArgumentNullException(nameof(measures));
            var acceptable = measures.Where(measure => !double.IsNaN(measure)).ToArray();
            return acceptable.Length == 0 ? Empty : new MeasureRange(acceptable.Min(), acceptable.Max());
        }

        public MeasureRange(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double Min { get; }

        public double Max { get; }

        [Pure]
        public bool IsEmpty => double.IsPositiveInfinity(Min) && double.IsNegativeInfinity(Max);

        [Pure]
        public bool Equals(MeasureRange other, double tolerance)
        {
            if (IsEmpty && other.IsEmpty) return true;
            if (IsEmpty || other.IsEmpty) return false;
            return Math.Abs(Min - other.Min) < tolerance && Math.Abs(Max - other.Max) < tolerance;
        }

        public bool Equals(MeasureRange other)
        {
            if (IsEmpty && other.IsEmpty) return true;
            if (IsEmpty || other.IsEmpty) return false;
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object obj) => obj is MeasureRange other && Equals(other);
        public override int GetHashCode() => IsEmpty ? 0 : Min.GetHashCode() ^ Max.GetHashCode();

        public override string ToString() => IsEmpty
            ? "{Empty}"
            : "{Min=" + Min.ToString(CultureInfo.InvariantCulture) + ",Max=" +
              Max.ToString(CultureInfo.InvariantCulture) + "}";
    }
}
