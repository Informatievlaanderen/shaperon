namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Albedo;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Xunit;

    public class DbaseCharacterTests
    {
        private readonly Fixture _fixture;

        public DbaseCharacterTests()
        {
            _fixture = new Fixture();
            _fixture.CustomizeDbaseFieldName();
            _fixture.CustomizeDbaseFieldLength();
            _fixture.CustomizeDbaseDecimalCount();
            _fixture.CustomizeDbaseString();
            _fixture.Register(() => new BinaryReader(new MemoryStream()));
            _fixture.Register(() => new BinaryWriter(new MemoryStream()));
        }

        [Fact]
        public void CreateFailsIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DbaseCharacter(null)
            );
        }

        [Fact]
        public void CreateFailsIfFieldIsNotCharacter()
        {
            var fieldType = new Generator<DbaseFieldType>(_fixture)
                .First(specimen => specimen != DbaseFieldType.Character);
            Assert.Throws<ArgumentException>(
                () =>
                    new DbaseCharacter(
                        new DbaseField(
                            _fixture.Create<DbaseFieldName>(),
                            fieldType,
                            _fixture.Create<ByteOffset>(),
                            _fixture.Create<DbaseFieldLength>(),
                            new DbaseDecimalCount(0)
                        )
                    )
            );
        }

        [Fact]
        public void IsDbaseFieldValue()
        {
            Assert.IsAssignableFrom<DbaseFieldValue>(_fixture.Create<DbaseCharacter>());
        }

        [Fact]
        public void ReaderCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseCharacter>().Select(instance => instance.Read(null)));
        }

        [Fact]
        public void WriterCanNotBeNull()
        {
            new GuardClauseAssertion(_fixture)
                .Verify(new Methods<DbaseCharacter>().Select(instance => instance.Write(null)));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CanReadWriteNullOrEmptyString(string value)
        {
            var sut = _fixture.Create<DbaseCharacter>();
            sut.Value = value;

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
                    var result = new DbaseCharacter(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWrite()
        {
            var sut = _fixture.Create<DbaseCharacter>();

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
                    var result = new DbaseCharacter(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanReadWriteNullString()
        {
            var sut = new DbaseCharacter(
                new DbaseField(
                    _fixture.Create<DbaseFieldName>(),
                    DbaseFieldType.Character,
                    ByteOffset.Initial,
                    new DbaseFieldLength(5),
                    new DbaseDecimalCount(0)
                ),
                null
            );

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
                    var result = new DbaseCharacter(sut.Field);
                    result.Read(reader);

                    Assert.Equal(sut.Field, result.Field);
                    Assert.Equal(sut.Value, result.Value);
                }
            }
        }

        [Fact]
        public void CanNotReadPastEndOfStream()
        {
            var sut = _fixture.Create<DbaseCharacter>();

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
                {
                    writer.Write(_fixture.CreateMany<byte>(new Random().Next(0, sut.Field.Length.ToInt32())).ToArray());
                    writer.Flush();
                }

                stream.Position = 0;

                using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
                {
                    var result = new DbaseCharacter(sut.Field);
                    Assert.Throws<EndOfStreamException>(() => result.Read(reader));
                }
            }
        }

        [Theory]
        [InlineData(10, "01234567890", false)]
        [InlineData(10, null, true)]
        [InlineData(10, "", true)]
        [InlineData(10, "0", true)]
        [InlineData(10, "012345678", true)]
        [InlineData(10, "0123456789", true)]
        public void AcceptsStringValueReturnsExpectedResult(int length, string value, bool accepted)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ));

            var result = sut.AcceptsValue(value);

            Assert.Equal(accepted, result);
        }

        [Theory]
        [MemberData(nameof(AcceptsNullableDateTimeValueCases))]
        public void AcceptsNullableDateTimeValueReturnsExpectedResult(int length, DbaseCharacterOptions options, DateTime? value, bool accepted)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.AcceptsValue(value);

            Assert.Equal(accepted, result);
        }

        public static IEnumerable<object[]> AcceptsNullableDateTimeValueCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    15,
                    null,
                    fixture.Create<DateTime?>(),
                    true
                };

                yield return new object[]
                {
                    14,
                    null,
                    fixture.Create<DateTime?>(),
                    false
                };

                yield return new object[]
                {
                    16,
                    null,
                    fixture.Create<DateTime?>(),
                    true
                };

                // custom options

                var custom = new DbaseCharacterOptions("yyyyMMdd", DbaseCharacterOptions.DefaultDateTimeOffsetFormat);

                yield return new object[]
                {
                    8,
                    custom,
                    fixture.Create<DateTime?>(),
                    true
                };

                yield return new object[]
                {
                    7,
                    custom,
                    fixture.Create<DateTime?>(),
                    false
                };

                yield return new object[]
                {
                    9,
                    custom,
                    fixture.Create<DateTime?>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(AcceptsNullableDateTimeOffsetValueCases))]
        public void AcceptsNullableDateTimeOffsetValueReturnsExpectedResult(int length, DbaseCharacterOptions options, DateTimeOffset? value, bool accepted)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.AcceptsValue(value);

            Assert.Equal(accepted, result);
        }

        public static IEnumerable<object[]> AcceptsNullableDateTimeOffsetValueCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };

                yield return new object[]
                {
                    24,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    false
                };

                yield return new object[]
                {
                    26,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };

                // custom options

                var custom = new DbaseCharacterOptions(DbaseCharacterOptions.DefaultDateTimeFormat, "yyyyMMdd");

                yield return new object[]
                {
                    8,
                    custom,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };

                yield return new object[]
                {
                    7,
                    custom,
                    fixture.Create<DateTimeOffset?>(),
                    false
                };

                yield return new object[]
                {
                    9,
                    custom,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(AcceptsDateTimeValueCases))]
        public void AcceptsDateTimeValueReturnsExpectedResult(int length, DbaseCharacterOptions options, DateTime value, bool accepted)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.AcceptsValue(value);

            Assert.Equal(accepted, result);
        }

        public static IEnumerable<object[]> AcceptsDateTimeValueCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    15,
                    null,
                    fixture.Create<DateTime>(),
                    true
                };

                yield return new object[]
                {
                    14,
                    null,
                    fixture.Create<DateTime>(),
                    false
                };

                yield return new object[]
                {
                    16,
                    null,
                    fixture.Create<DateTime>(),
                    true
                };

                // custom options

                var custom = new DbaseCharacterOptions("yyyyMMdd", DbaseCharacterOptions.DefaultDateTimeOffsetFormat);

                yield return new object[]
                {
                    8,
                    custom,
                    fixture.Create<DateTime>(),
                    true
                };

                yield return new object[]
                {
                    7,
                    custom,
                    fixture.Create<DateTime>(),
                    false
                };

                yield return new object[]
                {
                    9,
                    custom,
                    fixture.Create<DateTime>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(AcceptsDateTimeOffsetValueCases))]
        public void AcceptsDateTimeOffsetValueReturnsExpectedResult(int length, DbaseCharacterOptions options, DateTimeOffset value, bool accepted)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.AcceptsValue(value);

            Assert.Equal(accepted, result);
        }

        public static IEnumerable<object[]> AcceptsDateTimeOffsetValueCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    true
                };

                yield return new object[]
                {
                    24,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    false
                };

                yield return new object[]
                {
                    26,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    true
                };

                // custom options

                var custom = new DbaseCharacterOptions(DbaseCharacterOptions.DefaultDateTimeFormat, "yyyyMMdd");

                yield return new object[]
                {
                    8,
                    custom,
                    fixture.Create<DateTimeOffset>(),
                    true
                };

                yield return new object[]
                {
                    7,
                    custom,
                    fixture.Create<DateTimeOffset>(),
                    false
                };

                yield return new object[]
                {
                    9,
                    custom,
                    fixture.Create<DateTimeOffset>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsNullableDateTimeCases))]
        public void TryGetValueAsNullableDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options, string value, bool gotten, DateTime? gottenValueAsDateTime)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            var result = sut.TryGetValueAsNullableDateTime(out var valueAsDateTime);

            Assert.Equal(gotten, result);
            Assert.Equal(gottenValueAsDateTime, valueAsDateTime);
        }

        public static IEnumerable<object[]> TryGetValueAsNullableDateTimeCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    15,
                    null,
                    "20170101T010203",
                    true,
                    new DateTime?(new DateTime(2017, 1, 1, 1, 2, 3, DateTimeKind.Unspecified))
                };

                yield return new object[]
                {
                    15,
                    null,
                    null,
                    true,
                    new DateTime?()
                };

                yield return new object[]
                {
                    16,
                    null,
                    " 20170101T010203",
                    true,
                    new DateTime?(new DateTime(2017, 1, 1, 1, 2, 3, DateTimeKind.Unspecified))
                };

                yield return new object[]
                {
                    15,
                    null,
                    "",
                    false,
                    new DateTime?()
                };

                yield return new object[]
                {
                    15,
                    null,
                    "not-a-date-time",
                    false,
                    new DateTime?()
                };

                yield return new object[]
                {
                    15,
                    null,
                    "20170101",
                    false,
                    new DateTime?()
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsNullableDateTimeOffsetCases))]
        public void TryGetValueAsNullableDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options, string value, bool gotten, DateTimeOffset? gottenValueAsDateTimeOffset)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            var result = sut.TryGetValueAsNullableDateTimeOffset(out var valueAsDateTime);

            Assert.Equal(gotten, result);
            Assert.Equal(gottenValueAsDateTimeOffset, valueAsDateTime);
        }

        public static IEnumerable<object[]> TryGetValueAsNullableDateTimeOffsetCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    25,
                    null,
                    "2017-01-01T01:02:03+00:00",
                    true,
                    new DateTimeOffset?(new DateTimeOffset(2017, 1, 1, 1, 2, 3, TimeSpan.Zero))
                };

                yield return new object[]
                {
                    25,
                    null,
                    null,
                    true,
                    new DateTimeOffset?()
                };

                yield return new object[]
                {
                    26,
                    null,
                    " 2017-01-01T01:02:03+00:00",
                    true,
                    new DateTimeOffset?(new DateTimeOffset(2017, 1, 1, 1, 2, 3, TimeSpan.Zero))
                };

                yield return new object[]
                {
                    25,
                    null,
                    "",
                    false,
                    new DateTimeOffset?()
                };

                yield return new object[]
                {
                    25,
                    null,
                    "not-a-date-time",
                    false,
                    new DateTimeOffset?()
                };

                yield return new object[]
                {
                    25,
                    null,
                    "20170101",
                    false,
                    new DateTimeOffset?()
                };
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsNullableDateTimeCases))]
        public void TrySetValueAsNullableDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTime? value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.TrySetValueAsNullableDateTime(value);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TrySetValueAsNullableDateTimeCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    15,
                    null,
                    fixture.Create<DateTime?>(),
                    true
                };

                yield return new object[]
                {
                    15,
                    null,
                    new DateTime?(),
                    true
                };

                yield return new object[]
                {
                    16,
                    null,
                    fixture.Create<DateTime?>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsNullableDateTimeOffsetCases))]
        public void TrySetValueAsNullableDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTimeOffset? value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.TrySetValueAsNullableDateTimeOffset(value);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TrySetValueAsNullableDateTimeOffsetCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };

                yield return new object[]
                {
                    26,
                    null,
                    fixture.Create<DateTimeOffset?>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsNullableDateTimeCases))]
        public void GetValueAsNullableDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options, string value,
            bool gotten, DateTime? gottenValueAsDateTime)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            if (!gotten)
            {
                Assert.Throws<FormatException>(() =>
                {
                    var _ = sut.ValueAsNullableDateTime;
                });
            }
            else
            {
                var result = sut.ValueAsNullableDateTime;
                Assert.Equal(gottenValueAsDateTime, result);
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsNullableDateTimeCases))]
        public void SetValueAsNullableDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTime? value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            if (!expected)
            {
                Assert.Throws<FormatException>(() =>
                {
                    sut.ValueAsNullableDateTime = value;
                });
            }
            else
            {
                sut.ValueAsNullableDateTime = value;
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsNullableDateTimeOffsetCases))]
        public void GetValueAsNullableDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            string value, bool gotten, DateTimeOffset? gottenValueAsDateTimeOffset)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            if (!gotten)
            {
                Assert.Throws<FormatException>(() =>
                {
                    var _ = sut.ValueAsNullableDateTimeOffset;
                });
            }
            else
            {
                var result = sut.ValueAsNullableDateTimeOffset;
                Assert.Equal(gottenValueAsDateTimeOffset, result);
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsNullableDateTimeOffsetCases))]
        public void SetValueAsNullableDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTimeOffset? value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            if (!expected)
            {
                Assert.Throws<FormatException>(() =>
                {
                    sut.ValueAsNullableDateTimeOffset = value;
                });
            }
            else
            {
                sut.ValueAsNullableDateTimeOffset = value;
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsDateTimeCases))]
        public void TryGetValueAsDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options, string value, bool gotten, DateTime gottenValueAsDateTime)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            var result = sut.TryGetValueAsDateTime(out var valueAsDateTime);

            Assert.Equal(gotten, result);
            Assert.Equal(gottenValueAsDateTime, valueAsDateTime);
        }

        public static IEnumerable<object[]> TryGetValueAsDateTimeCases
        {
            get
            {
                // default options

                yield return new object[]
                {
                    15,
                    null,
                    "20170101T010203",
                    true,
                    new DateTime(2017, 1, 1, 1, 2, 3, DateTimeKind.Unspecified)
                };

                yield return new object[]
                {
                    15,
                    null,
                    null,
                    false,
                    default(DateTime)
                };

                yield return new object[]
                {
                    16,
                    null,
                    " 20170101T010203",
                    true,
                    new DateTime(2017, 1, 1, 1, 2, 3, DateTimeKind.Unspecified)
                };

                yield return new object[]
                {
                    15,
                    null,
                    "",
                    false,
                    new DateTime()
                };

                yield return new object[]
                {
                    15,
                    null,
                    "not-a-date-time",
                    false,
                    new DateTime()
                };

                yield return new object[]
                {
                    15,
                    null,
                    "20170101",
                    false,
                    new DateTime()
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsDateTimeOffsetCases))]
        public void TryGetValueAsDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options, string value, bool gotten, DateTimeOffset gottenValueAsDateTimeOffset)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            var result = sut.TryGetValueAsDateTimeOffset(out var valueAsDateTime);

            Assert.Equal(gotten, result);
            Assert.Equal(gottenValueAsDateTimeOffset, valueAsDateTime);
        }

        public static IEnumerable<object[]> TryGetValueAsDateTimeOffsetCases
        {
            get
            {
                // default options

                yield return new object[]
                {
                    25,
                    null,
                    "2017-01-01T01:02:03+00:00",
                    true,
                    new DateTimeOffset(2017, 1, 1, 1, 2, 3, TimeSpan.Zero)
                };

                yield return new object[]
                {
                    25,
                    null,
                    null,
                    false,
                    default(DateTimeOffset)
                };

                yield return new object[]
                {
                    26,
                    null,
                    " 2017-01-01T01:02:03+00:00",
                    true,
                    new DateTimeOffset(2017, 1, 1, 1, 2, 3, TimeSpan.Zero)
                };

                yield return new object[]
                {
                    25,
                    null,
                    "",
                    false,
                    new DateTimeOffset()
                };

                yield return new object[]
                {
                    25,
                    null,
                    "not-a-date-time",
                    false,
                    new DateTimeOffset()
                };

                yield return new object[]
                {
                    25,
                    null,
                    "20170101",
                    false,
                    new DateTimeOffset()
                };
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsDateTimeCases))]
        public void TrySetValueAsDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTime value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.TrySetValueAsDateTime(value);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TrySetValueAsDateTimeCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    15,
                    null,
                    fixture.Create<DateTime>(),
                    true
                };

                yield return new object[]
                {
                    15,
                    null,
                    new DateTime(),
                    true
                };

                yield return new object[]
                {
                    16,
                    null,
                    fixture.Create<DateTime>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsDateTimeOffsetCases))]
        public void TrySetValueAsDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTimeOffset value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            var result = sut.TrySetValueAsDateTimeOffset(value);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TrySetValueAsDateTimeOffsetCases
        {
            get
            {
                var fixture = new Fixture();

                // default options

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    true
                };

                yield return new object[]
                {
                    25,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    true
                };

                yield return new object[]
                {
                    26,
                    null,
                    fixture.Create<DateTimeOffset>(),
                    true
                };
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsDateTimeCases))]
        public void GetValueAsDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options, string value,
            bool gotten, DateTime gottenValueAsDateTime)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            if (!gotten)
            {
                Assert.Throws<FormatException>(() =>
                {
                    var _ = sut.ValueAsDateTime;
                });
            }
            else
            {
                var result = sut.ValueAsDateTime;
                Assert.Equal(gottenValueAsDateTime, result);
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsDateTimeCases))]
        public void SetValueAsDateTimeReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTime value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            if (!expected)
            {
                Assert.Throws<FormatException>(() =>
                {
                    sut.ValueAsDateTime = value;
                });
            }
            else
            {
                sut.ValueAsDateTime = value;
            }
        }

        [Theory]
        [MemberData(nameof(TryGetValueAsDateTimeOffsetCases))]
        public void GetValueAsDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            string value, bool gotten, DateTimeOffset gottenValueAsDateTimeOffset)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), value, options);

            if (!gotten)
            {
                Assert.Throws<FormatException>(() =>
                {
                    var _ = sut.ValueAsDateTimeOffset;
                });
            }
            else
            {
                var result = sut.ValueAsDateTimeOffset;
                Assert.Equal(gottenValueAsDateTimeOffset, result);
            }
        }

        [Theory]
        [MemberData(nameof(TrySetValueAsDateTimeOffsetCases))]
        public void SetValueAsDateTimeOffsetReturnsExpectedResult(int length, DbaseCharacterOptions options,
            DateTimeOffset value, bool expected)
        {
            var sut = new DbaseCharacter(
                DbaseField.CreateCharacterField(
                    _fixture.Create<DbaseFieldName>(),
                    new DbaseFieldLength(length)
                ), options: options);

            if (!expected)
            {
                Assert.Throws<FormatException>(() =>
                {
                    sut.ValueAsDateTimeOffset = value;
                });
            }
            else
            {
                sut.ValueAsDateTimeOffset = value;
            }
        }
    }
}
