using System;
using System.Collections.Generic;
using System.Text;

namespace FastPoisson
{
    internal static class RandomExtensions
    {
        public static float NextFloat(this Random random, float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
    }
}
