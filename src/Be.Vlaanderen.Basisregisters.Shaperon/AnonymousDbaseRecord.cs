namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class AnonymousDbaseRecord : DbaseRecord
    {
        public AnonymousDbaseRecord(DbaseField[] fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            IsDeleted = false;
            Values = Array.ConvertAll(fields, field => field.CreateFieldValue());
        }

        public AnonymousDbaseRecord(DbaseFieldValue[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            IsDeleted = false;
            Values = values;
        }

        protected override void ReadValues(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            for (var index = 0; index < Values.Length; index++)
            {
                var value = Values[index];
                value.Read(reader);
                if (value is DbaseString candidate)
                {
                    Values[index] = candidate.TryInferDateTime();
                }
            }
        }
    }
}
