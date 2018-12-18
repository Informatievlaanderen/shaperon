namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;

    public class PooledRandom : Random, IDisposable
    {
        private static readonly RandomPool Pool = new RandomPool();

        private readonly Random _instance;
        private bool _disposed;

        public PooledRandom()
        {
            _instance = Pool.Take();
        }

        public PooledRandom(int seed)
        {
            _instance = Pool.Take(seed);
        }

        public override int Next() => _instance.Next();
        public override int Next(int maxValue) => _instance.Next(maxValue);
        public override int Next(int minValue, int maxValue) => _instance.Next(minValue, maxValue);

        public override void NextBytes(byte[] buffer) => _instance.NextBytes(buffer);
        public override void NextBytes(Span<byte> buffer) => _instance.NextBytes(buffer);

        public override double NextDouble() => _instance.NextDouble();

        public void Dispose()
        {
            if (!_disposed)
            {
                Pool.Return(_instance);
                _disposed = true;
            }
        }
    }
}
