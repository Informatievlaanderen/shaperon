namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public enum DbaseFieldType : byte
    {
        Number = (byte) 'N',
        Date = (byte) 'D',
        Character = (byte) 'C',
        Float = (byte) 'F',
        Logical = (byte) 'L',
        [Obsolete("Please use Character or Date instead. The anonymous dbase record will no longer infer date time values. The dbase field will no longer write datetime fields as being character fields in the dbase file header.")]
        DateTime = (byte) 'T'
    }
}
