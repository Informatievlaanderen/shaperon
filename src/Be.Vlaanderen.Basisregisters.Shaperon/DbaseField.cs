namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DbaseField
    {
        public static readonly DbaseFieldLength DecimalSeparatorLength = new DbaseFieldLength(1);
        public static readonly DbaseFieldLength SignLength = new DbaseFieldLength(1);

        private readonly Dictionary<DbaseFieldType, Func<DbaseField, DbaseFieldValue>> _factories =
            new Dictionary<DbaseFieldType, Func<DbaseField, DbaseFieldValue>>
            {
                {
                    DbaseFieldType.Character,
                    field => new DbaseString(field)
                },
                {
                    DbaseFieldType.Number,
                    field =>
                    {
                        if (field.DecimalCount.ToInt32() == 0)
                        {
                            return new DbaseInt32(field);
                        }

                        return new DbaseDouble(field);
                    }
                },
                {
                    DbaseFieldType.Float,
                    field =>
                    {
                        if (field.DecimalCount.ToInt32() == 0)
                        {
                            return new DbaseInt32(field);
                        }

                        return new DbaseSingle(field);
                    }
                },
                {
                    DbaseFieldType.DateTime,
                    field => new DbaseDateTime(field)
                },
                {
                    DbaseFieldType.Logical,
                    field => new DbaseBoolean(field)
                }
            };

        public DbaseField(DbaseFieldName name, DbaseFieldType fieldType, ByteOffset offset, DbaseFieldLength length,
            DbaseDecimalCount decimalCount)
        {
            if (!Enum.IsDefined(typeof(DbaseFieldType), fieldType))
            {
                throw new ArgumentException(
                    $"The field type {fieldType} of field {name} is not supported.",
                    nameof(fieldType));
            }

            switch (fieldType)
            {
                case DbaseFieldType.Character:
                    if (decimalCount.ToInt32() != 0)
                        throw new ArgumentException(
                            $"The character field {name} decimal count ({decimalCount}) must be set to 0.",
                            nameof(decimalCount));

                    break;
                case DbaseFieldType.DateTime:
                    if (length.ToInt32() != 15)
                        throw new ArgumentException($"The datetime field {name} length ({length}) must be set to 15.",
                            nameof(length));

                    if (decimalCount.ToInt32() != 0)
                        throw new ArgumentException(
                            $"The datetime field {name} decimal count ({decimalCount}) must be set to 0.",
                            nameof(decimalCount));

                    break;
                case DbaseFieldType.Number:
                    if (length > DbaseDouble.MaximumLength)
                        throw new ArgumentException(
                            $"The number field {name} length ({length}) must be less than or equal to {DbaseDouble.MaximumLength}.",
                            nameof(length));

                    if (decimalCount.ToInt32() != 0)
                    {
                        if (length < DbaseDouble.MinimumLength)
                            throw new ArgumentException(
                                $"The number field {name} length ({length}) must be at least {DbaseDouble.MinimumLength}.",
                                nameof(length));

                        if (decimalCount > DbaseDouble.MaximumDecimalCount)
                            throw new ArgumentException(
                                $"The number field {name} decimal count ({decimalCount}) must be less than or equal to {DbaseDouble.MaximumDecimalCount}.",
                                nameof(decimalCount));

                        if (decimalCount.ToInt32() > length.ToInt32() - 2)
                            throw new ArgumentException(
                                $"The number field {name} decimal count ({decimalCount}) must be 2 less than its length ({length}).",
                                nameof(decimalCount));
                    }

                    break;

                case DbaseFieldType.Float:
                    if (length > DbaseSingle.MaximumLength)
                        throw new ArgumentException(
                            $"The float field {name} length ({length}) must be less than or equal to {DbaseSingle.MaximumLength}.",
                            nameof(length));

                    if (decimalCount.ToInt32() != 0)
                    {
                        if (length < DbaseSingle.MinimumLength)
                            throw new ArgumentException(
                                $"The number field {name} length ({length}) must be at least {DbaseSingle.MinimumLength}.",
                                nameof(length));

                        if (decimalCount > DbaseSingle.MaximumDecimalCount)
                            throw new ArgumentException(
                                $"The float field {name} decimal count ({decimalCount}) must be less than or equal to {DbaseSingle.MaximumDecimalCount}.",
                                nameof(decimalCount));

                        if (decimalCount.ToInt32() > length.ToInt32() - 2)
                            throw new ArgumentException(
                                $"The float field {name} decimal count ({decimalCount}) must be 2 less than its length ({length}).",
                                nameof(decimalCount));
                    }

                    break;

                case DbaseFieldType.Logical:
                    if (decimalCount.ToInt32() != 0)
                        throw new ArgumentException(
                            $"The logical field {name} decimal count ({decimalCount}) must be set to 0.",
                            nameof(decimalCount));

                    if (length.ToInt32() != 1)
                        throw new ArgumentException(
                            $"The logical field {name} length ({length}) must be set to 1.",
                            nameof(length));

                    break;
            }

            Name = name;
            FieldType = fieldType;
            Offset = offset;
            Length = length;
            DecimalCount = decimalCount;

            if (FieldType == DbaseFieldType.Number || FieldType == DbaseFieldType.Float)
            {
                PositiveIntegerDigits =
                    DecimalCount.ToInt32() != 0
                        ? new DbaseIntegerDigits(
                            Length
                                .Minus(DecimalCount.ToLength())
                                .Minus(DecimalSeparatorLength)
                                .ToInt32()
                        )
                        : new DbaseIntegerDigits(Length.ToInt32());

                NegativeIntegerDigits =
                    PositiveIntegerDigits != new DbaseIntegerDigits(0)
                        ? PositiveIntegerDigits
                            .Minus(SignLength)
                        : new DbaseIntegerDigits(0);
            }
            else
            {
                PositiveIntegerDigits = new DbaseIntegerDigits(0);
                NegativeIntegerDigits = new DbaseIntegerDigits(0);
            }
        }

        public DbaseField After(DbaseField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return new DbaseField(Name, FieldType, field.Offset.Plus(field.Length), Length, DecimalCount);
        }

        public DbaseField At(ByteOffset offset) => new DbaseField(Name, FieldType, offset, Length, DecimalCount);

        public static DbaseField CreateStringField(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Character, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        public static DbaseField CreateInt32Field(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        public static DbaseField CreateInt16Field(DbaseFieldName name, DbaseFieldLength length)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, new DbaseDecimalCount(0));

        public static DbaseField CreateDateTimeField(DbaseFieldName name)
            => new DbaseField(name, DbaseFieldType.DateTime, ByteOffset.Initial, new DbaseFieldLength(15), new DbaseDecimalCount(0));

        public static DbaseField CreateDoubleField(DbaseFieldName name, DbaseFieldLength length, DbaseDecimalCount decimalCount)
            => new DbaseField(name, DbaseFieldType.Number, ByteOffset.Initial, length, decimalCount);

        public static DbaseField CreateSingleField(DbaseFieldName name, DbaseFieldLength length, DbaseDecimalCount decimalCount)
            => new DbaseField(name, DbaseFieldType.Float, ByteOffset.Initial, length, decimalCount);

        public static DbaseField CreateLogicalField(DbaseFieldName name)
            => new DbaseField(name, DbaseFieldType.Logical, ByteOffset.Initial, new DbaseFieldLength(1), new DbaseDecimalCount(0));

        public DbaseFieldName Name { get; }
        public DbaseFieldType FieldType { get; }
        public ByteOffset Offset { get; }
        public DbaseFieldLength Length { get; }
        public DbaseDecimalCount DecimalCount { get; }
        public DbaseIntegerDigits PositiveIntegerDigits { get; }
        public DbaseIntegerDigits NegativeIntegerDigits { get; }

        private bool Equals(DbaseField other) =>
            other != null &&
            Name.Equals(other.Name) &&
            // HACK: Because legacy represents date times as characters - so why bother with DateTime support?
            (
                (
                    (FieldType == DbaseFieldType.Character || FieldType == DbaseFieldType.DateTime)
                    &&
                    (other.FieldType == DbaseFieldType.Character || other.FieldType == DbaseFieldType.DateTime)
                )
                ||
                FieldType == other.FieldType
            ) &&
            Offset.Equals(other.Offset) &&
            Length.Equals(other.Length) &&
            DecimalCount.Equals(other.DecimalCount);

        public override bool Equals(object obj) =>
            obj is DbaseField field && Equals(field);

        public override int GetHashCode() =>
            Name.GetHashCode() ^
            // HACK: Because legacy represents date times as characters - so why bother with DateTime support?
            (FieldType == DbaseFieldType.DateTime ? DbaseFieldType.Character : FieldType).GetHashCode() ^
            Offset.GetHashCode() ^
            Length.GetHashCode() ^
            DecimalCount.GetHashCode();

        public DbaseFieldValue CreateFieldValue() => _factories[FieldType](this);

        public static DbaseField Read(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var name = new DbaseFieldName(reader.ReadRightPaddedFieldName(11, char.MinValue));
            var typeOfField = reader.ReadByte();

            if (!Enum.IsDefined(typeof(DbaseFieldType), typeOfField))
            {
                var values = Enum.GetValues(typeof(DbaseFieldType));
                var supportedValues = string.Join(
                    ",",
                    Enumerable
                        .Range(0, values.Length)
                        .Select(index =>
                            values.GetValue(index).ToString() + "[" + ((byte) values.GetValue(index)).ToString() + "]")
                );

                throw new DbaseFileHeaderException(
                    $"The field type {typeOfField} of field {name} is not supported ({supportedValues}).");
            }

            var fieldType = (DbaseFieldType) typeOfField;
            var offset = new ByteOffset(reader.ReadInt32());
            var length = new DbaseFieldLength(reader.ReadByte());
            var decimalCount = new DbaseDecimalCount(reader.ReadByte());
            reader.ReadBytes(14);

            return new DbaseField(name, fieldType, offset, length, decimalCount);
        }

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteRightPaddedString(Name.ToString(), 11, char.MinValue);

            // HACK: Because legacy represents date times as characters - so why bother with DateTime support?
            if (FieldType == DbaseFieldType.DateTime)
            {
                writer.Write((byte) DbaseFieldType.Character);
            }
            else
            {
                writer.Write((byte) FieldType);
            }

            writer.Write(Offset.ToInt32());
            writer.Write(Length.ToByte());
            writer.Write(DecimalCount.ToByte());
            writer.Write(new byte[14]);
        }
    }
}
