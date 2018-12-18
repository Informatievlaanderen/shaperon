using System;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public class AnonymousDbaseSchema : DbaseSchema
    {
        public AnonymousDbaseSchema(DbaseField[] fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            Fields = fields;
        }
    }
}
