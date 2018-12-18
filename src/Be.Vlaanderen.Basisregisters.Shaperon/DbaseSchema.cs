namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;

    public abstract class DbaseSchema
    {
        public const int MaximumFieldCount = 128;

        private DbaseField[] _fields = Array.Empty<DbaseField>();

        public DbaseField[] Fields
        {
            get => _fields;
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.Length > MaximumFieldCount)
                {
                    throw new ArgumentException(
                        $"The value with ({value.Length}) fields exceeds the maximum number of fields ({MaximumFieldCount}).",
                        nameof(value));
                }

                var fields = new DbaseField[value.Length];
                for (var index = 0; index < fields.Length; index++)
                {
                    fields[index] = index == 0
                        ? value[index].At(ByteOffset.Initial)
                        : value[index].After(fields[index - 1]);
                }

                _fields = fields;
            }
        }

        public DbaseRecordLength Length =>
            Fields.Aggregate(DbaseRecordLength.Initial, (length, field) => length.Plus(field.Length));

        public bool Equals(DbaseSchema other) => other != null && other.Fields.SequenceEqual(Fields);
        public override bool Equals(object obj) => obj is DbaseSchema schema && Equals(schema);
        public override int GetHashCode() => Fields.Aggregate(0, (current, field) => current ^ field.GetHashCode());
    }
}
