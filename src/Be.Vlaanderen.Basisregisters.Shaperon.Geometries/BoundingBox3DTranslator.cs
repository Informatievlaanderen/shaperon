namespace Be.Vlaanderen.Basisregisters.Shaperon.Geometries
{
    using System.Linq;
    using NetTopologySuite.Geometries;

    public static class BoundingBox3DTranslator
    {
        public static BoundingBox3D FromGeometry(Geometry geometry)
        {
            var z = geometry.GetOrdinates(Ordinate.Z);
            var m = geometry.GetOrdinates(Ordinate.M);

            return new BoundingBox3D(
                geometry.EnvelopeInternal.MinX,
                geometry.EnvelopeInternal.MinY,
                geometry.EnvelopeInternal.MaxX,
                geometry.EnvelopeInternal.MaxY,
                z.DefaultIfEmpty(double.NaN).Min(),
                z.DefaultIfEmpty(double.NaN).Max(),
                m.DefaultIfEmpty(double.NaN).Min(),
                m.DefaultIfEmpty(double.NaN).Max());
        }
    }
}

