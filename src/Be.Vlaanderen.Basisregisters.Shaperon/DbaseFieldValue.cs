namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    public abstract class DbaseFieldValue
    {
        protected DbaseFieldValue(DbaseField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public DbaseField Field { get; }
        public abstract void Read(BinaryReader reader);
        public abstract void Write(BinaryWriter writer);
        public abstract void Accept(IDbaseFieldValueVisitor visitor);
    }
}
