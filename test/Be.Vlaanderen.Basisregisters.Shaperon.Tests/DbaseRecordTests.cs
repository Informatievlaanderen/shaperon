namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseRecordTests
    {
        private readonly Fixture _fixture;

        public DbaseRecordTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseField();
            _fixture.Customize<DbaseRecord>(customization =>
                customization.FromFactory<int>(value =>
                    new AnonymousDbaseRecord(_fixture.CreateMany<DbaseField>(new Random(value).Next(1, 50))
                        .ToArray())));
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseRecord>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseRecord>().Select(instance => instance.Write(null)));
        }


        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseRecord>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    sut.Write(writer);
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new AnonymousDbaseRecord(
                        sut.Values.Select(value => value.Field).ToArray()
                    );
                    result.Read(reader);

                    Assert.Equal(sut.IsDeleted, result.IsDeleted);
                    Assert.Equal(sut.Values, result.Values, new DbaseFieldValueEqualityComparer());
                }
            }
        }
    }
}
