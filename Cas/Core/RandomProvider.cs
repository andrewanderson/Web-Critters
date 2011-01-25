using System;

namespace Cas.Core
{
    /// <summary>
    /// A central provider of random numbers for the application.
    /// </summary>
    /// <remarks>
    /// This just wraps up the Random class and allows me to use one generator
    /// instead of holding onto many (which may have seed collisions).
    /// </remarks>
    public static class RandomProvider
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static int Next(int max)
        {
            lock (syncLock)
            {
                return random.Next(max);
            }
        }

        public static int Next(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }

        public static double NextDouble()
        {
            lock (syncLock)
            {
                return random.NextDouble();
            }
        }

    }
}
