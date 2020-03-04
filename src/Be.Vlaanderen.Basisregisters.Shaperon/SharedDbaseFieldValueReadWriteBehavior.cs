namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal static class SharedDbaseFieldValueReadWriteBehavior
    {
        // shared by dbase boolean, nullable boolean, logical
        public static bool? ReadAsNullableBoolean(this BinaryReader reader)
        {
            bool? result = default;

            switch (reader.ReadByte())
            {
                case DbaseLogicalBytes.Bytet:
                case DbaseLogicalBytes.ByteT:
                case DbaseLogicalBytes.Bytey:
                case DbaseLogicalBytes.ByteY:
                    result = true;
                    break;

                case DbaseLogicalBytes.Bytef:
                case DbaseLogicalBytes.ByteF:
                case DbaseLogicalBytes.Byten:
                case DbaseLogicalBytes.ByteN:
                    result = false;
                    break;
            }

            return result;
        }

        public static void WriteAsNullableBoolean(this BinaryWriter writer, bool? value)
        {
            writer.Write(FormatAsByte(value));
        }

        private static byte FormatAsByte(bool? value) =>
            value.HasValue
                ? value.Value
                    ? DbaseLogicalBytes.ByteT
                    : DbaseLogicalBytes.ByteF
                : DbaseLogicalBytes.ByteUnknown;

        // shared by dbase decimal, nullable decimal

        public static decimal? ReadAsNullableDecimal(this BinaryReader reader, DbaseField field, NumberFormatInfo provider)
        {
            decimal? result = default;

            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');

                result = decimal.TryParse(unpadded, DbaseNumber.DoubleNumberStyle, provider, out var parsed)
                    ? (decimal?) parsed
                    : default;
            }

            return result;
        }

        public static void WriteAsNullableDecimal(this BinaryWriter writer, DbaseField field, NumberFormatInfo provider, decimal? value)
        {
            if (value.HasValue)
            {
                var unpadded = value.Value.ToString("F", provider);
                if (unpadded.Length < field.Length.ToInt32() && field.DecimalCount.ToInt32() > 0)
                {
                    // Pad with decimal zeros if space left.
                    var parts = unpadded.Split(provider.NumberDecimalSeparator.Single());
                    if (parts.Length == 2 && parts[1].Length < field.DecimalCount.ToInt32())
                    {
                        unpadded = string.Concat(
                            unpadded,
                            new string(
                                '0',
                                field.DecimalCount.ToInt32() - parts[1].Length
                            )
                        );
                    }
                }

                writer.WriteLeftPaddedString(unpadded, field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        // shared by dbase double, nullable double, number

        public static double? ReadAsNullableDouble(this BinaryReader reader, DbaseField field, NumberFormatInfo provider)
        {
            double? result = default;
            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');

                result = double.TryParse(unpadded, DbaseNumber.DoubleNumberStyle, provider, out var parsed)
                    ? (double?) parsed
                    : default;
            }

            return result;
        }

        public static void WriteAsNullableDouble(this BinaryWriter writer, DbaseField field, NumberFormatInfo provider,
            double? value)
        {
            if (value.HasValue)
            {
                var unpadded = value.Value.ToString("F", provider);
                if (unpadded.Length < field.Length.ToInt32() && field.DecimalCount.ToInt32() > 0)
                {
                    // Pad with decimal zeros if space left.
                    var parts = unpadded.Split(provider.NumberDecimalSeparator.Single());
                    if (parts.Length == 2 && parts[1].Length < field.DecimalCount.ToInt32())
                    {
                        unpadded = string.Concat(
                            unpadded,
                            new string(
                                '0',
                                field.DecimalCount.ToInt32() - parts[1].Length
                            )
                        );
                    }
                }

                writer.WriteLeftPaddedString(unpadded, field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        // shared by dbase single, nullable single, float

        public static float? ReadAsNullableSingle(this BinaryReader reader, DbaseField field, NumberFormatInfo provider)
        {
            float? result = default;

            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');
                result = float.TryParse(unpadded, DbaseFloat.NumberStyle, provider, out var parsed)
                    ? (float?) parsed
                    : default;
            }

            return result;
        }

        public static void WriteAsNullableSingle(this BinaryWriter writer, DbaseField field, NumberFormatInfo provider,
            float? value)
        {
            if (value.HasValue)
            {
                var unpadded = value.Value.ToString("F", provider);
                if (unpadded.Length < field.Length.ToInt32() && field.DecimalCount.ToInt32() > 0)
                {
                    // Pad with decimal zeros if space left.
                    var parts = unpadded.Split(provider.NumberDecimalSeparator.Single());
                    if (parts.Length == 2 && parts[1].Length < field.DecimalCount.ToInt32())
                    {
                        unpadded = string.Concat(
                            unpadded,
                            new string(
                                '0',
                                field.DecimalCount.ToInt32() - parts[1].Length));
                    }
                }

                writer.WriteLeftPaddedString(unpadded, field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        // shared by dbase int16, nullable int16

        public static short? ReadAsNullableInt16(this BinaryReader reader, DbaseField field)
        {
            short? result = default;
            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');

                result = short.TryParse(unpadded,
                    DbaseNumber.IntegerNumberStyle,
                    CultureInfo.InvariantCulture, out var parsed)
                    ? (short?) parsed
                    : default;
            }

            return result;
        }

        public static void WriteAsNullableInt16(this BinaryWriter writer, DbaseField field, short? value)
        {
            if (value.HasValue)
            {
                var unpadded = DbaseInt16.FormatAsString(value.Value);
                writer.WriteLeftPaddedString(unpadded, field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        // shared by dbase int16, nullable int16

        public static int? ReadAsNullableInt32(this BinaryReader reader, DbaseField field)
        {
            int? result = default;
            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                var unpadded = reader.ReadLeftPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');

                result = int.TryParse(unpadded,
                    DbaseNumber.IntegerNumberStyle,
                    CultureInfo.InvariantCulture, out var parsed)
                    ? (int?) parsed
                    : default;
            }

            return result;
        }

        public static void WriteAsNullableInt32(this BinaryWriter writer, DbaseField field, int? value)
        {
            if (value.HasValue)
            {
                var unpadded = DbaseInt32.FormatAsString(value.Value);
                writer.WriteLeftPaddedString(unpadded, field.Length.ToInt32(), ' ');
            }
            else
            {
                writer.Write(new string(' ', field.Length.ToInt32()).ToCharArray());
                // or writer.Write(new byte[Field.Length]); // to determine
            }
        }

        // shared by dbase string, character, datetime, nullable datetime, datetime offset, nullable datetime offset

        public static string ReadAsNullableString(this BinaryReader reader, DbaseField field)
        {
            string result = default;
            if (reader.PeekChar() == '\0')
            {
                var read = reader.ReadBytes(field.Length.ToInt32());
                if (read.Length != field.Length.ToInt32())
                {
                    throw new EndOfStreamException(
                        $"Unable to read beyond the end of the stream. Expected stream to have {field.Length.ToInt32()} byte(s) available but only found {read.Length} byte(s) as part of reading field {field.Name.ToString()}."
                    );
                }
            }
            else
            {
                result = reader.ReadRightPaddedString(field.Name.ToString(), field.Length.ToInt32(), ' ');
            }

            return result;
        }

        public static void WriteAsNullableString(this BinaryWriter writer, DbaseField field, string value)
        {
            if (value == null)
            {
                writer.Write(new byte[field.Length.ToInt32()]);
            }
            else
            {
                writer.WriteRightPaddedString(value, field.Length.ToInt32(), ' ');
            }
        }
    }
}
