namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class MeasureRangeTests
    {
        private readonly Fixture _fixture;

        public MeasureRangeTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void MinReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new MeasureRange(value, double.MaxValue);
            var result = sut.Min;
            Assert.Equal(value, result);
        }

        [Fact]
        public void MaxReturnsExpectedResult()
        {
            var value = _fixture.Create<double>();
            var sut = new MeasureRange(double.MinValue, value);
            var result = sut.Max;
            Assert.Equal(value, result);
        }

        [Fact]
        public void VerifyEquality()
        {
            new CompositeIdiomaticAssertion(
                new EqualsNewObjectAssertion(_fixture),
                new EqualsNullAssertion(_fixture),
                new EqualsSelfAssertion(_fixture),
                new EqualsSuccessiveAssertion(_fixture),
                new GetHashCodeSuccessiveAssertion(_fixture)
            ).Verify(typeof(MeasureRange));
        }

        [Fact]
        public void FromMeasuresMeasureCanNotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => MeasureRange.FromMeasures(null));
        }

        [Theory]
        [MemberData(nameof(FromMeasuresCases))]
        public void FromMeasuresReturnsExpectedResult(double[] values, MeasureRange expected)
        {
            var result = MeasureRange.FromMeasures(values);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> FromMeasuresCases
        {
            get
            {
                yield return new object[]
                {
                    new double[0],
                    MeasureRange.Empty
                };

                var fixture = new Fixture();
                var values = fixture.CreateMany<double>(new Random().Next(1, 10)).ToArray();
                yield return new object[]
                {
                    values,
                    new MeasureRange(values.Min(), values.Max())
                };
            }
        }
    }
}
