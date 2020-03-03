namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using System.Linq;

    public class DbaseFieldNumberGenerator
    {
        private readonly Random _random;

        public DbaseFieldNumberGenerator(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public decimal GenerateAcceptableValue(DbaseDecimal value)
        {
            decimal result = default;
            // null only
            if (value.Field.Length < DbaseNumber.MinimumLength)
            {
                return result;
            }

            // positive only
            if (value.Field.Length < DbaseNumber.NegativeValueMinimumLength)
            {
                result = GeneratePositiveDecimalValue(value.Field);
                return result;
            }

            // positive or negative
            switch (_random.Next() % 2)
            {
                case 0:
                    result = GeneratePositiveDecimalValue(value.Field);
                    break;
                case 1:
                    result = GenerateNegativeDecimalValue(value.Field);
                    break;
            }

            return result;
        }

        public decimal? GenerateAcceptableValue(DbaseNullableDecimal value)
        {
            decimal? result = default;
            // null only
            if (value.Field.Length < DbaseNullableDecimal.MinimumLength)
            {

                return result;
            }

            // null or positive only
            if (value.Field.Length < DbaseNullableDecimal.NegativeValueMinimumLength)
            {
                switch (_random.Next() % 2)
                {
                    //case 0: null
                    case 1:
                        result = GeneratePositiveNullableDecimalValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableDecimalValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableDecimalValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] DecimalSupportedFieldTypes = new[] {DbaseFieldType.Number};

        private decimal? GeneratePositiveNullableDecimalValue(DbaseField field)
        {
            if (field.Length < DbaseDecimal.PositiveValueMinimumLength || !DecimalSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var positiveMultiplier =
                Math.Min(
                    Math.Pow(10, field.PositiveIntegerDigits.ToInt32()),
                    Math.Pow(10, DbaseDecimal.MaximumIntegerDigits.ToInt32())
                );
            return
                Math.Min(
                    Convert.ToDecimal(_random.NextDouble() * positiveMultiplier),
                    decimal.MaxValue
                );
        }

        private decimal? GenerateNegativeNullableDecimalValue(DbaseField field)
        {
            if (field.Length < DbaseDecimal.NegativeValueMinimumLength || !DoubleSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var negativeMultiplier =
                Math.Max(
                    -Math.Pow(10, field.NegativeIntegerDigits.ToInt32()),
                    -Math.Pow(10, DbaseDecimal.MaximumIntegerDigits.ToInt32())
                );
            return
                Math.Max(
                    Convert.ToDecimal(_random.NextDouble() * negativeMultiplier),
                    decimal.MinValue
                );
        }

        private decimal GeneratePositiveDecimalValue(DbaseField field)
        {
            return GeneratePositiveNullableDecimalValue(field).GetValueOrDefault();
        }

        private decimal GenerateNegativeDecimalValue(DbaseField field)
        {
            return GenerateNegativeNullableDecimalValue(field).GetValueOrDefault();
        }

        public double? GenerateAcceptableValue(DbaseNumber value)
        {
            double? result = default;
            // null only
            if (value.Field.Length < DbaseNumber.MinimumLength)
            {

                return result;
            }

            // null or positive only
            if (value.Field.Length < DbaseNumber.NegativeValueMinimumLength)
            {
                switch (_random.Next() % 2)
                {
                    //case 0: null
                    case 1:
                        result = GeneratePositiveNullableDoubleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableDoubleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableDoubleValue(value.Field);
                    break;
            }

            return result;
        }

        public double GenerateAcceptableValue(DbaseDouble value)
        {
            double result = default;
            // null only
            if (value.Field.Length < DbaseDouble.MinimumLength)
            {
                return result;
            }

            // positive only
            if (value.Field.Length < DbaseDouble.NegativeValueMinimumLength)
            {
                result = GeneratePositiveDoubleValue(value.Field);
                return result;
            }

            // positive or negative
            switch (_random.Next() % 2)
            {
                case 0:
                    result = GeneratePositiveDoubleValue(value.Field);
                    break;
                case 1:
                    result = GenerateNegativeDoubleValue(value.Field);
                    break;
            }

            return result;
        }

        public double? GenerateAcceptableValue(DbaseNullableDouble value)
        {
            double? result = default;
            // null only
            if (value.Field.Length < DbaseNullableDouble.MinimumLength)
            {

                return result;
            }

            // null or positive only
            if (value.Field.Length < DbaseNullableDouble.NegativeValueMinimumLength)
            {
                switch (_random.Next() % 2)
                {
                    //case 0: null
                    case 1:
                        result = GeneratePositiveNullableDoubleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableDoubleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableDoubleValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] DoubleSupportedFieldTypes = new[] {DbaseFieldType.Number};

        private double? GeneratePositiveNullableDoubleValue(DbaseField field)
        {
            if (field.Length < DbaseNumber.PositiveValueMinimumLength || !DoubleSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var digits = DbaseIntegerDigits.Min(field.PositiveIntegerDigits, DbaseNumber.MaximumIntegerDigits);
            var number = GeneratePositiveNumber(digits, field.DecimalCount);
            return Double.Parse(
                number,
                NumberStyles.AllowDecimalPoint,
                new NumberFormatInfo
                {
                    NumberDecimalDigits = field.DecimalCount.ToInt32(),
                    NumberDecimalSeparator = "."
                });
        }

        private double? GenerateNegativeNullableDoubleValue(DbaseField field)
        {
            if (field.Length < DbaseNumber.NegativeValueMinimumLength || !DoubleSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var digits = DbaseIntegerDigits.Min(field.NegativeIntegerDigits, DbaseNumber.MaximumIntegerDigits);
            var number = GenerateNegativeNumber(digits, field.DecimalCount);
            return Double.Parse(
                number,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                new NumberFormatInfo
                {
                    NumberDecimalDigits = field.DecimalCount.ToInt32(),
                    NumberDecimalSeparator = ".",
                    NegativeSign = "-"
                });
        }

        private double GeneratePositiveDoubleValue(DbaseField field)
        {
            return GeneratePositiveNullableDoubleValue(field).GetValueOrDefault();
        }

        private double GenerateNegativeDoubleValue(DbaseField field)
        {
            return GenerateNegativeNullableDoubleValue(field).GetValueOrDefault();
        }

        public float? GenerateAcceptableValue(DbaseFloat value)
        {
            float? result = default;
            // null only
            if (value.Field.Length < DbaseFloat.MinimumLength)
            {

                return result;
            }

            // null or positive only
            if (value.Field.Length < DbaseFloat.NegativeValueMinimumLength)
            {
                switch (_random.Next() % 2)
                {
                    //case 0: null
                    case 1:
                        result = GeneratePositiveNullableSingleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableSingleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableSingleValue(value.Field);
                    break;
            }

            return result;
        }

        public float GenerateAcceptableValue(DbaseSingle value)
        {
            float result = default;
            // null only
            if (value.Field.Length < DbaseSingle.MinimumLength)
            {

                return result;
            }

            // positive only
            if (value.Field.Length < DbaseSingle.NegativeValueMinimumLength)
            {
                result = GeneratePositiveSingleValue(value.Field);
                return result;
            }

            // positive or negative
            switch (_random.Next() % 2)
            {
                case 0:
                    result = GeneratePositiveSingleValue(value.Field);
                    break;
                case 1:
                    result = GenerateNegativeSingleValue(value.Field);
                    break;
            }

            return result;
        }

        public float? GenerateAcceptableValue(DbaseNullableSingle value)
        {
            float? result = default;
            // null only
            if (value.Field.Length < DbaseNullableSingle.MinimumLength)
            {

                return result;
            }

            // null or positive only
            if (value.Field.Length < DbaseNullableSingle.NegativeValueMinimumLength)
            {
                switch (_random.Next() % 2)
                {
                    //case 0: null
                    case 1:
                        result = GeneratePositiveNullableSingleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableSingleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableSingleValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] SingleSupportedFieldTypes = new[] {DbaseFieldType.Float};

        private float? GeneratePositiveNullableSingleValue(DbaseField field)
        {
            if (field.Length < DbaseFloat.PositiveValueMinimumLength || !SingleSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var digits = DbaseIntegerDigits.Min(field.PositiveIntegerDigits, DbaseFloat.MaximumIntegerDigits);
            var number = GeneratePositiveNumber(digits, field.DecimalCount);
            return Single.Parse(
                number,
                NumberStyles.AllowDecimalPoint,
                new NumberFormatInfo
                {
                    NumberDecimalDigits = field.DecimalCount.ToInt32(),
                    NumberDecimalSeparator = "."
                });
        }

        private float? GenerateNegativeNullableSingleValue(DbaseField field)
        {
            if (field.Length < DbaseFloat.NegativeValueMinimumLength || !SingleSupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var digits = DbaseIntegerDigits.Min(field.NegativeIntegerDigits, DbaseFloat.MaximumIntegerDigits);
            var number = GenerateNegativeNumber(digits, field.DecimalCount);
            return Single.Parse(
                number,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                new NumberFormatInfo
                {
                    NumberDecimalDigits = field.DecimalCount.ToInt32(),
                    NumberDecimalSeparator = ".",
                    NegativeSign = "-"
                });
        }

        private float GeneratePositiveSingleValue(DbaseField field)
        {
            return GeneratePositiveNullableSingleValue(field).GetValueOrDefault();
        }

        private float GenerateNegativeSingleValue(DbaseField field)
        {
            return GenerateNegativeNullableSingleValue(field).GetValueOrDefault();
        }

        public short? GenerateAcceptableValue(DbaseNullableInt16 value)
        {
            var result = default(short?);
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableInt16Value(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableInt16Value(value.Field);
                    break;
            }

            return result;
        }

        public short GenerateAcceptableValue(DbaseInt16 value)
        {
            var result = default(short);
            switch (_random.Next() % 3)
            {
                case 0:
                    result = GeneratePositiveInt16Value(value.Field);
                    break;
                case 1:
                    result = GenerateNegativeInt16Value(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] Int16SupportedFieldTypes =
            {DbaseFieldType.Number, DbaseFieldType.Float};

        private short? GeneratePositiveNullableInt16Value(DbaseField field)
        {
            if (!Int16SupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var positiveMaximum =
                Convert.ToInt32(
                    Math.Min(
                        Math.Min(
                            Math.Pow(10, field.PositiveIntegerDigits.ToInt32()),
                            Math.Pow(10, DbaseInt16.MaximumIntegerDigits.ToInt32())
                        ),
                        short.MaxValue
                    )
                );
            return Convert.ToInt16(_random.Next(positiveMaximum));
        }

        private short GeneratePositiveInt16Value(DbaseField field)
        {
            return GeneratePositiveNullableInt16Value(field).GetValueOrDefault();
        }

        private short? GenerateNegativeNullableInt16Value(DbaseField field)
        {
            if (!Int16SupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var negativeMaximum =
                Convert.ToInt32(
                    Math.Min(
                        Math.Min(
                            Math.Pow(10, field.NegativeIntegerDigits.ToInt32()),
                            Math.Pow(10, DbaseInt16.MaximumIntegerDigits.ToInt32())
                        ),
                        short.MaxValue
                    )
                );
            return Convert.ToInt16(-_random.Next(negativeMaximum));
        }

        private short GenerateNegativeInt16Value(DbaseField field)
        {
            return GenerateNegativeNullableInt16Value(field).GetValueOrDefault();
        }

        public int GenerateAcceptableValue(DbaseInt32 value)
        {
            var result = default(int);
            switch (_random.Next() % 2)
            {
                case 0:
                    result = GeneratePositiveInt32Value(value.Field);
                    break;
                case 1:
                    result = GenerateNegativeInt32Value(value.Field);
                    break;
            }

            return result;
        }

        public int? GenerateAcceptableValue(DbaseNullableInt32 value)
        {
            var result = default(int?);
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveNullableInt32Value(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeNullableInt32Value(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] Int32SupportedFieldTypes =
            {DbaseFieldType.Number, DbaseFieldType.Float};

        private int? GeneratePositiveNullableInt32Value(DbaseField field)
        {
            if (!Int32SupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var positiveMaximum =
                Convert.ToInt32(
                    Math.Min(
                        Math.Min(
                            Math.Pow(10, field.PositiveIntegerDigits.ToInt32()),
                            Math.Pow(10, DbaseInt32.MaximumIntegerDigits.ToInt32())
                        ),
                        int.MaxValue
                    )
                );
            return _random.Next(positiveMaximum);
        }

        private int GeneratePositiveInt32Value(DbaseField field)
        {
            return GeneratePositiveNullableInt32Value(field).GetValueOrDefault();
        }

        private int? GenerateNegativeNullableInt32Value(DbaseField field)
        {
            if (!Int32SupportedFieldTypes.Contains(field.FieldType))
            {
                return null;
            }

            var negativeMaximum =
                Convert.ToInt32(
                    Math.Min(
                        Math.Min(
                            Math.Pow(10, field.NegativeIntegerDigits.ToInt32()),
                            Math.Pow(10, DbaseInt32.MaximumIntegerDigits.ToInt32())
                        ),
                        int.MaxValue
                    )
                );
            return -_random.Next(negativeMaximum);
        }

        private int GenerateNegativeInt32Value(DbaseField field)
        {
            return GenerateNegativeNullableInt32Value(field).GetValueOrDefault();
        }

        private char[] GeneratePositiveNumber(DbaseIntegerDigits digits, DbaseDecimalCount decimals)
        {
            var data = new char[digits.ToInt32() + 1 + decimals.ToInt32()];
            for(var index = 0; index < digits.ToInt32(); index++)
            {
                data[index] = _random.Next(0, 10).ToString()[0];
            }
            data[digits.ToInt32()] = '.';
            for(var index = 0; index < decimals.ToInt32(); index++)
            {
                data[digits.ToInt32() + 1 + index] = _random.Next(0, 10).ToString()[0];
            }
            return data;
        }

        private char[] GenerateNegativeNumber(DbaseIntegerDigits digits, DbaseDecimalCount decimals)
        {
            var data = new char[1 + digits.ToInt32() + 1 + decimals.ToInt32()];
            data[0] = '-';
            for(var index = 0; index < digits.ToInt32(); index++)
            {
                data[1 + index] = _random.Next(0, 10).ToString()[0];
            }
            data[1 + digits.ToInt32()] = '.';
            for(var index = 0; index < decimals.ToInt32(); index++)
            {
                data[1 + digits.ToInt32() + 1 + index] = _random.Next(0, 10).ToString()[0];
            }
            return data;
        }
    }
}
