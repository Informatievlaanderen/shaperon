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
            var selector = new ComparerSelectingVisitor(left.Field.DecimalCount);
            left.Accept(selector);
            return selector.Comparer.Equals(left, right);
        }

        public int GetHashCode(DbaseFieldValue instance)
        {
            var visitor = new HashCodeVisitor();
            instance.Accept(visitor);
            return instance.Field.GetHashCode() ^ visitor.HashCode;
        }

        private class DelegatingDbaseFieldValueEqualityComparer<TDbaseFieldValue> : IEqualityComparer<object>
            where TDbaseFieldValue : DbaseFieldValue
        {
            private readonly IEqualityComparer<TDbaseFieldValue> _comparer;

            public DelegatingDbaseFieldValueEqualityComparer(IEqualityComparer<TDbaseFieldValue> comparer)
            {
                _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            bool IEqualityComparer<object>.Equals(object left, object right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left is TDbaseFieldValue leftValue && right is TDbaseFieldValue rightValue &&
                       _comparer.Equals(leftValue, rightValue);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
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

        private class DbaseNullableInt16EqualityComparer : IEqualityComparer<DbaseNullableInt16>
        {
            public bool Equals(DbaseNullableInt16 left, DbaseNullableInt16 right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseNullableInt16 obj)
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

        private class DbaseNullableInt32EqualityComparer : IEqualityComparer<DbaseNullableInt32>
        {
            public bool Equals(DbaseNullableInt32 left, DbaseNullableInt32 right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseNullableInt32 obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        public class DbaseStringEqualityComparer : IEqualityComparer<DbaseCharacter>
        {
            public bool Equals(DbaseCharacter left, DbaseCharacter right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) &&
                       Equals(left.Value, right.Value);
            }

            public int GetHashCode(DbaseCharacter obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value?.GetHashCode() ?? 0;
            }
        }

        private class DbaseLogicalEqualityComparer : IEqualityComparer<DbaseLogical>
        {
            public bool Equals(DbaseLogical left, DbaseLogical right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseLogical obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseDateEqualityComparer : IEqualityComparer<DbaseDate>
        {
            public bool Equals(DbaseDate left, DbaseDate right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                return left.Field.Equals(right.Field) && left.Value.Equals(right.Value);
            }

            public int GetHashCode(DbaseDate obj)
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

        private class DbaseSingleEqualityComparer : IEqualityComparer<DbaseFloat>
        {
            private readonly DbaseDecimalCount _precision;

            public DbaseSingleEqualityComparer(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public bool Equals(DbaseFloat left, DbaseFloat right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                if (left.Value == null && right.Value == null) return left.Field.Equals(right.Field);
                if (left.Value == null || right.Value == null) return false;
                return left.Field.Equals(right.Field) &&
                       Math.Abs(left.Value.Value - right.Value.Value) < Convert.ToSingle(Math.Pow(10, -_precision.ToInt32()));
            }

            public int GetHashCode(DbaseFloat obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class DbaseDoubleEqualityComparer : IEqualityComparer<DbaseNumber>
        {
            private readonly DbaseDecimalCount _precision;

            public DbaseDoubleEqualityComparer(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public bool Equals(DbaseNumber left, DbaseNumber right)
            {
                if (left == null && right == null) return true;
                if (left == null || right == null) return false;
                if (left.Value == null && right.Value == null) return left.Field.Equals(right.Field);
                if (left.Value == null || right.Value == null) return false;
                return left.Field.Equals(right.Field) &&
                       Math.Abs(left.Value.Value - right.Value.Value) < Math.Pow(10, -_precision.ToInt32());
            }

            public int GetHashCode(DbaseNumber obj)
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
                       Math.Abs(left.Value.Value - right.Value.Value) < Convert.ToDecimal(Math.Pow(10, -_precision.ToInt32()));
            }

            public int GetHashCode(DbaseDecimal obj)
            {
                return obj.Field.GetHashCode() ^ obj.Value.GetHashCode();
            }
        }

        private class ComparerSelectingVisitor : IDbaseFieldValueVisitor
        {
            private readonly DbaseDecimalCount _precision;
            public IEqualityComparer<object> Comparer { get; private set; }

            public ComparerSelectingVisitor(DbaseDecimalCount precision)
            {
                _precision = precision;
            }

            public void Visit(DbaseDate value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDate>(
                    new DbaseDateEqualityComparer());
            }

            public void Visit(DbaseDateTime value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDateTime>(
                    new DbaseDateTimeEqualityComparer());
            }

            public void Visit(DbaseDecimal value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseDecimal>(
                    new DbaseDecimalEqualityComparer(_precision));
            }

            public void Visit(DbaseNumber value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseNumber>(
                    new DbaseDoubleEqualityComparer(_precision));
            }

            public void Visit(DbaseFloat value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseFloat>(
                    new DbaseSingleEqualityComparer(_precision));
            }

            public void Visit(DbaseInt16 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseInt16>(
                    new DbaseInt16EqualityComparer());
            }

            public void Visit(DbaseInt32 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseInt32>(
                    new DbaseInt32EqualityComparer());
            }

            public void Visit(DbaseNullableInt16 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseNullableInt16>(
                    new DbaseNullableInt16EqualityComparer());
            }

            public void Visit(DbaseNullableInt32 value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseNullableInt32>(
                    new DbaseNullableInt32EqualityComparer());
            }

            public void Visit(DbaseCharacter value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseCharacter>(
                    new DbaseStringEqualityComparer());
            }

            public void Visit(DbaseLogical value)
            {
                Comparer = new DelegatingDbaseFieldValueEqualityComparer<DbaseLogical>(
                    new DbaseLogicalEqualityComparer());
            }
        }

        private class HashCodeVisitor : IDbaseFieldValueVisitor
        {
            public int HashCode { get; private set; }

            public void Visit(DbaseDate value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseDateTime value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseDecimal value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseNumber value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseFloat value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseInt16 value)
            {
                HashCode = value.Value.GetHashCode();
            }

            public void Visit(DbaseInt32 value)
            {
                HashCode = value.Value.GetHashCode();
            }

            public void Visit(DbaseNullableInt16 value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseNullableInt32 value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseCharacter value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }

            public void Visit(DbaseLogical value)
            {
                HashCode = value.Value?.GetHashCode() ?? 0;
            }
        }
    }
}
