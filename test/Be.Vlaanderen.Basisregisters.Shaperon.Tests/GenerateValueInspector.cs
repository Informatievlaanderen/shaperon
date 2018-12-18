namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Linq;
    using AutoFixture;

    public class GenerateValueInspector : IDbaseFieldValueInspector
    {
        private readonly IFixture _fixture;
        private readonly Random _random;
        private readonly DbaseFieldNumberGenerator _generator;


        public GenerateValueInspector(IFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _random = new Random();
            _generator = new DbaseFieldNumberGenerator(_random);
        }

        public void Inspect(DbaseDateTime value)
        {
            value.Value = _fixture.Create<DateTime?>();
        }

        public void Inspect(DbaseDecimal value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Inspect(DbaseDouble value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Inspect(DbaseSingle value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Inspect(DbaseInt16 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Inspect(DbaseInt32 value)
        {
            value.Value = _generator.GenerateAcceptableValue(value);
        }

        public void Inspect(DbaseString value)
        {
            var length = new Generator<DbaseFieldLength>(_fixture)
                .First(specimen => specimen <= value.Field.Length);

            switch (_random.Next() % 2)
            {
                case 0:
                    value.Value = null;
                    break;
                case 1:
                    if (value.Field.Length != new DbaseFieldLength(0))
                    {
                        var generated = _fixture.Create<string>();
                        value.Value = generated.Substring(0, Math.Min(generated.Length, length.ToInt32()));
                    }
                    else
                    {
                        value.Value = string.Empty;
                    }

                    break;
            }
        }

        public void Inspect(DbaseBoolean value)
        {
            value.Value = _fixture.Create<bool?>();
        }
    }
}
