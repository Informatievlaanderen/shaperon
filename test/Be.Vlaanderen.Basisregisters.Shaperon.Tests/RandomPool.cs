namespace Be.Vlaanderen.Basisregisters.Shaperon
{
    using System;
    using System.Collections.Concurrent;

    public class RandomPool
    {
        private ConcurrentStack<Random> _instances;

        public RandomPool()
        {
            _instances = new ConcurrentStack<Random>();
        }

        public Random Take()
        {
            if (_instances.TryPop(out Random result))
            {
                return result;
            }

            return new Random();
        }

        public Random Take(int seed)
        {
            if (_instances.TryPop(out Random result))
            {
                return result;
            }

            return new Random(seed);
        }

        public void Return(Random random)
        {
            _instances.Push(random);
        }
    }
}
