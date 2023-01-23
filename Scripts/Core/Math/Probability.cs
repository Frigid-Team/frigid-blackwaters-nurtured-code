using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class Probability
    {
        public static int NormalDistribution(float mean, float stdDev, int min, int max)
        {
            float minfloat = min;
            float maxfloat = max;

            float u1 = maxfloat - Random.Range(minfloat, maxfloat);
            float u2 = maxfloat - Random.Range(minfloat, maxfloat);
            float randStdNormal = Mathf.Sqrt(2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
            float randNormal = mean + stdDev * randStdNormal;
            int finalValue = Mathf.RoundToInt(randNormal);

            return Mathf.Clamp(finalValue, min, max);
        }

        public static float NormalDistribution(float mean, float stdDev, float min, float max)
        {
            float u1 = max - Random.Range(min, max) + 1;
            float u2 = max - Random.Range(min, max);
            float randStdNormal = Mathf.Sqrt(2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
            float randNormal = mean + stdDev * randStdNormal;

            return Mathf.Clamp(randNormal, min, max);
        }
    }
}
