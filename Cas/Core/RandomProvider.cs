using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static int Next(int max)
        {
            return random.Next(max);
        }

        public static int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

    }
}
