namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class AnonymousDbaseSchema : DbaseSchema
    {
        public AnonymousDbaseSchema(DbaseField[] fields)
        {
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
        }
    }
}
