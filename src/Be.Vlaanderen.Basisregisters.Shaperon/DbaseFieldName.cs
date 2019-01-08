namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public readonly struct DbaseFieldName : IEquatable<DbaseFieldName>
    {
        private readonly string _value;

        public DbaseFieldName(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 11)
                throw new ArgumentException("The name can not be longer than 11 characters.", nameof(value));

            _value = value;
        }

        public bool Equals(DbaseFieldName other) => _value.Equals(other._value);
        public override bool Equals(object obj) => obj is DbaseFieldName name && Equals(name);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;
        public static implicit operator string(DbaseFieldName instance) => instance.ToString();
    }
}
