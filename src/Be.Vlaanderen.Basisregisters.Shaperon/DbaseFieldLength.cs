using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct DbaseFieldLength : IEquatable<DbaseFieldLength>, IComparable<DbaseFieldLength>
    {
        public static readonly DbaseFieldLength MinLength = new DbaseFieldLength(0);
        public static readonly DbaseFieldLength MaxLength = new DbaseFieldLength(254);

        private readonly int _value;

        public DbaseFieldLength(int value)
        {
            if (value < 0 || value > 254)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The length of a dbase field must be between 0 and 254.");
            _value = value;
        }

        public static DbaseFieldLength Min(DbaseFieldLength left, DbaseFieldLength right)
        {
            return new DbaseFieldLength(Math.Min(left.ToInt32(), right.ToInt32()));
        }

        public static DbaseFieldLength Max(DbaseFieldLength left, DbaseFieldLength right)
        {
            return new DbaseFieldLength(Math.Max(left.ToInt32(), right.ToInt32()));
        }

        [Pure]
        public DbaseFieldLength Plus(DbaseFieldLength other)
        {
            return new DbaseFieldLength(_value + other._value);
        }

        [Pure]
        public DbaseFieldLength Minus(DbaseFieldLength other)
        {
            return new DbaseFieldLength(_value - other._value);
        }

        public bool Equals(DbaseFieldLength other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DbaseFieldLength length && Equals(length);
        public override int GetHashCode() => _value;
        [Pure]
        public int ToInt32() => _value;
        [Pure]
        public byte ToByte() => Convert.ToByte(_value);
        public override string ToString() => _value.ToString();

        public int CompareTo(DbaseFieldLength other) => _value.CompareTo(other._value);

        public static DbaseFieldLength operator +(DbaseFieldLength left, DbaseFieldLength right) => left.Plus(right);
        public static DbaseFieldLength operator -(DbaseFieldLength left, DbaseFieldLength right) => left.Minus(right);
        public static bool operator ==(DbaseFieldLength left, DbaseFieldLength right) => left.Equals(right);
        public static bool operator !=(DbaseFieldLength left, DbaseFieldLength right) => !left.Equals(right);
        public static bool operator <=(DbaseFieldLength left, DbaseFieldLength right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseFieldLength left, DbaseFieldLength right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseFieldLength left, DbaseFieldLength right) => left.CompareTo(right) >= 0;

        public static bool operator >(DbaseFieldLength left, DbaseFieldLength right) => left.CompareTo(right) > 0;
        //public static implicit operator int(DbaseFieldLength instance) => instance.ToInt32();
        //public static implicit operator byte(DbaseFieldLength instance) => instance.ToByte();
    }
}
