using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Core
{
    public static class TweenCoroutine
    {
        public static IEnumerator<FrigidCoroutine.Delay> DelayedCall(float duration, Action onComplete, bool useRealTime = false)
        {
            return Value(duration, useRealTime: useRealTime, onComplete: onComplete);
        }

        public static IEnumerator<FrigidCoroutine.Delay> Value(
            float iterationDuration,
            Vector2 from,
            Vector2 to,
            EasingType function = EasingType.Linear,
            bool loopInfinitely = false,
            int numberIterations = 1,
            float durationBetweenIterations = 0,
            float initialElapsedDuration = 0,
            bool pingPong = false,
            bool useRealTime = false,
            Action<Vector2> onValueUpdated = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return Value(
                iterationDuration,
                function,
                loopInfinitely,
                numberIterations,
                durationBetweenIterations,
                initialElapsedDuration,
                pingPong,
                useRealTime,
                (float val) => onValueUpdated?.Invoke(from + (to - from) * val),
                onIterationBegin,
                onIterationComplete,
                onComplete
                );
        }

        public static IEnumerator<FrigidCoroutine.Delay> Value(
            float iterationDuration,
            Color from,
            Color to,
            EasingType function = EasingType.Linear,
            bool loopInfinitely = false,
            int numberIterations = 1,
            float durationBetweenIterations = 0,
            float initialElapsedDuration = 0,
            bool pingPong = false,
            bool useRealTime = false,
            Action<Color> onValueUpdated = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return Value(
                iterationDuration,
                function,
                loopInfinitely,
                numberIterations,
                durationBetweenIterations,
                initialElapsedDuration,
                pingPong,
                useRealTime,
                (float val) => onValueUpdated?.Invoke(from + (to - from) * val),
                onIterationBegin,
                onIterationComplete,
                onComplete
                );
        }

        public static IEnumerator<FrigidCoroutine.Delay> Value(
            float iterationDuration,
            int from,
            int to,
            EasingType function = EasingType.Linear,
            bool loopInfinitely = false,
            int numberIterations = 1,
            float durationBetweenIterations = 0,
            float initialElapsedDuration = 0,
            bool pingPong = false,
            bool useRealTime = false,
            Action<int> onValueUpdated = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return Value(
                iterationDuration,
                function,
                loopInfinitely,
                numberIterations,
                durationBetweenIterations,
                initialElapsedDuration,
                pingPong,
                useRealTime,
                (float val) => onValueUpdated?.Invoke(from + Mathf.FloorToInt((to - from) * val)),
                onIterationBegin,
                onIterationComplete,
                onComplete
                );
        }

        public static IEnumerator<FrigidCoroutine.Delay> Value(
            float iterationDuration,
            float from,
            float to,
            EasingType function = EasingType.Linear,
            bool loopInfinitely = false,
            int numberIterations = 1,
            float durationBetweenIterations = 0,
            float initialElapsedDuration = 0,
            bool pingPong = false,
            bool useRealTime = false,
            Action<float> onValueUpdated = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            return Value(
                iterationDuration,
                function,
                loopInfinitely,
                numberIterations,
                durationBetweenIterations,
                initialElapsedDuration,
                pingPong,
                useRealTime,
                (float val) => onValueUpdated?.Invoke(from + (to - from) * val),
                onIterationBegin,
                onIterationComplete,
                onComplete
                );
        }

        public static IEnumerator<FrigidCoroutine.Delay> Value(
            float iterationDuration,
            EasingType function = EasingType.Linear,
            bool loopInfinitely = false,
            int numberIterations = 1,
            float durationBetweenIterations = 0,
            float initialElapsedDuration = 0,
            bool pingPong = false,
            bool useRealTime = false,
            Action<float> onUpdate = null,
            Action onIterationBegin = null,
            Action onIterationComplete = null,
            Action onComplete = null
            )
        {
            if (initialElapsedDuration < 0)
            {
                if (!useRealTime)
                {
                    yield return new FrigidCoroutine.DelayForSeconds(Mathf.Abs(initialElapsedDuration));
                }
                else
                {
                    yield return new FrigidCoroutine.DelayForSecondsRealTime(Mathf.Abs(initialElapsedDuration));
                }
                initialElapsedDuration = 0;
            }

            float elapsedDuration = initialElapsedDuration;
            Func<float, float, float, float> tweenFunc = EasingFunctions.GetFunc(function);

            for (int i = 0; loopInfinitely || i < numberIterations; i++)
            {
                onIterationBegin?.Invoke();

                if (elapsedDuration < iterationDuration)
                {
                    onUpdate?.Invoke(elapsedDuration / iterationDuration);
                    while (elapsedDuration < iterationDuration)
                    {
                        elapsedDuration += useRealTime ? FrigidCoroutine.UnscaledDeltaTime : FrigidCoroutine.DeltaTime;
                        onUpdate?.Invoke(tweenFunc.Invoke(0, 1, Mathf.Clamp01(elapsedDuration / iterationDuration)));
                        yield return null;
                    }
                }

                onUpdate?.Invoke(1);

                if (pingPong)
                {
                    elapsedDuration = iterationDuration;
                    while (elapsedDuration > 0)
                    {
                        elapsedDuration -= useRealTime ? FrigidCoroutine.UnscaledDeltaTime : FrigidCoroutine.DeltaTime;
                        onUpdate?.Invoke(tweenFunc.Invoke(0, 1, Mathf.Clamp01(elapsedDuration / iterationDuration)));
                        yield return null;
                    }
                    onUpdate?.Invoke(0);
                }

                onIterationComplete?.Invoke();

                if (durationBetweenIterations > 0 && (loopInfinitely || i < numberIterations - 1))
                {
                    if (!useRealTime)
                    {
                        yield return new FrigidCoroutine.DelayForSeconds(durationBetweenIterations);
                    }
                    else
                    {
                        yield return new FrigidCoroutine.DelayForSecondsRealTime(durationBetweenIterations);
                    }
                }

                elapsedDuration = 0;
            }

            onComplete?.Invoke();
        }
    }
}
