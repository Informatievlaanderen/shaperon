namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using AutoFixture;
    using Xunit;

    public class DbaseFileHeaderReadBehaviorTests
    {
        [Fact]
        public void CtorInitializedProperties()
        {
            var fixture = new Fixture();
            var value = fixture.Create<bool>();

            var sut = new DbaseFileHeaderReadBehavior(value);

            Assert.Equal(value, sut.IgnoreFieldOffset);
        }

        [Fact]
        public void DefaultReturnsExpectedResult()
        {
            var sut = DbaseFileHeaderReadBehavior.Default;

            Assert.False(sut.IgnoreFieldOffset);
        }
    }
}
