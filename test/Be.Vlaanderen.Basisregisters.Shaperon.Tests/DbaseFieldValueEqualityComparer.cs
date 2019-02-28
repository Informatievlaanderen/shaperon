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
                return left.Field.Equals(right.Field);
            }
            var selector = new ComparerSelectingInspector(left.Field.DecimalCount);
            left.Inspect(selector);
            return selector.Comparer.Equals(left, right);
        }

        public int GetHashCode(DbaseFieldValue instance)
        {
            var inspector = new HashCodeInspector();
            instance.Inspect(inspector);
            return instance.Field.GetHashCode() ^ inspector.HashCode;
        }

        private class DelegatingDbaseFieldValueEqualityComparer<TDbaseFieldValue> : IEqualityComparer<object>
            where TDbaseFieldValue : DbaseFieldValue
        {
            private readonly IEqualityComparer<TDbaseFieldValue> _comparer;

            public DelegatingDbaseFieldValueEqualityComparer(IEqualityComparer<TDbaseFieldValue> comparer)
            {
                _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            public bool Equals(object left, object right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left is TDbaseFieldValue leftValue && right is TDbaseFieldValue rightValue &&
                       _comparer.Equals(leftValue, rightValue);
            }

            public int GetHashCode(object obj)
            {
                return obj is TDbaseFieldValue instance ? _comparer.GetHashCode(instance) : 0;
            }
        }

        private class DbaseInt16EqualityComparer : IEqualityComparer<DbaseInt16>
        {
            public bool Equals(DbaseInt16 left, DbaseInt16 right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseInt16 obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseInt32EqualityComparer : IEqualityComparer<DbaseInt32>
        {
            public bool Equals(DbaseInt32 left, DbaseInt32 right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseInt32 obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseStringEqualityComparer : IEqualityComparer<DbaseString>
        {
            public bool Equals(DbaseString left, DbaseString right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) &&
                       Equals(left.Value, right.Value);
            }

            public int GetHashCode(DbaseString obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value?.GetHashCode() ?? 0;
            }
        }

        private class DbaseBooleanEqualityComparer : IEqualityComparer<DbaseBoolean>
        {
            public bool Equals(DbaseBoolean left, DbaseBoolean right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseBoolean obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseDateTimeEqualityComparer : IEqualityComparer<DbaseDateTime>
        {
            public bool Equals(DbaseDateTime left, DbaseDateTime right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseDateTime obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseSingleEqualityComparer : IEqualityComparer<DbaseSingle>
        {
            private readonly DbaseDecimalCount _precision;

            public DbaseSingleEqualityComparer(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public bool Equals(DbaseSingle left, DbaseSingle right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                if (left.Value == null && right.Value == null) return left.Field.Equals(right.Field);
                if (left.Value == null || right.Value == null) return false;
                return left.Field.Equals(right.Field) &&
                       Math.Abs(left.Value.Value - right.Value.Value) < _precision.ToInt32();
            }

            public int GetHashCode(DbaseSingle obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseDoubleEqualityComparer : IEqualityComparer<DbaseDouble>
        {
            private readonly DbaseDecimalCount _precision;

            public DbaseDoubleEqualityComparer(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public bool Equals(DbaseDouble left, DbaseDouble right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                if (left.Value == null && right.Value == null) return left.Field.Equals(right.Field);
                if (left.Value == null || right.Value == null) return false;
                return left.Field.Equals(right.Field) &&
                       Math.Abs(left.Value.Value - right.Value.Value) < _precision.ToInt32();
            }

            public int GetHashCode(DbaseDouble obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseDecimalEqualityComparer : IEqualityComparer<DbaseDecimal>
        {
            private readonly DbaseDecimalCount _precision;

            public DbaseDecimalEqualityComparer(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public bool Equals(DbaseDecimal left, DbaseDecimal right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                if (left.Value == null && right.Value == null) return left.Field.Equals(right.Field);
                if (left.Value == null || right.Value == null) return false;
                return left.Field.Equals(right.Field) &&
                       Math.Abs(left.Value.Value - right.Value.Value) < _precision.ToInt32();
            }

            public int GetHashCode(DbaseDecimal obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class ComparerSelectingInspector : IDbaseFieldValueInspector
        {
            private readonly DbaseDecimalCount _precision;
            public IEqualityComparer<object> Comparer { get; private set; }

            public ComparerSelectingInspector(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public void Inspect(DbaseDateTime value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDateTime>(
                    new DbaseDateTimeEqualityComparer());
            }

            public void Inspect(DbaseDecimal value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDecimal>(
                    new DbaseDecimalEqualityComparer(_precision));
            }

            public void Inspect(DbaseDouble value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDouble>(
                    new DbaseDoubleEqualityComparer(_precision));
            }

            public void Inspect(DbaseSingle value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseSingle>(
                    new DbaseSingleEqualityComparer(_precision));
            }

            public void Inspect(DbaseInt16 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseInt16>(
                    new DbaseInt16EqualityComparer());
            }

            public void Inspect(DbaseInt32 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseInt32>(
                    new DbaseInt32EqualityComparer());
            }

            public void Inspect(DbaseString value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseString>(
                    new DbaseStringEqualityComparer());
            }

            public void Inspect(DbaseBoolean value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseBoolean>(
                    new DbaseBooleanEqualityComparer());
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
