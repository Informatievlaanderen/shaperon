namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using GeoAPI.Geometries;

    public class BoundingBox3D
    {
        public static readonly BoundingBox3D Empty = new BoundingBox3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

        public BoundingBox3D(double xMin, double yMin, double xMax, double yMax, double zMin, double zMax, double mMin,
            double mMax)
        {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
            ZMin = zMin;
            ZMax = zMax;
            MMin = mMin;
            MMax = mMax;
        }

        public double XMin { get; }
        public double YMin { get; }
        public double XMax { get; }
        public double YMax { get; }
        public double ZMin { get; }
        public double ZMax { get; }
        public double MMin { get; }
        public double MMax { get; }

        public override bool Equals(object obj) => obj is BoundingBox3D box && Equals(box);

        private bool Equals(BoundingBox3D other) =>
            other != null &&
            ((Double.IsNaN(XMin) && Double.IsNaN(other.XMin)) || Math.Abs(XMin - other.XMin) < Double.Epsilon) &&
            ((Double.IsNaN(YMin) && Double.IsNaN(other.YMin)) || Math.Abs(YMin - other.YMin) < Double.Epsilon) &&
            ((Double.IsNaN(XMax) && Double.IsNaN(other.XMax)) || Math.Abs(XMax - other.XMax) < Double.Epsilon) &&
            ((Double.IsNaN(YMax) && Double.IsNaN(other.YMax)) || Math.Abs(YMax - other.YMax) < Double.Epsilon) &&
            ((Double.IsNaN(ZMin) && Double.IsNaN(other.ZMin)) || Math.Abs(ZMin - other.ZMin) < Double.Epsilon) &&
            ((Double.IsNaN(ZMax) && Double.IsNaN(other.ZMax)) || Math.Abs(ZMax - other.ZMax) < Double.Epsilon) &&
            ((Double.IsNaN(MMin) && Double.IsNaN(other.MMin)) || Math.Abs(MMin - other.MMin) < Double.Epsilon) &&
            ((Double.IsNaN(MMax) && Double.IsNaN(other.MMax)) || Math.Abs(MMax - other.MMax) < Double.Epsilon);

        public override int GetHashCode() =>
            XMin.GetHashCode() ^
            YMin.GetHashCode() ^
            XMax.GetHashCode() ^
            YMax.GetHashCode() ^
            ZMin.GetHashCode() ^
            ZMax.GetHashCode() ^
            MMin.GetHashCode() ^
            MMax.GetHashCode();

        public BoundingBox3D ExpandWith(BoundingBox3D other)
        {
            return new BoundingBox3D(
                Math.Min(XMin, other.XMin),
                Math.Min(YMin, other.YMin),
                Math.Max(XMax, other.XMax),
                Math.Max(YMax, other.YMax),
                Math.Min(ZMin, other.ZMin),
                Math.Max(ZMax, other.ZMax),
                Math.Min(MMin, other.MMin),
                Math.Max(MMax, other.MMax)
            );
        }

        public static BoundingBox3D FromGeometry(IGeometry geometry)
        {
            var z = geometry.GetOrdinates(Ordinate.Z);
            var m = geometry.GetOrdinates(Ordinate.M);
            return new BoundingBox3D(
                geometry.EnvelopeInternal.MinX,
                geometry.EnvelopeInternal.MinY,
                geometry.EnvelopeInternal.MaxX,
                geometry.EnvelopeInternal.MaxY,
                z.DefaultIfEmpty(Double.NaN).Min(),
                z.DefaultIfEmpty(Double.NaN).Max(),
                m.DefaultIfEmpty(Double.NaN).Min(),
                m.DefaultIfEmpty(Double.NaN).Max()
            );
        }
    }
}
