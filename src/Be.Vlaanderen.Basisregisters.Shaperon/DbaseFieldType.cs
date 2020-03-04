namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public enum DbaseFieldType : byte
    {
        Number = (byte) 'N',
        Date = (byte) 'D',
        Character = (byte) 'C',
        Float = (byte) 'F',
        Logical = (byte) 'L'
    }
}
