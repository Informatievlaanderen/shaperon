namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;

    public class DbaseFieldValueEqualityComparer : IEqualityComparer<DbaseFieldValue>
    {
        public bool Equals(DbaseFieldValue left, DbaseFieldValue right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            var leftInspector = new ValueInspector();
            var rightInspector = new ValueInspector();
            left.Inspect(leftInspector);
            right.Inspect(rightInspector);
            var sameField = left.Field.Equals(right.Field);
            if (left.Field.Length.Equals(new DbaseFieldLength(15)) &&
                left.Field.DecimalCount.Equals(new DbaseDecimalCount(0)) &&
                right.Field.Length.Equals(new DbaseFieldLength(15)) &&
                right.Field.DecimalCount.Equals(new DbaseDecimalCount(0)) &&
                (
                    (
                        left.Field.FieldType == DbaseFieldType.Character
                        ||
                        left.Field.FieldType == DbaseFieldType.DateTime
                    )
                    &&
                    (
                        right.Field.FieldType == DbaseFieldType.Character
                        ||
                        right.Field.FieldType == DbaseFieldType.DateTime
                    )
                ) &&
                (
                    leftInspector.Value == null
                    ||
                    (leftInspector.Value is string leftString && leftString == "")
                ) &&
                (
                    rightInspector.Value == null
                    ||
                    (rightInspector.Value is string rightString && rightString == "")
                )
            )
            {
                return sameField;
            }

            IEqualityComparer<object> comparer = new ObjectEqualityComparer(left.Field.DecimalCount);
            var sameValue = comparer.Equals(leftInspector.Value, rightInspector.Value);
            return sameField && sameValue;
        }

        public int GetHashCode(DbaseFieldValue instance)
        {
            var inspector = new HashCodeInspector();
            instance.Inspect(inspector);
            return instance.Field.GetHashCode() ^ inspector.HashCode;
        }

        private class ObjectEqualityComparer : IEqualityComparer<object>
        {
            private readonly DbaseDecimalCount _digits;

            public ObjectEqualityComparer(DbaseDecimalCount digits)
            {
                _digits = digits;
            }

            bool IEqualityComparer<object>.Equals(object left, object right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;

                if (left is double leftDouble && right is double rightDouble)
                {
                    var doubleDigits = DbaseDecimalCount.Min(_digits, DbaseDouble.MaximumDecimalCount).ToInt32();
                    var leftRounded = Math.Round(leftDouble, doubleDigits);
                    var rightRounded = Math.Round(rightDouble, doubleDigits);
                    return Math.Abs(leftRounded - rightRounded) <= Math.Pow(10, -doubleDigits);
                }

                if (left is double? && right is double?)
                {
                    var nullableLeft = (double?) left;
                    var nullableRight = (double?) right;
                    if (!nullableLeft.HasValue && !nullableRight.HasValue) return true;
                    if (nullableLeft.HasValue || nullableRight.HasValue) return false;
                    var doubleDigits = DbaseDecimalCount.Min(_digits, DbaseDouble.MaximumDecimalCount).ToInt32();
                    var leftRounded = Math.Round((double) nullableLeft.Value, doubleDigits);
                    var rightRounded = Math.Round((double) nullableRight.Value, doubleDigits);
                    return Math.Abs(leftRounded - rightRounded) <= Math.Pow(10, -doubleDigits);
                }

                if (left is float leftSingle && right is float rightSingle)
                {
                    var singleDigits = DbaseDecimalCount.Min(_digits, DbaseSingle.MaximumDecimalCount).ToInt32();
                    var leftRounded = Math.Round(leftSingle, singleDigits);
                    var rightRounded = Math.Round(rightSingle, singleDigits);
                    return Math.Abs(leftRounded - rightRounded) <= Math.Pow(10, -singleDigits);
                }

                if (left is float? && right is float?)
                {
                    var nullableLeft = (float?) left;
                    var nullableRight = (float?) right;
                    if (!nullableLeft.HasValue && !nullableRight.HasValue) return true;
                    if (nullableLeft.HasValue || nullableRight.HasValue) return false;
                    var singleDigits = DbaseDecimalCount.Min(_digits, DbaseSingle.MaximumDecimalCount).ToInt32();
                    var leftRounded = Math.Round((float) nullableLeft.Value, singleDigits);
                    var rightRounded = Math.Round((float) nullableRight.Value, singleDigits);
                    return Math.Abs(leftRounded - rightRounded) <= Math.Pow(10, -singleDigits);
                }

                return Equals(left, right);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                throw new NotSupportedException();
            }
        }

        private class ValueInspector : IDbaseFieldValueInspector
        {
            public object Value { get; private set; }

            public void Inspect(DbaseDateTime value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseDecimal value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseDouble value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseSingle value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseInt16 value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseInt32 value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseString value)
            {
                Value = value.Value;
            }

            public void Inspect(DbaseBoolean value)
            {
                Value = value.Value;
            }
        }

        private class HashCodeInspector : IDbaseFieldValueInspector
        {
            public int HashCode { get; private set; }

            public void Inspect(DbaseDateTime value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseDecimal value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseDouble value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseSingle value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseInt16 value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseInt32 value)
            {
                HashCode = value.Value.HasValue
                    ? value.Value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseString value)
            {
                HashCode = value.Value != null
                    ? value.Value.GetHashCode()
                    : 0;
            }

            public void Inspect(DbaseBoolean value)
            {
                HashCode = value.Value != null
                    ? value.Value.GetHashCode()
                    : 0;
            }
        }
    }
}
