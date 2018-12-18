using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct DbaseIntegerDigits : IEquatable<DbaseIntegerDigits>, IComparable<DbaseIntegerDigits>
    {
        private readonly int _value;

        public DbaseIntegerDigits(int value)
        {
            if (value < 0 || value > 254)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The integer digits of a dbase field must be between 0 and 254.");
            _value = value;
        }

        public static DbaseIntegerDigits Min(DbaseIntegerDigits left, DbaseIntegerDigits right)
        {
            return new DbaseIntegerDigits(Math.Min(left.ToInt32(), right.ToInt32()));
        }

        public static DbaseIntegerDigits Max(DbaseIntegerDigits left, DbaseIntegerDigits right)
        {
            return new DbaseIntegerDigits(Math.Max(left.ToInt32(), right.ToInt32()));
        }

        [Pure]
        public DbaseIntegerDigits Plus(DbaseIntegerDigits other)
        {
            return new DbaseIntegerDigits(_value + other._value);
        }

        [Pure]
        public DbaseIntegerDigits Minus(DbaseIntegerDigits other)
        {
            return new DbaseIntegerDigits(_value - other._value);
        }

        [Pure]
        public DbaseIntegerDigits Plus(DbaseFieldLength other)
        {
            return new DbaseIntegerDigits(_value + other.ToInt32());
        }

        [Pure]
        public DbaseIntegerDigits Minus(DbaseFieldLength other)
        {
            return new DbaseIntegerDigits(_value - other.ToInt32());
        }

        public bool Equals(DbaseIntegerDigits other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DbaseIntegerDigits count && Equals(count);
        public override int GetHashCode() => _value;
        [Pure]
        public int ToInt32() => _value;
        public override string ToString() => _value.ToString();
        [Pure]
        public DbaseFieldLength ToLength() => new DbaseFieldLength(_value);
        public int CompareTo(DbaseIntegerDigits other) => _value.CompareTo(other._value);

        public static DbaseIntegerDigits operator +(DbaseIntegerDigits left, DbaseIntegerDigits right) =>
            left.Plus(right);

        public static DbaseIntegerDigits operator -(DbaseIntegerDigits left, DbaseIntegerDigits right) =>
            left.Minus(right);

        public static DbaseIntegerDigits operator +(DbaseIntegerDigits left, DbaseFieldLength right) =>
            left.Plus(right);

        public static DbaseIntegerDigits operator -(DbaseIntegerDigits left, DbaseFieldLength right) =>
            left.Minus(right);

        public static bool operator ==(DbaseIntegerDigits left, DbaseIntegerDigits right) => left.Equals(right);
        public static bool operator !=(DbaseIntegerDigits left, DbaseIntegerDigits right) => !left.Equals(right);
        public static bool operator <=(DbaseIntegerDigits left, DbaseIntegerDigits right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseIntegerDigits left, DbaseIntegerDigits right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseIntegerDigits left, DbaseIntegerDigits right) => left.CompareTo(right) >= 0;

        public static bool operator >(DbaseIntegerDigits left, DbaseIntegerDigits right) => left.CompareTo(right) > 0;
        //public static implicit operator int(DbaseIntegerDigits instance) => instance.ToInt32();
        //public static implicit operator DbaseFieldLength(DbaseIntegerDigits instance) => instance.ToLength();
    }
}
