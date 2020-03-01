namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public partial interface ITypedDbaseFieldValueVisitor
    {
        void Visit(DbaseInt16 value);
        void Visit(DbaseNullableInt16 value);
        void Visit(DbaseInt32 value);
        void Visit(DbaseNullableInt32 value);
        void Visit(DbaseString value);
        //DbaseDouble
        //DbaseNullableDouble
        //DbaseSingle
        //DbaseNullableSingle
        //DbaseBoolean
        //DbaseNullableBoolean
        //
        void Visit(DbaseDateTime value);
        void Visit(DbaseNullableDateTime value);
        //DbaseDateTimeOffset
        //DbaseNullableDateTimeOffset
    }
}
