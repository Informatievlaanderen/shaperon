namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public partial interface IDbaseFieldValueVisitor
    {
        void Visit(DbaseDate value);
        void Visit(DbaseNumber value);
        void Visit(DbaseFloat value);
        void Visit(DbaseCharacter value);
        void Visit(DbaseLogical value);
    }
}
