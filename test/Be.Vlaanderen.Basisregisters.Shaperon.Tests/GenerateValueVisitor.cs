namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using AutoFixture;

    public class GenerateValueVisitor : ITypedDbaseFieldValueVisitor
    {
        private readonly IFixture _fixture;
        private readonly Random _random;
        private readonly DbaseFieldNumberGenerator _generator;


        public GenerateValueVisitor(IFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _random = new Random();
            _generator = new DbaseFieldNumberGenerator(_random);
        }

        public void Visit(DbaseDate value)
        {
            value.Value = _fixture.Create<DateTime?>();
        }

        public void Visit(DbaseDateTime value)
        {
            value.Value = _fixture.Create<DateTime>();
        }

        public void Visit(DbaseNullableDateTime value)
        {
            value.Value = _fixture.Create<DateTime?>();
        }

        public void Visit(DbaseDateTimeOffset value)
        {
            value.Value = _fixture.Create<DateTimeOffset>();
        }

        public void Visit(DbaseNullableDateTimeOffset value)
        {
            value.Value = _fixture.Create<DateTimeOffset?>();
        }

        public void Visit(DbaseDecimal value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNullableDecimal value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseDouble value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNullableDouble value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseSingle value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNullableSingle value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNumber value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseFloat value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseInt16 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNullableInt16 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseInt32 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseNullableInt32 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Visit(DbaseCharacter value)
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen <= value.Field.Length);

            switch (_random.Next() % 3)
            {
                case 0:
                    value.Value = null;
                    break;
                case 1:
                    var generated = _fixture.Create<string>();
                    value.Value = generated.Substring(0, Math.Min(generated.Length, length.ToInt32()));
                    break;
                case 2:
                    value.Value = string.Empty;
                    break;
            }
        }

        public void Visit(DbaseString value)
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen <= value.Field.Length);

            switch (_random.Next() % 3)
            {
                case 0:
                    value.Value = null;
                    break;
                case 1:
                    var generated = _fixture.Create<string>();
                    value.Value = generated.Substring(0, Math.Min(generated.Length, length.ToInt32()));
                    break;
                case 2:
                    value.Value = string.Empty;
                    break;
            }
        }

        public void Visit(DbaseLogical value)
        {
            value.Value = _fixture.Create<bool?>();
        }

        public void Visit(DbaseBoolean value)
        {
            value.Value = _fixture.Create<bool>();
        }

        public void Visit(DbaseNullableBoolean value)
        {
            value.Value = _fixture.Create<bool?>();
        }
    }
}
