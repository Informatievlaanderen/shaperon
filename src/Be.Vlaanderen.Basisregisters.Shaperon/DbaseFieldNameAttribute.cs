namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class DbaseFieldNameAttribute : Attribute
    {
        public readonly string DbaseFieldName;

        public DbaseFieldNameAttribute(string dbaseFieldName)
        {
            DbaseFieldName = dbaseFieldName;
        }
    }
}
