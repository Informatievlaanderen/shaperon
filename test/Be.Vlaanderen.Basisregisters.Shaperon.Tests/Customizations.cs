namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using AutoFixture.Dsl;
    using System;
    using System.Linq;

    internal static class Customizations
    {
        public static void CustomizePoint(this IFixture fixture)
        {
            fixture.Customize<Point>(customization =>
                customization.FromFactory(generator =>
                    new Point(
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
        }

        public static void CustomizePolygon(this IFixture fixture)
        {
            fixture.Customize<BoundingBox2D>(customization =>
                customization.FromFactory(generator =>
                    new BoundingBox2D(
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            fixture.Customize<Polygon>(customization =>
                customization.FromFactory(generator =>
                {
                    var numberOfPoints = generator.Next(10, 101);
                    numberOfPoints = numberOfPoints % 2 == 0 ? numberOfPoints : numberOfPoints + 1;
                    var numberOfParts = numberOfPoints * 2;
                    var parts = Enumerable.Range(0, numberOfParts).Select(part => part * 2).ToArray();
                    var points = fixture.CreateMany<Point>(numberOfPoints).ToArray();
                    return new Polygon(fixture.Create<BoundingBox2D>(), parts, points);
                }).OmitAutoProperties()
            );
        }

        public static void CustomizePolyLineM(this IFixture fixture)
        {
            fixture.Customize<BoundingBox2D>(customization =>
                customization.FromFactory(generator =>
                    new BoundingBox2D(
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties()
            );
            fixture.Customize<PolyLineM>(customization =>
                customization.FromFactory(generator =>
                {
                    var numberOfPoints = generator.Next(10, 101);
                    numberOfPoints = numberOfPoints % 2 == 0 ? numberOfPoints : numberOfPoints + 1;
                    var numberOfParts = numberOfPoints * 2;
                    var parts = Enumerable.Range(0, numberOfParts).Select(part => part * 2).ToArray();
                    var points = fixture.CreateMany<Point>(numberOfPoints).ToArray();
                    var measures = fixture.CreateMany<double>(numberOfPoints).ToArray();
                    return new PolyLineM(fixture.Create<BoundingBox2D>(), parts, points, measures);
                }).OmitAutoProperties()
            );
        }

        public static void CustomizeWordLength(this IFixture fixture)
        {
            fixture.Customize<WordLength>(
                customization =>
                    customization.FromFactory<int>(
                        value => new WordLength(Math.Abs(value))
                    ));
        }

        public static void CustomizeWordOffset(this IFixture fixture)
        {
            fixture.Customize<WordOffset>(
                customization =>
                    customization.FromFactory<int>(
                        value => new WordOffset(Math.Abs(value))
                    ));
        }

        public static void CustomizeByteOffset(this IFixture fixture)
        {
            fixture.Customize<ByteOffset>(
                customization =>
                    customization.FromFactory<int>(
                        value => new ByteOffset(Math.Abs(value))
                    ));
        }

        public static void CustomizeByteLength(this IFixture fixture)
        {
            fixture.Customize<ByteLength>(
                customization =>
                    customization.FromFactory<int>(
                        value => new ByteLength(value.AsByteLengthValue())
                    ));
        }

        public static void CustomizeRecordNumber(this IFixture fixture)
        {
            fixture.Customize<RecordNumber>(
                customization =>
                    customization.FromFactory<int>(
                        value => new RecordNumber(value.AsRecordNumberValue())
                    ));
        }

        public static void CustomizeDbaseFieldName(this IFixture fixture)
        {
            fixture.Customize<DbaseFieldName>(customization =>
                customization.FromFactory<int>(value =>
                    new DbaseFieldName(new string('a', value.AsDbaseFieldNameLength()))));
        }

        public static void CustomizeDbaseFieldLength(this IFixture fixture)
        {
            fixture.Customize<DbaseFieldLength>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseFieldLength(value.AsDbaseFieldLengthValue())
                    ));
        }

        public static void CustomizeDbaseFieldLength(this IFixture fixture, DbaseFieldLength maxLength)
        {
            fixture.Customize<DbaseFieldLength>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseFieldLength(value.AsDbaseFieldLengthValue(maxLength.ToInt32()))
                    ));
        }

        public static void CustomizeDbaseRecordCount(this IFixture fixture)
        {
            fixture.Customize<DbaseRecordCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseRecordCount(value.AsDbaseRecordCountValue())
                    ));
        }

        public static void CustomizeDbaseRecordCount(this IFixture fixture, int maximum)
        {
            fixture.Customize<DbaseRecordCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseRecordCount(value.AsDbaseRecordCountValue(maximum))
                    ));
        }

        public static void CustomizeShapeRecordCount(this IFixture fixture)
        {
            fixture.Customize<ShapeRecordCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new ShapeRecordCount(value.AsShapeRecordCountValue())
                    ));
        }

        public static void CustomizeShapeRecordCount(this IFixture fixture, int maximum)
        {
            fixture.Customize<ShapeRecordCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new ShapeRecordCount(value.AsShapeRecordCountValue(maximum))
                    ));
        }

        public static void CustomizeDbaseRecordLength(this IFixture fixture)
        {
            fixture.Customize<DbaseRecordLength>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseRecordLength(value.AsDbaseRecordLengthValue())
                    ));
        }

        public static void CustomizeDbaseDecimalCount(this IFixture fixture)
        {
            fixture.Customize<DbaseDecimalCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseDecimalCount(value.AsDbaseDecimalCountValue())
                    ));
        }

        public static void CustomizeDbaseDecimalCount(this IFixture fixture, DbaseDecimalCount maximum)
        {
            fixture.Customize<DbaseDecimalCount>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseDecimalCount(value.AsDbaseDecimalCountValue(maximum.ToInt32()))
                    ));
        }

        public static void CustomizeDbaseIntegerDigits(this IFixture fixture)
        {
            fixture.Customize<DbaseIntegerDigits>(
                customization =>
                    customization.FromFactory<int>(
                        value => new DbaseIntegerDigits(value.AsDbaseIntegerDigitsValue())
                    ));
        }

        public static DbaseFieldLength GenerateDbaseInt32Length(this IFixture fixture, DbaseFieldType fieldType)
        {
            using (var random = new PooledRandom())
            {
                if (fieldType == DbaseFieldType.Number)
                {
                    return new DbaseFieldLength(random.Next(1, DbaseNumber.MaximumLength.ToInt32()));
                }

                return new DbaseFieldLength(random.Next(1, DbaseFloat.MaximumLength.ToInt32()));
            }
        }

        public static DbaseFieldLength GenerateDbaseInt16Length(this IFixture fixture, DbaseFieldType fieldType)
        {
            using (var random = new PooledRandom())
            {
                if (fieldType == DbaseFieldType.Number)
                {
                    return new DbaseFieldLength(random.Next(1, DbaseNumber.MaximumLength.ToInt32()));
                }

                return new DbaseFieldLength(random.Next(1, DbaseFloat.MaximumLength.ToInt32()));
            }
        }

        public static DbaseFieldLength GenerateDbaseInt32LengthLessThan(this IFixture fixture,
            DbaseFieldLength maxLength)
        {
            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(1, maxLength.ToInt32()));
            }
        }

        public static DbaseFieldType GenerateDbaseInt32FieldType(this IFixture fixture)
        {
            return new Generator<DbaseFieldType>(fixture)
                .First(specimen => specimen == DbaseFieldType.Number || specimen == DbaseFieldType.Float);
        }

        public static DbaseFieldType GenerateDbaseInt16FieldType(this IFixture fixture)
        {
            return new Generator<DbaseFieldType>(fixture)
                .First(specimen => specimen == DbaseFieldType.Number || specimen == DbaseFieldType.Float);
        }

        public static DbaseFieldType GenerateDbaseDateTimeFieldType(this IFixture fixture)
        {
            return new Generator<DbaseFieldType>(fixture)
                .First(specimen => specimen == DbaseFieldType.DateTime || specimen == DbaseFieldType.Character);
        }

        public static int GenerateDbaseSchemaFieldCount(this IFixture fixture)
        {
            return new Generator<int>(fixture)
                .First(specimen => specimen <= DbaseSchema.MaximumFieldCount);
        }

        public static void CustomizeDbaseCodePage(this IFixture fixture)
        {
            fixture.Customize<DbaseCodePage>(
                customization =>
                    customization.FromFactory<int>(
                        value => DbaseCodePage.All[value % DbaseCodePage.All.Length]
                    ));
        }

        public static void CustomizeDbaseCodePageWithCodePage(this IFixture fixture)
        {
            fixture.Customize<DbaseCodePage>(
                customization =>
                    customization.FromFactory<int>(
                        value => DbaseCodePage.AllWithCodePage[value % DbaseCodePage.AllWithCodePage.Length]
                    ));
        }

        public static void CustomizeDbaseSchema(this IFixture fixture)
        {
            fixture.Customize<DbaseSchema>(
                customization =>
                    customization.FromFactory<int>(
                        value =>
                        {
                            var fields = fixture
                                .CreateMany<DbaseField>(value.AsMaximumFieldCount())
                                .ToArray();
                            for (var index = 0; index < fields.Length; index++)
                            {
                                fields[index] = index == 0
                                    ? fields[index].At(ByteOffset.Initial)
                                    : fields[index].After(fields[index - 1]);
                            }

                            return new AnonymousDbaseSchema(fields);
                        }
                    ));
        }

        public static void CustomizeAnonymousDbaseSchema(this IFixture fixture)
        {
            fixture.Customize<AnonymousDbaseSchema>(
                customization =>
                    customization.FromFactory<int>(
                        value =>
                        {
                            var fields = fixture
                                .CreateMany<DbaseField>(value.AsMaximumFieldCount())
                                .ToArray();
                            for (var index = 0; index < fields.Length; index++)
                            {
                                fields[index] = index == 0
                                    ? fields[index].At(ByteOffset.Initial)
                                    : fields[index].After(fields[index - 1]);
                            }

                            return new AnonymousDbaseSchema(fields);
                        }
                    ));
        }

        public static DbaseFieldLength GenerateDbaseDecimalLength(this IFixture fixture)
        {
            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseDecimal.MinimumLength.ToInt32(),
                    DbaseDecimal.MaximumLength.ToInt32() + 1));
            }
        }

        public static DbaseFieldLength GenerateDbaseDecimalLengthLessThan(this IFixture fixture,
            DbaseFieldLength maxLength)
        {
            if (maxLength < DbaseDecimal.MinimumLength || maxLength > DbaseDecimal.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseDecimal.MinimumLength} and {DbaseDecimal.MaximumLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseDecimal.MinimumLength.ToInt32(), maxLength.ToInt32() + 1));
            }
        }

        public static DbaseFieldLength GenerateDbaseDecimalLengthBetween(this IFixture fixture,
            DbaseFieldLength minLength, DbaseFieldLength maxLength)
        {
            if (minLength < DbaseDecimal.MinimumLength || minLength > DbaseDecimal.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length needs to be between {DbaseDecimal.MinimumLength} and {DbaseDecimal.MaximumLength}.");
            }

            if (maxLength < DbaseDecimal.MinimumLength || maxLength > DbaseDecimal.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseDecimal.MinimumLength} and {DbaseDecimal.MaximumLength}.");
            }

            if (minLength > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length {minLength} needs to be less than or equal to the maximum length {maxLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(minLength.ToInt32(), maxLength.ToInt32() + 1));
            }
        }

        public static DbaseDecimalCount GenerateDbaseDecimalDecimalCount(this IFixture fixture, DbaseFieldLength length)
        {
            if (length > DbaseDecimal.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"The length ({length}) can not exceed the maximum length ({DbaseDecimal.MaximumLength}).");
            }

            if (length < DbaseDecimal.MinimumLength)
            {
                return new DbaseDecimalCount(0);
            }

            using (var random = new PooledRandom())
            {
                return new DbaseDecimalCount(random.Next(0,
                    Math.Min(DbaseDecimal.MaximumDecimalCount.ToInt32(), length.ToInt32()) - 2));
            }
        }

        public static DbaseDecimalCount GenerateDbaseDecimalDecimalCount(this IFixture fixture,
            DbaseDecimalCount minimum, DbaseFieldLength length)
        {
            if (length > DbaseDecimal.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"The length ({length}) can not exceed the maximum length ({DbaseDecimal.MaximumLength}).");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseDecimalCount(random.Next(minimum.ToInt32(),
                    Math.Min(DbaseDecimal.MaximumDecimalCount.ToInt32() + 1, length.ToInt32()) - 2));
            }
        }

        public static DbaseFieldLength GenerateDbaseDoubleLength(this IFixture fixture)
        {
            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseNumber.MinimumLength.ToInt32(),
                    DbaseNumber.MaximumLength.ToInt32() + 1));
            }
        }

        public static DbaseFieldLength GenerateDbaseDoubleLengthLessThan(this IFixture fixture,
            DbaseFieldLength maxLength)
        {
            if (maxLength < DbaseNumber.MinimumLength || maxLength > DbaseNumber.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseNumber.MinimumLength} and {DbaseNumber.MaximumLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseNumber.MinimumLength.ToInt32(), maxLength.ToInt32() + 1));
            }
        }

        public static DbaseFieldLength GenerateDbaseDoubleLengthBetween(this IFixture fixture,
            DbaseFieldLength minLength, DbaseFieldLength maxLength)
        {
            if (minLength < DbaseNumber.MinimumLength || minLength > DbaseNumber.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length needs to be between {DbaseNumber.MinimumLength} and {DbaseNumber.MaximumLength}.");
            }

            if (maxLength < DbaseNumber.MinimumLength || maxLength > DbaseNumber.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseNumber.MinimumLength} and {DbaseNumber.MaximumLength}.");
            }

            if (minLength > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length {minLength} needs to be less than or equal to the maximum length {maxLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(minLength.ToInt32(), maxLength.ToInt32() + 1));
            }
        }

        public static DbaseDecimalCount GenerateDbaseDoubleDecimalCount(this IFixture fixture, DbaseFieldLength length)
        {
            if (length > DbaseNumber.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"The length ({length}) can not exceed the maximum length ({DbaseNumber.MaximumLength}).");
            }

            if (length < DbaseNumber.MinimumLength)
            {
                return new DbaseDecimalCount(0);
            }

            using (var random = new PooledRandom())
            {
                return new DbaseDecimalCount(random.Next(0,
                    Math.Min(DbaseNumber.MaximumDecimalCount.ToInt32(), length.ToInt32()) - 2));
            }
        }

        public static DbaseDecimalCount GenerateDbaseDoubleDecimalCount(this IFixture fixture,
            DbaseDecimalCount minimum, DbaseFieldLength length)
        {
            if (length > DbaseNumber.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"The length ({length}) can not exceed the maximum length ({DbaseNumber.MaximumLength}).");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseDecimalCount(random.Next(minimum.ToInt32(),
                    Math.Min(DbaseNumber.MaximumDecimalCount.ToInt32() + 1, length.ToInt32()) - 2));
            }
        }

        public static DbaseFieldLength GenerateDbaseSingleLength(this IFixture fixture)
        {
            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseFloat.MinimumLength.ToInt32(),
                    DbaseFloat.MaximumLength.ToInt32()));
            }
        }

        public static DbaseFieldLength GenerateDbaseSingleLengthLessThan(this IFixture fixture,
            DbaseFieldLength maxLength)
        {
            if (maxLength < DbaseFloat.MinimumLength || maxLength > DbaseFloat.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseFloat.MinimumLength} and {DbaseFloat.MaximumLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(DbaseFloat.MinimumLength.ToInt32(), maxLength.ToInt32()));
            }
        }

        public static DbaseFieldLength GenerateDbaseSingleLengthBetween(this IFixture fixture,
            DbaseFieldLength minLength, DbaseFieldLength maxLength)
        {
            if (minLength < DbaseFloat.MinimumLength || minLength > DbaseFloat.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length needs to be between {DbaseFloat.MinimumLength} and {DbaseFloat.MaximumLength}.");
            }

            if (maxLength < DbaseFloat.MinimumLength || maxLength > DbaseFloat.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength),
                    $"The maximum length needs to be between {DbaseFloat.MinimumLength} and {DbaseFloat.MaximumLength}.");
            }

            if (minLength > maxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength),
                    $"The minimum length {minLength} needs to be less than or equal to the maximum length {maxLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseFieldLength(random.Next(minLength.ToInt32(), maxLength.ToInt32()));
            }
        }

        public static DbaseDecimalCount GenerateDbaseSingleDecimalCount(this IFixture fixture, DbaseFieldLength length)
        {
            if (length < DbaseFloat.MinimumLength || length > DbaseFloat.MaximumLength)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"The length needs to be between {DbaseFloat.MinimumLength} and {DbaseFloat.MaximumLength}.");
            }

            using (var random = new PooledRandom())
            {
                return new DbaseDecimalCount(random.Next(1,
                    Math.Min(DbaseFloat.MaximumDecimalCount.ToInt32(), length.ToInt32()) - 2));
            }
        }

        public static void CustomizeDbaseField(this IFixture fixture)
        {
            fixture.Customize<DbaseField>(
                customization =>
                    customization.FromFactory<int>(
                        value =>
                        {
                            DbaseField field;
                            switch (value % 5)
                            {
                                case 1: // date
                                    field = new DbaseField(
                                        fixture.Create<DbaseFieldName>(),
                                        DbaseFieldType.Date,
                                        fixture.Create<ByteOffset>(),
                                        new DbaseFieldLength(8),
                                        new DbaseDecimalCount(0)
                                    );
                                    break;
                                case 2: // number
                                    var doubleLength = fixture.GenerateDbaseDoubleLength();
                                    var doubleDecimalCount = fixture.GenerateDbaseDoubleDecimalCount(doubleLength);
                                    field = new DbaseField(
                                        fixture.Create<DbaseFieldName>(),
                                        DbaseFieldType.Number,
                                        fixture.Create<ByteOffset>(),
                                        doubleLength,
                                        doubleDecimalCount
                                    );
                                    break;
                                case 3: // float
                                    var singleLength =
                                        fixture.GenerateDbaseSingleLengthLessThan(DbaseFloat.MaximumLength);
                                    var singleDecimalCount = fixture.GenerateDbaseSingleDecimalCount(singleLength);
                                    field = new DbaseField(
                                        fixture.Create<DbaseFieldName>(),
                                        DbaseFieldType.Float,
                                        fixture.Create<ByteOffset>(),
                                        singleLength,
                                        singleDecimalCount
                                    );
                                    break;
                                case 4: // logical
                                    field = new DbaseField(
                                        fixture.Create<DbaseFieldName>(),
                                        DbaseFieldType.Logical,
                                        fixture.Create<ByteOffset>(),
                                        new DbaseFieldLength(1),
                                        new DbaseDecimalCount(0)
                                    );
                                    break;
                                default:
                                    // case 0: // character
                                    field = new DbaseField(
                                        fixture.Create<DbaseFieldName>(),
                                        DbaseFieldType.Character,
                                        fixture.Create<ByteOffset>(),
                                        fixture.Create<DbaseFieldLength>(),
                                        new DbaseDecimalCount(0)
                                    );
                                    break;
                            }

                            return field;
                        }
                    )
            );
        }

        public static void CustomizeDbaseString(this IFixture fixture)
        {
            fixture.Customize<DbaseCharacter>(
                customization =>
                    customization
                        .FromFactory<int>(
                            value =>
                            {
                                using (var random = new PooledRandom(value))
                                {
                                    var length = new DbaseFieldLength(random.Next(1, 255));
                                    return new DbaseCharacter(
                                        new DbaseField(
                                            fixture.Create<DbaseFieldName>(),
                                            DbaseFieldType.Character,
                                            fixture.Create<ByteOffset>(),
                                            length,
                                            new DbaseDecimalCount(0)
                                        ),
                                        new string('a', random.Next(0, length.ToInt32())));
                                }
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseInt32(this IFixture fixture)
        {
            fixture.Customize<DbaseInt32>(
                customization =>
                    customization
                        .FromNumberGenerator(
                            generator =>
                            {
                                var fieldType = fixture.GenerateDbaseInt32FieldType();
                                var length = fixture.GenerateDbaseInt32Length(fieldType);
                                var field = new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    fieldType,
                                    fixture.Create<ByteOffset>(),
                                    length,
                                    new DbaseDecimalCount(0)
                                );
                                var value = new DbaseInt32(field);
                                value.Value = generator.GenerateAcceptableValue(value);
                                return value;
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseInt16(this IFixture fixture)
        {
            fixture.Customize<DbaseInt16>(
                customization =>
                    customization
                        .FromNumberGenerator(
                            generator =>
                            {
                                var fieldType = fixture.GenerateDbaseInt16FieldType();
                                var length = fixture.GenerateDbaseInt16Length(fieldType);
                                var field = new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    fieldType,
                                    fixture.Create<ByteOffset>(),
                                    length,
                                    new DbaseDecimalCount(0)
                                );
                                var value = new DbaseInt16(field);
                                value.Value = generator.GenerateAcceptableValue(value);
                                return value;
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseLogical(this IFixture fixture)
        {
            fixture.Customize<DbaseLogical>(
                customization =>
                    customization
                        .FromFactory<bool?>(
                            value => new DbaseLogical(
                                new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    DbaseFieldType.Logical,
                                    fixture.Create<ByteOffset>(),
                                    new DbaseFieldLength(1),
                                    new DbaseDecimalCount(0)
                                ), value)
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseDecimal(this IFixture fixture)
        {
            fixture.Customize<DbaseDecimal>(
                customization =>
                    customization
                        .FromNumberGenerator(
                            generator =>
                            {
                                var length = fixture.GenerateDbaseDecimalLength();
                                var decimalCount = fixture.GenerateDbaseDecimalDecimalCount(length);
                                var field = new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    DbaseFieldType.Number,
                                    fixture.Create<ByteOffset>(),
                                    length,
                                    decimalCount
                                );
                                var value = new DbaseDecimal(field);
                                value.Value = generator.GenerateAcceptableValue(value);
                                return value;
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseDouble(this IFixture fixture)
        {
            fixture.Customize<DbaseNumber>(
                customization =>
                    customization
                        .FromNumberGenerator(
                            generator =>
                            {
                                var length = fixture.GenerateDbaseDoubleLength();
                                var decimalCount = fixture.GenerateDbaseDoubleDecimalCount(length);
                                var field = new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    DbaseFieldType.Number,
                                    fixture.Create<ByteOffset>(),
                                    length,
                                    decimalCount
                                );
                                var value = new DbaseNumber(field);
                                value.Value = generator.GenerateAcceptableValue(value);
                                return value;
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseSingle(this IFixture fixture)
        {
            fixture.Customize<DbaseFloat>(
                customization =>
                    customization
                        .FromNumberGenerator(
                            generator =>
                            {
                                var length = fixture.GenerateDbaseSingleLength();
                                var decimalCount = fixture.GenerateDbaseSingleDecimalCount(length);
                                var field = new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    DbaseFieldType.Float,
                                    fixture.Create<ByteOffset>(),
                                    length,
                                    decimalCount
                                );
                                var value = new DbaseFloat(field);
                                value.Value = generator.GenerateAcceptableValue(value);
                                return value;
                            }
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseDateTime(this IFixture fixture)
        {
            fixture.Customize<DbaseDateTime>(
                customization =>
                    customization
                        .FromFactory<DateTime?>(
                            value => new DbaseDateTime(
                                new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    fixture.GenerateDbaseDateTimeFieldType(),
                                    fixture.Create<ByteOffset>(),
                                    new DbaseFieldLength(15),
                                    new DbaseDecimalCount(0)
                                ), value)
                        )
                        .OmitAutoProperties());
        }

        public static void CustomizeDbaseDate(this IFixture fixture)
        {
            fixture.Customize<DbaseDate>(
                customization =>
                    customization
                        .FromFactory<DateTime?>(
                            value => new DbaseDate(
                                new DbaseField(
                                    fixture.Create<DbaseFieldName>(),
                                    DbaseFieldType.Date,
                                    fixture.Create<ByteOffset>(),
                                    new DbaseFieldLength(8),
                                    new DbaseDecimalCount(0)
                                ), value)
                        )
                        .OmitAutoProperties());
        }

        public static DbaseField[] GenerateDbaseFields(this IFixture fixture)
        {
            return fixture.GenerateDbaseFields(fixture.GenerateDbaseSchemaFieldCount());
        }

        public static DbaseField[] GenerateDbaseFields(this IFixture fixture, int count)
        {
            var fields = fixture.CreateMany<DbaseField>(count).ToArray();
            for (var index = 0; index < count; index++)
            {
                fields[index] = index == 0
                    ? fields[index].At(ByteOffset.Initial)
                    : fields[index].After(fields[index - 1]);
            }

            return fields;
        }

        public static DbaseRecord GenerateDbaseRecord(this IFixture fixture, DbaseField[] fields)
        {
            return fixture.GenerateDbaseRecords(fields, new DbaseRecordCount(1)).Single();
        }

        public static DbaseRecord[] GenerateDbaseRecords(this IFixture fixture, DbaseField[] fields)
        {
            return fixture.GenerateDbaseRecords(fields, fixture.Create<DbaseRecordCount>());
        }

        public static DbaseRecord[] GenerateDbaseRecords(this IFixture fixture, DbaseField[] fields,
            DbaseRecordCount count)
        {
            var records = new DbaseRecord[count.ToInt32()];
            var inspector = new GenerateValueVisitor(fixture);
            for (var index = 0; index < records.Length; index++)
            {
                var record = new AnonymousDbaseRecord(fields);
                foreach (var value in record.Values)
                {
                    value.Accept(inspector);
                }

                records[index] = record;
            }

            return records;
        }

        public static IPostprocessComposer<T> FromFactory<T>(this IFactoryComposer<T> composer, Func<Random, T> factory)
        {
            return composer.FromFactory<int>(value =>
            {
                using (var random = new PooledRandom(value))
                {
                    return factory(random);
                }
            });
        }

        public static IPostprocessComposer<T> FromNumberGenerator<T>(this IFactoryComposer<T> composer,
            Func<DbaseFieldNumberGenerator, T> factory)
        {
            return composer.FromFactory<int>(value =>
            {
                using (var random = new PooledRandom(value))
                {
                    return factory(new DbaseFieldNumberGenerator(random));
                }
            });
        }
    }
}
