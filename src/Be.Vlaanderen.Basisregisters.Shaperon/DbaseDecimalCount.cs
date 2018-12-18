using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct DbaseDecimalCount : IEquatable<DbaseDecimalCount>, IComparable<DbaseDecimalCount>
    {
        private readonly int _value;

        public DbaseDecimalCount(int value)
        {
            if (value < 0 || value > 254)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The decimal count of a dbase field must be between 0 and 254.");
            _value = value;
        }

        public static DbaseDecimalCount Min(DbaseDecimalCount left, DbaseDecimalCount right)
        {
            return new DbaseDecimalCount(Math.Min(left.ToInt32(), right.ToInt32()));
        }

        public static DbaseDecimalCount Max(DbaseDecimalCount left, DbaseDecimalCount right)
        {
            return new DbaseDecimalCount(Math.Max(left.ToInt32(), right.ToInt32()));
        }

        [Pure]
        public DbaseDecimalCount Plus(DbaseDecimalCount other)
        {
            return new DbaseDecimalCount(_value + other._value);
        }

        [Pure]
        public DbaseDecimalCount Minus(DbaseDecimalCount other)
        {
            return new DbaseDecimalCount(_value - other._value);
        }

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
        public static DbaseDecimalCount operator +(DbaseDecimalCount left, DbaseDecimalCount right) => left.Plus(right);

        public static DbaseDecimalCount operator -(DbaseDecimalCount left, DbaseDecimalCount right) =>
            left.Minus(right);

        public static bool operator ==(DbaseDecimalCount left, DbaseDecimalCount right) => left.Equals(right);
        public static bool operator !=(DbaseDecimalCount left, DbaseDecimalCount right) => !left.Equals(right);
        public static bool operator <=(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) >= 0;

        public static bool operator >(DbaseDecimalCount left, DbaseDecimalCount right) => left.CompareTo(right) > 0;
        //public static implicit operator int(DbaseDecimalCount instance) => instance.ToInt32();
        //public static implicit operator byte(DbaseDecimalCount instance) => instance.ToByte();
        //public static implicit operator DbaseDecimalCount(DbaseDecimalCount instance) => instance.ToLength();
    }
}
