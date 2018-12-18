using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct WordLength : IEquatable<WordLength>, IComparable<WordLength>, IComparable<ByteLength>
    {
        private readonly int _value;

        public WordLength(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The value of word length must be greater than or equal to 0.");
            }

            _value = value;
        }

        [Pure]
        public WordLength Plus(WordLength other)
        {
            return new WordLength(_value + other.ToInt32());
        }

        [Pure]
        public WordLength Plus(ByteLength other)
        {
            return new WordLength(_value + other.ToWordLength().ToInt32());
        }

        [Pure]
        public WordLength Minus(WordLength other)
        {
            return new WordLength(_value - other.ToInt32());
        }

        [Pure]
        public WordLength Minus(ByteLength other)
        {
            return new WordLength(_value - other.ToWordLength().ToInt32());
        }

        [Pure]
        public WordLength Times(int times)
        {
            return new WordLength(_value * times);
        }

        [Pure]
        public int ToInt32() => _value;
        [Pure]
        public ByteLength ToByteLength() => new ByteLength(_value * 2);
        public bool Equals(WordLength instance) => instance._value == _value;
        public override bool Equals(object obj) => obj is WordLength length && Equals(length);
        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        public int CompareTo(WordLength other)
        {
            return _value.CompareTo(other.ToInt32());
        }

        public int CompareTo(ByteLength other)
        {
            return _value.CompareTo(other.ToWordLength().ToInt32());
        }

        //public static implicit operator int(WordLength instance) => instance.ToInt32();

        public static WordLength operator +(WordLength left, WordLength right) => left.Plus(right);
        public static WordLength operator -(WordLength left, WordLength right) => left.Minus(right);
        public static bool operator ==(WordLength left, WordLength right) => left.Equals(right);
        public static bool operator !=(WordLength left, WordLength right) => !left.Equals(right);
        public static bool operator <(WordLength left, WordLength right) => left.CompareTo(right) < 0;
        public static bool operator <=(WordLength left, WordLength right) => left.CompareTo(right) <= 0;
        public static bool operator >=(WordLength left, WordLength right) => left.CompareTo(right) >= 0;
        public static bool operator >(WordLength left, WordLength right) => left.CompareTo(right) > 0;

        public static WordLength operator +(WordLength left, ByteLength right) => left.Plus(right);
        public static WordLength operator -(WordLength left, ByteLength right) => left.Minus(right);
        public static bool operator ==(WordLength left, ByteLength right) => left.Equals(right.ToWordLength());
        public static bool operator !=(WordLength left, ByteLength right) => !left.Equals(right.ToWordLength());
        public static bool operator <(WordLength left, ByteLength right) => left.CompareTo(right.ToWordLength()) < 0;
        public static bool operator <=(WordLength left, ByteLength right) => left.CompareTo(right.ToWordLength()) <= 0;
        public static bool operator >=(WordLength left, ByteLength right) => left.CompareTo(right.ToWordLength()) >= 0;
        public static bool operator >(WordLength left, ByteLength right) => left.CompareTo(right.ToWordLength()) > 0;
    }
}
