namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;

    internal static class BinaryReaderExtensions
    {
        public static string ReadRightPaddedFieldName(this BinaryReader reader, int length, char padding)
        {
            var characters = reader.ReadChars(length);
            if (characters.Length != length)
            {
                throw new EndOfStreamException(
                    $"Unable to read beyond the end of the stream. Expected stream to have {length} byte(s) available but only found {characters.Length} byte(s) as part of reading a field name."
                );
            }
            var index = characters.Length - 1;

            while (index >= 0 && characters[index].Equals(padding))
                index--;

            return index < 0
                ? string.Empty
                : new string(characters, 0, index + 1);
        }

        public static string ReadRightPaddedString(this BinaryReader reader, string field, int length, char padding)
        {
            var characters = reader.ReadChars(length);
            if (characters.Length != length)
            {
                throw new EndOfStreamException(
                    $"Unable to read beyond the end of the stream. Expected stream to have {length} byte(s) available but only found {characters.Length} byte(s) as part of reading field {field}."
                );
            }
            var index = characters.Length - 1;

            while (index >= 0 && characters[index].Equals(padding))
                index--;

            return index < 0
                ? string.Empty
                : new string(characters, 0, index + 1);
        }

        public static string ReadLeftPaddedString(this BinaryReader reader, string field, int length, char padding)
        {
            var characters = reader.ReadChars(length);
            if (characters.Length != length)
            {
                throw new EndOfStreamException(
                    $"Unable to read beyond the end of the stream. Expected stream to have {length} byte(s) available but only found {characters.Length} byte(s) as part of reading field {field}."
                );
            }
            var index = 0;

            while (index < characters.Length && characters[index].Equals(padding))
                index++;

            return index == characters.Length
                ? string.Empty
                : new string(characters, index, characters.Length - index);
        }

        public static void WriteRightPaddedString(this BinaryWriter writer, string value, int length, char padding)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            writer.Write(value.PadRight(length, padding).ToCharArray());
        }

        public static void WriteLeftPaddedString(this BinaryWriter writer, string value, int length, char padding)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            writer.Write(value.PadLeft(length, padding).ToCharArray());
        }
    }
}
