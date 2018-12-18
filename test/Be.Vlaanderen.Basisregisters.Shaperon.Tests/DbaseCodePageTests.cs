namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseCodePageTests
    {
        private readonly Fixture _fixture;

        public DbaseCodePageTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseCodePage();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void ParseValueMustBeSupported()
        {
            var allSupported = Array.ConvertAll(DbaseCodePage.All, _ => _.ToByte());
            var value = new Generator<byte>(_fixture)
                .First(candidate => !Array.Exists(allSupported, supported => supported == candidate));

            Assert.Throws<ArgumentException>(() => DbaseCodePage.Parse(value));
        }

        [Fact]
        public void TryParseWithUnsupportedValueReturnsExpectedResult()
        {
            var allSupported = Array.ConvertAll(DbaseCodePage.All, _ => _.ToByte());
            var value = new Generator<byte>(_fixture)
                .First(candidate => !Array.Exists(allSupported, supported => supported == candidate));

            var result = DbaseCodePage.TryParse(value, out DbaseCodePage parsed);
            Assert.False(result);
            Assert.Null(parsed);
        }

        [Fact]
        public void TryParseWithSupportedValueReturnsExpectedResult()
        {
            var supported = _fixture.Create<DbaseCodePage>();

            var result = DbaseCodePage.TryParse(supported.ToByte(), out DbaseCodePage parsed);
            Assert.True(result);
            Assert.Equal(supported, parsed);
        }

        [Fact]
        public void ToByteReturnsExpectedValue()
        {
            var value = _fixture.Create<byte>().AsDbaseCodePageValue();
            var sut = DbaseCodePage.Parse(value);

            var result = sut.ToByte();

            Assert.Equal(value, result);
        }

        // [Fact]
        // public void ImplicitConversionToByteReturnsExpectedValue()
        // {
        //     var value = _fixture.Create<byte>().AsDbaseCodePageValue();
        //     var sut = DbaseCodePage.Parse(value);

        //     byte result = sut;

        //     Assert.Equal((byte)value, result);
        // }

        [Fact]
        public void ToStringReturnsExpectedValue()
        {
            var value = _fixture.Create<byte>().AsDbaseCodePageValue();
            var sut = DbaseCodePage.Parse(value);

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
            ).Verify(typeof(DbaseCodePage));
        }

        [Fact]
        public void IsEquatableToDbaseCodePage()
        {
            Assert.IsAssignableFrom<IEquatable<DbaseCodePage>>(_fixture.Create<DbaseCodePage>());
        }
    }
}
