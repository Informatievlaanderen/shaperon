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

        public decimal? GenerateAcceptableValue(DbaseDecimal value)
        {
            decimal? result = default;
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
                        result = GeneratePositiveDecimalValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveDecimalValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeDecimalValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] DecimalSupportedFieldTypes = new[] {DbaseFieldType.Number};

        private decimal? GeneratePositiveDecimalValue(DbaseField field)
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

        private decimal? GenerateNegativeDecimalValue(DbaseField field)
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
                        result = GeneratePositiveDoubleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveDoubleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeDoubleValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] DoubleSupportedFieldTypes = new[] {DbaseFieldType.Number};

        private double? GeneratePositiveDoubleValue(DbaseField field)
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

        private double? GenerateNegativeDoubleValue(DbaseField field)
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
                        result = GeneratePositiveSingleValue(value.Field);
                        break;
                }

                return result;
            }

            // null or positive or negative
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveSingleValue(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeSingleValue(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] SingleSupportedFieldTypes = new[] {DbaseFieldType.Float};

        private float? GeneratePositiveSingleValue(DbaseField field)
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

        private float? GenerateNegativeSingleValue(DbaseField field)
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

        public short? GenerateAcceptableValue(DbaseInt16 value)
        {
            var result = default(short?);
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveInt16Value(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeInt16Value(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] Int16SupportedFieldTypes =
            {DbaseFieldType.Number, DbaseFieldType.Float};

        private short? GeneratePositiveInt16Value(DbaseField field)
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

        private short? GenerateNegativeInt16Value(DbaseField field)
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

        public int? GenerateAcceptableValue(DbaseInt32 value)
        {
            var result = default(int?);
            switch (_random.Next() % 3)
            {
                //case 0: null
                case 1:
                    result = GeneratePositiveInt32Value(value.Field);
                    break;
                case 2:
                    result = GenerateNegativeInt32Value(value.Field);
                    break;
            }

            return result;
        }

        private static readonly DbaseFieldType[] Int32SupportedFieldTypes =
            {DbaseFieldType.Number, DbaseFieldType.Float};

        private int? GeneratePositiveInt32Value(DbaseField field)
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

        private int? GenerateNegativeInt32Value(DbaseField field)
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
