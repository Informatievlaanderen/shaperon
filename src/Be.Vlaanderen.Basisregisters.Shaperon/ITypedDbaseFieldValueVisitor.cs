namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public interface ITypedDbaseFieldValueVisitor : IDbaseFieldValueVisitor
    {
        void Visit(DbaseInt16 value);
        void Visit(DbaseNullableInt16 value);
        void Visit(DbaseInt32 value);
        void Visit(DbaseNullableInt32 value);
        void Visit(DbaseString value);
        void Visit(DbaseBoolean value);
        void Visit(DbaseNullableBoolean value);
        void Visit(DbaseDecimal value);
        void Visit(DbaseNullableDecimal value);
        void Visit(DbaseDouble value);
        void Visit(DbaseNullableDouble value);
        void Visit(DbaseSingle value);
        void Visit(DbaseNullableSingle value);
        void Visit(DbaseDateTime value);
        void Visit(DbaseNullableDateTime value);
        void Visit(DbaseDateTimeOffset value);
        void Visit(DbaseNullableDateTimeOffset value);
    }
}
