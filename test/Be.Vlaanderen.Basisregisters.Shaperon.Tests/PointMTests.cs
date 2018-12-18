namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using GeoAPI.Geometries;
    using NetTopologySuite.Geometries.Implementation;
    using Xunit;

    public class WhenCreatingAPointFromAnCoordinateSequence
    {
        private readonly PointM _sut;
        private readonly Fixture _fixture;
        private readonly ICoordinateSequence _xySequence;

        public WhenCreatingAPointFromAnCoordinateSequence()
        {
            _fixture = new Fixture();

            _xySequence = new DotSpatialAffineCoordinateSequence(1, Ordinates.XYZ);
            _xySequence.SetOrdinate(0, Ordinate.X, _fixture.Create<double>());
            _xySequence.SetOrdinate(0, Ordinate.Y, _fixture.Create<double>());
            _xySequence.SetOrdinate(0, Ordinate.Z, _fixture.Create<double>());

            _sut = new PointM(_xySequence);
        }

        [Fact]
        public void ThenXContiansTheOrdinateXValue()
        {
            Assert.Equal(_xySequence.GetOrdinate(0, Ordinate.X), _sut.X);
        }

        [Fact]
        public void ThenYContiansTheOrdinateYValue()
        {
            Assert.Equal(_xySequence.GetOrdinate(0, Ordinate.Y), _sut.Y);
        }

        [Fact]
        public void ThenZContiansTheOrdinateZValue()
        {
            Assert.Equal(_xySequence.GetOrdinate(0, Ordinate.Z), _sut.Z);
        }

        [Fact]
        public void ThenMeasureValueCanBeSet()
        {
            var m = _fixture.Create<double>();
            _sut.ChangeMeasurement(m);
            Assert.Equal(m, _sut.M);
        }
    }

    public class WhenCreatingAPointFromACoordinate
    {
        private readonly PointM _sut;
        private readonly Fixture _fixture;
        private readonly Coordinate _coordinate;

        public WhenCreatingAPointFromACoordinate()
        {
            _fixture = new Fixture();

            _coordinate = new Coordinate(
                _fixture.Create<double>(),
                _fixture.Create<double>(),
                _fixture.Create<double>()
            );
            _sut = new PointM(_coordinate);
        }

        [Fact]
        public void ThenXContiansTheOrdinateXValue()
        {
            Assert.Equal(_coordinate.X, _sut.X);
        }

        [Fact]
        public void ThenYContiansTheOrdinateYValue()
        {
            Assert.Equal(_coordinate.Y, _sut.Y);
        }

        [Fact]
        public void ThenZContiansTheOrdinateZValue()
        {
            Assert.Equal(_coordinate.Z, _sut.Z);
        }

        [Fact]
        public void ThenMeasureValueCanBeSet()
        {
            var m = _fixture.Create<double>();
            _sut.ChangeMeasurement(m);
            Assert.Equal(m, _sut.M);
        }
    }

    public class WhenCreatingAPointFromXyValues
    {
        private readonly PointM _sut;
        private readonly Fixture _fixture;
        private readonly double _x, _y;

        public WhenCreatingAPointFromXyValues()
        {
            _fixture = new Fixture();

            _x = _fixture.Create<double>();
            _y = _fixture.Create<double>();
            _sut = new PointM(_x, _y);
        }

        [Fact]
        public void ThenXValueIsSet()
        {
            Assert.Equal(_x, _sut.X);
        }

        [Fact]
        public void ThenYValueIsSet()
        {
            Assert.Equal(_y, _sut.Y);
        }

        [Fact]
        public void ThenZValueIsNotSet()
        {
            Assert.Equal(double.NaN, _sut.Z);
        }

        [Fact]
        public void ThenMeasureValueCanBeSet()
        {
            var m = _fixture.Create<double>();
            _sut.ChangeMeasurement(m);
            Assert.Equal(m, _sut.M);
        }
    }

    public class WhenCreatingAPointFromXyzValues
    {
        private readonly PointM _sut;
        private readonly Fixture _fixture;
        private readonly double _x, _y, _z;

        public WhenCreatingAPointFromXyzValues()
        {
            _fixture = new Fixture();

            _x = _fixture.Create<double>();
            _y = _fixture.Create<double>();
            _z = _fixture.Create<double>();
            _sut = new PointM(_x, _y, _z);
        }

        [Fact]
        public void ThenXValueIsSet()
        {
            Assert.Equal(_x, _sut.X);
        }

        [Fact]
        public void ThenYValueIsSet()
        {
            Assert.Equal(_y, _sut.Y);
        }

        [Fact]
        public void ThenZValueIsSet()
        {
            Assert.Equal(_z, _sut.Z);
        }

        [Fact]
        public void ThenMeasureValueCanBeSet()
        {
            var m = _fixture.Create<double>();
            _sut.ChangeMeasurement(m);
            Assert.Equal(m, _sut.M);
        }
    }

    public class WhenCreatingAPointFromXyzmValues
    {
        private readonly PointM _sut;
        private readonly Fixture _fixture;
        private readonly double _x, _y, _z, _m;

        public WhenCreatingAPointFromXyzmValues()
        {
            _fixture = new Fixture();

            _x = _fixture.Create<double>();
            _y = _fixture.Create<double>();
            _z = _fixture.Create<double>();
            _m = _fixture.Create<double>();
            _sut = new PointM(_x, _y, _z, _m);
        }

        [Fact]
        public void ThenXValueIsSet()
        {
            Assert.Equal(_x, _sut.X);
        }

        [Fact]
        public void ThenYValueIsSet()
        {
            Assert.Equal(_y, _sut.Y);
        }

        [Fact]
        public void ThenZValueIsSet()
        {
            Assert.Equal(_z, _sut.Z);
        }

        [Fact]
        public void ThenMValueIsSet()
        {
            Assert.Equal(_m, _sut.M);
        }

        [Fact]
        public void ThenMeasureValueCanBeSet()
        {
            var m = _fixture.Create<double>();
            _sut.ChangeMeasurement(m);
            Assert.Equal(m, _sut.M);
        }
    }
}
