namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class ByteLengthTests
    {
        private readonly Fixture _fixture;

        public ByteLengthTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeByteLength();
            _fixture.CustomizeWordLength();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ValueCanNotBeNegative(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteLength(value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void ValueCanNotBeOdd(int value)
        {
            Assert.Throws<ArgumentException>(() => new ByteLength(value));
        }

        [Fact]
        public void ToInt32ReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsByteLengthValue();
            var sut = new ByteLength(value);

            var result = sut.ToInt32();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToInt32ReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<int>().AsByteLengthValue();
        //     var sut = new ByteLength(value);

        //     int result = sut;

        //     Assert.Equal(value, result);
        // }

        [Fact]
        public void PlusByteLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<ByteLength>();
            var sut = _fixture.Create<ByteLength>();

            var result = sut.Plus(value);

            Assert.Equal(new ByteLength(value.ToInt32() + sut.ToInt32()), result);
        }

        [Fact]
        public void PlusWordLengthReturnsExpectedValue()
        {
            var value = _fixture.Create<WordLength>();
            var sut = _fixture.Create<ByteLength>();

            var result = sut.Plus(value);

            Assert.Equal(new ByteLength(value.ToInt32() * 2 + sut.ToInt32()), result);
        }

        [Fact]
        public void ToWordLengthReturnsExpectedValue()
        {
            var sut = _fixture.Create<ByteLength>();

            var result = sut.ToWordLength();

            Assert.Equal(new WordLength(sut.ToInt32() / 2), result);
        }


        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<int>().AsByteLengthValue();
            var sut = new ByteLength(value);

            var result = sut.ToString();

            Assert.Equal(value.ToString(), result);
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
            ).Verify(typeof(ByteLength));
        }

        [Fact]
        public void IsEquatableToByteLength()
        {
            Assert.IsAssignableFrom<IEquatable<ByteLength>>(_fixture.Create<ByteLength>());
        }
    }
}
