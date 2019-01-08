namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct WordOffset : IEquatable<WordOffset>, IComparable<WordOffset>
    {
        private readonly int _value;

        public WordOffset(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "The value of word offset must be greater than or equal to 0.");

            _value = value;
        }

        [Pure]
        public WordOffset Plus(ByteLength other) => new WordOffset(_value + other.ToWordLength().ToInt32());

        [Pure]
        public WordOffset Plus(WordLength other) => new WordOffset(_value + other.ToInt32());

        [Pure]
        public int ToInt32() => _value;

        public bool Equals(WordOffset instance) => instance._value == _value;
        public override bool Equals(object obj) => obj is WordOffset offset && Equals(offset);
        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        public int CompareTo(WordOffset other) => _value.CompareTo(other.ToInt32());

        public static WordOffset operator +(WordOffset left, WordLength right) => left.Plus(right);
        public static WordOffset operator +(WordOffset left, ByteLength right) => left.Plus(right);
        public static bool operator ==(WordOffset left, WordOffset right) => left.Equals(right);
        public static bool operator !=(WordOffset left, WordOffset right) => !left.Equals(right);
        public static bool operator <(WordOffset left, WordOffset right) => left.CompareTo(right) < 0;
        public static bool operator <=(WordOffset left, WordOffset right) => left.CompareTo(right) <= 0;
        public static bool operator >=(WordOffset left, WordOffset right) => left.CompareTo(right) >= 0;
        public static bool operator >(WordOffset left, WordOffset right) => left.CompareTo(right) > 0;
    }
}
