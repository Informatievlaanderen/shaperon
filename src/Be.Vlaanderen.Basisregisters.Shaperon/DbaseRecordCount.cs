namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct DbaseRecordCount :
        IEquatable<DbaseRecordCount>,
        IComparable<DbaseRecordCount>,
        IEquatable<RecordNumber>,
        IComparable<RecordNumber>
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

        public int CompareTo(DbaseRecordCount other)
        {
            return _value.CompareTo(other._value);
        }

        public bool Equals(RecordNumber other) => _value.Equals(other.ToInt32());

        public int CompareTo(RecordNumber other)
        {
            return _value.CompareTo(other.ToInt32());
        }

        public override bool Equals(object obj)
        {
            if (obj is DbaseRecordCount length && Equals(length))
            {
                return true;
            }

            if (obj is RecordNumber number && Equals(number))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode() => _value;

        [Pure]
        public int ToInt32() => _value;

        public override string ToString() => _value.ToString();

        public static bool operator ==(DbaseRecordCount left, DbaseRecordCount right) => left.Equals(right);
        public static bool operator !=(DbaseRecordCount left, DbaseRecordCount right) => !left.Equals(right);
        public static bool operator <=(DbaseRecordCount left, DbaseRecordCount right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseRecordCount left, DbaseRecordCount right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseRecordCount left, DbaseRecordCount right) => left.CompareTo(right) >= 0;
        public static bool operator >(DbaseRecordCount left, DbaseRecordCount right) => left.CompareTo(right) > 0;

        public static bool operator ==(DbaseRecordCount left, RecordNumber right) => left.Equals(right);
        public static bool operator !=(DbaseRecordCount left, RecordNumber right) => !left.Equals(right);
        public static bool operator <=(DbaseRecordCount left, RecordNumber right) => left.CompareTo(right) <= 0;
        public static bool operator <(DbaseRecordCount left, RecordNumber right) => left.CompareTo(right) < 0;
        public static bool operator >=(DbaseRecordCount left, RecordNumber right) => left.CompareTo(right) >= 0;
        public static bool operator >(DbaseRecordCount left, RecordNumber right) => left.CompareTo(right) > 0;
    }
}
