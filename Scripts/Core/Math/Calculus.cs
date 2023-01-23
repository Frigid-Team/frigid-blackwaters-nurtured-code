using System;

namespace FrigidBlackwaters.Core
{
    public static class Calculus
    {
        public static float Integral(Func<float, float> f, float x1, float x2, int nSteps)
        {
            float h = (x2 - x1) / nSteps;
            float res = (f(x1) + f(x2)) / 2;
            for (int i = 1; i < nSteps; i++)
            {
                res += f(x1 + i * h);
            }
            return h * res;
        }
    }
}
