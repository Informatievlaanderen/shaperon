namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public readonly struct Tolerance : IEquatable<Tolerance>
    {
        private readonly double _value;
        public Tolerance(double value)
        {
            if(double.IsNaN(value))
                throw new ArgumentException("Tolerance value can not be NaN.", nameof(value));

            if(double.IsNegativeInfinity(value))
                throw new ArgumentException("Tolerance value can not be negative infinity.", nameof(value));

            if(double.IsPositiveInfinity(value))
                throw new ArgumentException("Tolerance value can not be positive infinity.", nameof(value));

            _value = value;
        }

        public bool Equals(Tolerance other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is Tolerance other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        [Pure]
        public double ToDouble() => _value;
        public static implicit operator double(Tolerance instance) => instance.ToDouble();
    }
}