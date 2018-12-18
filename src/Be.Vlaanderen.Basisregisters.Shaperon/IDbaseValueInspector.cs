namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public interface IDbaseFieldValueInspector
    {
        void Inspect(DbaseDateTime value);
        void Inspect(DbaseDecimal value);
        void Inspect(DbaseDouble value);
        void Inspect(DbaseSingle value);
        void Inspect(DbaseInt16 value);
        void Inspect(DbaseInt32 value);
        void Inspect(DbaseString value);
        void Inspect(DbaseBoolean value);
    }
}
