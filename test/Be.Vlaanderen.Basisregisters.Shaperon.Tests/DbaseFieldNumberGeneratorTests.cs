namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Globalization;
    using AutoFixture;
    using Xunit;
    using Xunit.Abstractions;

    public class DbaseFieldNumberGeneratorTests
    {
        private readonly Fixture _fixture;
        private readonly DbaseFieldNumberGenerator _sut;
        private readonly ITestOutputHelper _output;

        public DbaseFieldNumberGeneratorTests(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));

            _fixture = new Fixture();
            _fixture.CustomizeByteOffset();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseField();
            _fixture.CustomizeDbaseDouble();
            _fixture.CustomizeDbaseInt32();
            _fixture.CustomizeDbaseInt16();
            _fixture.CustomizeDbaseSingle();
            _fixture.CustomizeDbaseDecimal();

            _sut = new DbaseFieldNumberGenerator(new Random());
        }

        [Fact]
        public void GenerateAcceptableNullableDecimalReturnsExpectedResult()
        {
            var fieldValue = _fixture.Create<DbaseDecimal>();

            var value = _sut.GenerateAcceptableValue(fieldValue);

            _output.WriteLine(
                "Generated value {0} for field with length {1}, positive digits {2}, negative digits {3} and decimals {4}.",
                value.HasValue
                    ? value.Value.ToString("F", new NumberFormatInfo {NumberDecimalDigits = fieldValue.Field.DecimalCount.ToInt32(), NumberDecimalSeparator = "."})
                    : "null",
                fieldValue.Field.Length,
                fieldValue.Field.PositiveIntegerDigits,
                fieldValue.Field.NegativeIntegerDigits,
                fieldValue.Field.DecimalCount);

            Assert.True(fieldValue.AcceptsValue(value));
        }

        [Fact]
        public void GenerateAcceptableNullableDoubleReturnsExpectedResult()
        {
            var fieldValue = _fixture.Create<DbaseDouble>();

            var value = _sut.GenerateAcceptableValue(fieldValue);

            _output.WriteLine(
                "Generated value {0} for field with length {1}, positive digits {2}, negative digits {3} and decimals {4}.",
                value.HasValue
                    ? value.Value.ToString("F", new NumberFormatInfo {NumberDecimalDigits = fieldValue.Field.DecimalCount.ToInt32(), NumberDecimalSeparator = "."})
                    : "null",
                fieldValue.Field.Length,
                fieldValue.Field.PositiveIntegerDigits,
                fieldValue.Field.NegativeIntegerDigits,
                fieldValue.Field.DecimalCount);

            Assert.True(fieldValue.AcceptsValue(value));
        }

        [Fact]
        public void GenerateAcceptableNullableSingleReturnsExpectedResult()
        {
            var fieldValue = _fixture.Create<DbaseSingle>();

            var value = _sut.GenerateAcceptableValue(fieldValue);

            _output.WriteLine(
                "Generated value {0} for field with length {1}, positive digits {2}, negative digits {3} and decimals {4}.",
                value.HasValue
                    ? value.Value.ToString("0.0######", new NumberFormatInfo {NumberDecimalSeparator = "."})
                    : "null",
                fieldValue.Field.Length,
                fieldValue.Field.PositiveIntegerDigits,
                fieldValue.Field.NegativeIntegerDigits,
                fieldValue.Field.DecimalCount);

            Assert.True(fieldValue.AcceptsValue(value));
        }

        [Fact]
        public void GenerateAcceptableNullableInt32ReturnsExpectedResult()
        {
            var fieldValue = _fixture.Create<DbaseInt32>();

            var value = _sut.GenerateAcceptableValue(fieldValue);

            _output.WriteLine(
                "Generated value {0} for field with length {1}, positive digits {2}, negative digits {3} and decimals {4}.",
                value.HasValue
                    ? value.Value.ToString()
                    : "null",
                fieldValue.Field.Length,
                fieldValue.Field.PositiveIntegerDigits,
                fieldValue.Field.NegativeIntegerDigits,
                fieldValue.Field.DecimalCount);

            Assert.True(fieldValue.AcceptsValue(value));
        }

        [Fact]
        public void GenerateAcceptableNullableInt16ReturnsExpectedResult()
        {
            var fieldValue = _fixture.Create<DbaseInt16>();

            var value = _sut.GenerateAcceptableValue(fieldValue);

            _output.WriteLine(
                "Generated value {0} for field with length {1}, positive digits {2}, negative digits {3} and decimals {4}.",
                value.HasValue
                    ? value.Value.ToString()
                    : "null",
                fieldValue.Field.Length,
                fieldValue.Field.PositiveIntegerDigits,
                fieldValue.Field.NegativeIntegerDigits,
                fieldValue.Field.DecimalCount);

            Assert.True(fieldValue.AcceptsValue(value));
        }
    }
}
