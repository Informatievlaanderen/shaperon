namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct ByteOffset : IEquatable<ByteOffset>, IComparable<ByteOffset>
    {
        public static readonly ByteOffset Initial = new ByteOffset(0);

        private readonly int _value;

        public ByteOffset(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value of byte offset must be greater than or equal to 0.");

            _value = value;
        }

        [Pure]
        public ByteOffset Plus(ByteLength other) => new ByteOffset(_value + other.ToInt32());

        [Pure]
        public ByteOffset Plus(DbaseFieldLength other) => new ByteOffset(_value + other.ToInt32());

        public bool Equals(ByteOffset instance) => instance._value == _value;
        public override bool Equals(object obj) => obj is ByteOffset offset && Equals(offset);
        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        [Pure]
        public int ToInt32() => _value;

        public int CompareTo(ByteOffset other) => _value.CompareTo(other.ToInt32());

        public static ByteOffset operator +(ByteOffset left, ByteOffset right) => new ByteOffset(left.ToInt32() + right.ToInt32());

        public static ByteOffset operator +(ByteOffset left, ByteLength right) => left.Plus(right);
        public static ByteOffset operator +(ByteOffset left, DbaseFieldLength right) => left.Plus(right);
        public static bool operator ==(ByteOffset left, ByteOffset right) => left.Equals(right);
        public static bool operator !=(ByteOffset left, ByteOffset right) => !left.Equals(right);
        public static bool operator <(ByteOffset left, ByteOffset right) => left.CompareTo(right) < 0;
        public static bool operator <=(ByteOffset left, ByteOffset right) => left.CompareTo(right) <= 0;
        public static bool operator >=(ByteOffset left, ByteOffset right) => left.CompareTo(right) >= 0;
        public static bool operator >(ByteOffset left, ByteOffset right) => left.CompareTo(right) > 0;
    }
}
