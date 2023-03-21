using System;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class EasingFunctions
    {
        public static Func<float, float, float, float> GetFunc(EasingType function)
        {
            switch (function)
            {
                case EasingType.Linear:
                    return Linear;
                case EasingType.EaseInQuad:
                    return EaseInQuad;
                case EasingType.EaseOutQuad:
                    return EaseOutQuad;
                case EasingType.EaseInOutQuad:
                    return EaseInOutQuad;
                case EasingType.EaseInCubic:
                    return EaseInCubic;
                case EasingType.EaseOutCubic:
                    return EaseOutCubic;
                case EasingType.EaseInOutCubic:
                    return EaseInOutCubic;
                case EasingType.EaseInQuart:
                    return EaseInQuart;
                case EasingType.EaseOutQuart:
                    return EaseOutQuart;
                case EasingType.EaseInOutQuart:
                    return EaseInOutQuart;
                case EasingType.EaseInQuint:
                    return EaseInQuint;
                case EasingType.EaseOutQuint:
                    return EaseOutQuint;
                case EasingType.EaseInOutQuint:
                    return EaseInOutQuint;
                case EasingType.EaseInSine:
                    return EaseInSine;
                case EasingType.EaseOutSine:
                    return EaseOutSine;
                case EasingType.EaseInOutSine:
                    return EaseInOutSine;
                case EasingType.EaseInExpo:
                    return EaseInExpo;
                case EasingType.EaseOutExpo:
                    return EaseOutExpo;
                case EasingType.EaseInOutExpo:
                    return EaseInOutExpo;
                case EasingType.EaseInCirc:
                    return EaseInCirc;
                case EasingType.EaseOutCirc:
                    return EaseOutCirc;
                case EasingType.EaseInOutCirc:
                    return EaseInOutCirc;
                case EasingType.EaseInBounce:
                    return EaseInBounce;
                case EasingType.EaseOutBounce:
                    return EaseOutBounce;
                case EasingType.EaseInOutBounce:
                    return EaseInOutBounce;
                case EasingType.EaseInBack:
                    return EaseInBack;
                case EasingType.EaseOutBack:
                    return EaseOutBack;
                case EasingType.EaseInOutBack:
                    return EaseInOutBack;
                case EasingType.EaseInElastic:
                    return EaseInElastic;
                case EasingType.EaseOutElastic:
                    return EaseOutElastic;
                case EasingType.EaseInOutElastic:
                    return EaseInOutElastic;
            }
            return Linear;
        }

        public static float Linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        public static float EaseInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        public static float EaseOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        public static float EaseOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        public static float EaseOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        public static float EaseOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        public static float EaseInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        public static float EaseOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        public static float EaseInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        public static float EaseInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        public static float EaseOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        public static float EaseInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        public static float EaseInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        public static float EaseOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        public static float EaseInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        public static float EaseInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - val) + start;
        }

        public static float EaseOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }

        public static float EaseInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return EaseInBounce(0, end, val * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public static float EaseInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        public static float EaseOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        public static float EaseInElastic(float start, float end, float val)
        {
            end -= start;

            float p = 0.3f;
            float a = 0f;

            if (val == 0f) return start;

            if (val == 1f) return start + end;

            float s;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4f;
            }
            else
            {
                s = p / (2f * Mathf.PI) * Mathf.Asin(end / a);
            }

            val = val - 1f;
            return start - (a * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val - s) * (2f * Mathf.PI) / p));
        }

        public static float EaseOutElastic(float start, float end, float val)
        {
            end -= start;

            float p = 0.3f;
            float a = 0f;

            if (val == 0f) return start;

            if (val == 1f) return start + end;

            float s;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4f;
            }
            else
            {
                s = p / (2f * Mathf.PI) * Mathf.Asin(end / a);
            }
            return start + end + a * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val - s) * (2f * Mathf.PI) / p);
        }

        public static float EaseInOutElastic(float start, float end, float val)
        {
            end -= start;

            float p = 0.3f;
            float a = 0f;

            if (val == 0f) return start;

            val = val / (1f / 2f);
            if (val == 2f) return start + end;

            float s;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4f;
            }
            else
            {
                s = p / (2f * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1f)
            {
                val = val - 1f;
                return start - 0.5f * (a * Mathf.Pow(2f, 10f * val) * Mathf.Sin((val - s) * (2f * Mathf.PI) / p));
            }
            val = val - 1f;
            return end + start + a * Mathf.Pow(2f, -10f * val) * Mathf.Sin((val - s) * (2f * Mathf.PI) / p) * 0.5f;
        }
    }
}


