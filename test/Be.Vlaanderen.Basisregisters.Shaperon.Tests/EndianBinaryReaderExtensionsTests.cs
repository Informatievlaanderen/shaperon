namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class EndianBinaryReaderExtensionsTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ReadInt32BigEndianThrowsWhenReadingPastEndOfStream(int length)
        {
            using (var stream = CreateStreamOfLength(length))
            {
                using (var sut = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var exception = Assert.Throws<EndOfStreamException>(() => sut.ReadInt32BigEndian());
                    Assert.Equal(
                        $"Unable to read beyond the end of the stream. Expected to be able to read 4 bytes but could only read {length} bytes.",
                        exception.Message);
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ReadInt32LittleEndianThrowsWhenReadingPastEndOfStream(int length)
        {
            using (var stream = CreateStreamOfLength(length))
            {
                using (var sut = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var exception = Assert.Throws<EndOfStreamException>(() => sut.ReadInt32LittleEndian());
                    Assert.Equal(
                        $"Unable to read beyond the end of the stream. Expected to be able to read 4 bytes but could only read {length} bytes.",
                        exception.Message);
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void ReadDoubleBigEndianThrowsWhenReadingPastEndOfStream(int length)
        {
            using (var stream = CreateStreamOfLength(length))
            {
                using (var sut = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var exception = Assert.Throws<EndOfStreamException>(() => sut.ReadDoubleBigEndian());
                    Assert.Equal(
                        $"Unable to read beyond the end of the stream. Expected to be able to read 8 bytes but could only read {length} bytes.",
                        exception.Message);
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void ReadDoubleLittleEndianThrowsWhenReadingPastEndOfStream(int length)
        {
            using (var stream = CreateStreamOfLength(length))
            {
                using (var sut = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    var exception = Assert.Throws<EndOfStreamException>(() => sut.ReadDoubleLittleEndian());
                    Assert.Equal(
                        $"Unable to read beyond the end of the stream. Expected to be able to read 8 bytes but could only read {length} bytes.",
                        exception.Message);
                }
            }
        }

        private static MemoryStream CreateStreamOfLength(int length)
        {
            var random = new Random();
            var bytes = Enumerable
                .Range(0, length)
                .Select(_ => (byte) random.Next(0, 256))
                .ToArray();
            return new MemoryStream(bytes);
        }
    }
}
