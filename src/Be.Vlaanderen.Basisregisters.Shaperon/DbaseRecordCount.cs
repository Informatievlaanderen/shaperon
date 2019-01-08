namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct DbaseRecordCount : IEquatable<DbaseRecordCount>
    {
        private readonly int _value;

        public DbaseRecordCount(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "The count of dbase records must be greater than or equal to 0.");

            _value = value;
        }

        public bool Equals(DbaseRecordCount other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DbaseRecordCount length && Equals(length);
        public override int GetHashCode() => _value;

        [Pure]
        public int ToInt32() => _value;

        public override string ToString() => _value.ToString();
    }
}
