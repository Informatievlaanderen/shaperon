namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Diagnostics.Contracts;

    public readonly struct SpatialReferenceSystemIdentifier : IEquatable<SpatialReferenceSystemIdentifier>
    {
        public static readonly SpatialReferenceSystemIdentifier BelgeLambert1972 =
            new SpatialReferenceSystemIdentifier(103300);

        private readonly int _value;

        public SpatialReferenceSystemIdentifier(int value) => _value = value;

        public bool Equals(SpatialReferenceSystemIdentifier other) => _value.Equals(other._value);

        public override bool Equals(object obj) =>
            obj is SpatialReferenceSystemIdentifier identifier && Equals(identifier);

        public override int GetHashCode() => _value;
        public override string ToString() => _value.ToString();

        [Pure]
        public int ToInt32() => _value;
    }
}
