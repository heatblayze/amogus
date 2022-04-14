using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amogus
{
    internal static class RandomUtil
    {
        public static Random random { get; private set; }

        static RandomUtil()
        {
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public static double RandomDouble(double min, double max) => random.NextDouble() * (max - min) + min;
    }
}
