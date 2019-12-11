namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public class AnonymousDbaseRecord : DbaseRecord
    {
        public AnonymousDbaseRecord(DbaseField[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            IsDeleted = false;
            Values = Array.ConvertAll(fields, field => field.CreateFieldValue());
        }

        public AnonymousDbaseRecord(DbaseFieldValue[] values)
        {
            IsDeleted = false;
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }
    }
}
