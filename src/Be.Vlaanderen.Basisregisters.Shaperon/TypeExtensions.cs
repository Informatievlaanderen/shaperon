#nullable disable
using System;

namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    public static class TypeExtensions
    {
        public static string GetAttributeValue(this Type t)
        {
            var attribute = Attribute.GetCustomAttribute(t, typeof(DbaseFieldNameAttribute)) as DbaseFieldNameAttribute;

            return attribute?.DbaseFieldName;
        }
    }
}
