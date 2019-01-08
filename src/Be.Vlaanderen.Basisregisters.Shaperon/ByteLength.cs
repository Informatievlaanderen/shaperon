namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct ByteLength : IEquatable<ByteLength>
    {
        public static readonly ByteLength Int32 = new ByteLength(4);
        public static readonly ByteLength Double = new ByteLength(8);

        private readonly int _value;

        public ByteLength(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value of byte length must be greater than or equal to 0.");

            if (value % 2 != 0)
                throw new ArgumentException("The value of byte length must be a multiple of 2 (even).", nameof(value));

            _value = value;
        }

        [Pure]
        public ByteLength Plus(ByteLength other) => new ByteLength(_value + other.ToInt32());

        [Pure]
        public ByteLength Plus(WordLength other) => new ByteLength(_value + other.ToByteLength().ToInt32());

        [Pure]
        public ByteLength Times(int times) => new ByteLength(_value * times);

        [Pure]
        public int ToInt32() => _value;

        [Pure]
        public WordLength ToWordLength() => new WordLength(_value / 2);

        public bool Equals(ByteLength instance) => instance._value == _value;
        public override bool Equals(object obj) => obj is ByteLength length && Equals(length);
        public override int GetHashCode() => _value;

        public override string ToString() => _value.ToString();
    }
}
