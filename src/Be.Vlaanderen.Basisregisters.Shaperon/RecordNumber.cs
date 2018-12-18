using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct RecordNumber : IEquatable<RecordNumber>
    {
        public static readonly RecordNumber Initial = new RecordNumber(1);

        private readonly int _value;

        public RecordNumber(int value)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The value of a record number must be greater than or equal to 1.");
            _value = value;
        }

        [Pure]
        public RecordNumber Next()
        {
            if (_value == int.MaxValue)
            {
                throw new InvalidOperationException(
                    "The maximum record number has been reached. There's no next record number.");
            }

            return new RecordNumber(_value + 1);
        }

        [Pure]
        public int ToInt32() => _value;
        public bool Equals(RecordNumber instance) => instance._value == _value;
        public override bool Equals(object obj) => obj is RecordNumber number && Equals(number);
        public override int GetHashCode() => _value;

        public override string ToString() => _value.ToString();
        //public static implicit operator int(RecordNumber instance) => instance.ToInt32();
    }
}
