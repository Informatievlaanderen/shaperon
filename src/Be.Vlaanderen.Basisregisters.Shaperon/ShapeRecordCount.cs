using System;
using System.Diagnostics.Contracts;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public readonly struct ShapeRecordCount : IEquatable<ShapeRecordCount>
    {
        private readonly int _value;

        public ShapeRecordCount(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The count of shape records must be greater than or equal to 0.");
            _value = value;
        }

        public bool Equals(ShapeRecordCount other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is ShapeRecordCount length && Equals(length);
        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        [Pure]
        public int ToInt32() => _value;
        //public static implicit operator int(ShapeRecordCount instance) => instance.ToInt32();
    }
}
