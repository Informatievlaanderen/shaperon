namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeoAPI.Geometries;

    public class PointSequence : ICoordinateSequence
    {
        private readonly PointM[] _points;

        public PointSequence(IEnumerable<PointM> points)
        {
            _points = points.ToArray();
        }

        public object Clone()
        {
            return Copy();
        }

        public Coordinate GetCoordinate(int index)
        {
            return GetCoordinateCopy(index);
        }

        public Coordinate GetCoordinateCopy(int index)
        {
            return new Coordinate(GetX(index), GetY(index), GetZ(index));
        }

        public void GetCoordinate(int index, Coordinate mutableCoordinate)
        {
            mutableCoordinate.X = GetX(index);
            mutableCoordinate.Y = GetY(index);
            mutableCoordinate.Z = GetZ(index);
        }

        public double GetX(int index)
        {
            return GetOrdinate(index, Ordinate.X);
        }

        public double GetY(int index)
        {
            return GetOrdinate(index, Ordinate.Y);
        }

        public double GetZ(int index)
        {
            return GetOrdinate(index, Ordinate.Z);
        }

        public double GetOrdinate(int index, Ordinate ordinate)
        {
            if (index >= _points.Length)
                return double.NaN;

            switch (ordinate)
            {
                case Ordinate.X:
                    return _points[index].X;
                case Ordinate.Y:
                    return _points[index].Y;
                case Ordinate.Z:
                    return _points[index].Z;
                case Ordinate.M:
                    return _points[index].M;
                default:
                    return double.NaN;
            }
        }

        public void SetOrdinate(int index, Ordinate ordinate, double value)
        {
            if (index >= _points.Length)
                return;

            _points[index] = new PointM(
                ordinate == Ordinate.X ? value : _points[index].X,
                ordinate == Ordinate.Y ? value : _points[index].Y,
                ordinate == Ordinate.Z ? value : _points[index].Z,
                ordinate == Ordinate.M ? value : _points[index].M
            );
        }

        public Coordinate[] ToCoordinateArray()
        {
            return Array.ConvertAll(_points, point => point.Coordinate.Copy());
        }

        public Envelope ExpandEnvelope(Envelope env)
        {
            var envelope = env.Copy();
            foreach (var point in _points)
            {
                envelope.ExpandToInclude(point.Coordinate);
            }

            return envelope;
        }

        public ICoordinateSequence Reversed()
        {
            return new PointSequence(_points.Reverse());
        }

        public ICoordinateSequence Copy()
        {
            return new PointSequence(_points);
        }

        public int Dimension => 4;
        public Ordinates Ordinates => Ordinates.XYZM;
        public int Count => _points.Length;
    }
}
