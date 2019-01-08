namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct DbaseDecimalCount : IEquatable<DbaseDecimalCount>, IComparable<DbaseDecimalCount>
    {
        private readonly int _value;

        public DbaseDecimalCount(int value)
        {
            if (value < 0 || value > 254)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "The decimal count of a dbase field must be between 0 and 254.");

            _value = value;
        }

        public static DbaseDecimalCount Min(DbaseDecimalCount left, DbaseDecimalCount right)
            => new DbaseDecimalCount(Math.Min(left.ToInt32(), right.ToInt32()));

        public static DbaseDecimalCount Max(DbaseDecimalCount left, DbaseDecimalCount right)
            => new DbaseDecimalCount(Math.Max(left.ToInt32(), right.ToInt32()));

        [Pure]
        public DbaseDecimalCount Plus(DbaseDecimalCount other)
            => new DbaseDecimalCount(_value + other._value);

        [Pure]
        public DbaseDecimalCount Minus(DbaseDecimalCount other)
            => new DbaseDecimalCount(_value - other._value);

        public bool Equals(DbaseDecimalCount other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DbaseDecimalCount count && Equals(count);
        public override int GetHashCode() => _value;

        [Pure]
        public int ToInt32() => _value;

        [Pure]
        public byte ToByte() => Convert.ToByte(_value);
        public override string ToString() => _value.ToString();

        [Pure]
        public DbaseFieldLength ToLength() => new DbaseFieldLength(_value);
        public int CompareTo(DbaseDecimalCount other) => _value.CompareTo(other._value);

        public static DbaseDecimalCount operator +(DbaseDecimalCount left, DbaseDecimalCount right)
            => left.Plus(right);

        public static DbaseDecimalCount operator -(DbaseDecimalCount left, DbaseDecimalCount right)
            => left.Minus(right);

        public static bool operator ==(DbaseDecimalCount left, DbaseDecimalCount right) => left.Equals(right);
        public static bool operator !=(DbaseDecimalCount left, DbaseDecimalCount right) => !left.Equals(right);
        public static bool operator <=(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) >= 0;
        public static bool operator >(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) > 0;
    }
}
