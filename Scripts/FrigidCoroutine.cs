using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrigidBlackwaters
{
    public partial class FrigidCoroutine
    {
        private static float time;
        private static float unscaledTime;
        private static float deltaTime;
        private static float unscaledDeltaTime;

        private IEnumerator<Delay> enumerator;
        private GameObject targetObject;
        private int index;
        private bool updated;
        private bool running;
        private float localTime;
        private float localTimeUnscaled;
        private float localTimeScale;

        public static float Time
        {
            get
            {
                return time;
            }
        }

        public static float UnscaledTime
        {
            get
            {
                return unscaledTime;
            }
        }

        public static float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }

        public static float UnscaledDeltaTime
        {
            get
            {
                return unscaledDeltaTime;
            }
        }

        public FrigidCoroutine(IEnumerator<Delay> enumerator, GameObject targetObject, int index, float localTimeScale)
        {
            this.enumerator = enumerator;
            this.targetObject = targetObject;
            this.index = index;
            this.updated = false;
            this.running = true;
            this.localTime = 0f;
            this.localTimeUnscaled = 0f;
            this.localTimeScale = localTimeScale;
        }

        public float LocalTimeScale
        {
            get
            {
                return this.localTimeScale;
            }
            set
            {
                this.localTimeScale = value;
            }
        }

        public static FrigidCoroutine Run(IEnumerator<Delay> enumerator, GameObject targetObject = null, float localTimeScale = 1.0f)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return null;
            }
#endif
            return Updater.Instance.RunCoroutine(enumerator, targetObject, localTimeScale);
        }

        public static void Kill(FrigidCoroutine coroutine)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return;
            }
#endif
            Updater.Instance.KillCoroutine(coroutine);
        }

        public abstract class Delay
        {
            public abstract bool IsDelayed();

            public virtual void Init() { }
        }

        public class DelayForSeconds : Delay
        {
            private float finishTime;
            private float delayDuration;

            public DelayForSeconds(float delayDuration)
            {
                this.delayDuration = delayDuration;
            }

            public override bool IsDelayed()
            {
                return time < this.finishTime;
            }

            public override void Init()
            {
                base.Init();
                this.finishTime = time + this.delayDuration;
            }
        }

        public class DelayForSecondsRealTime : Delay
        {
            private float finishTime;
            private float delayDuration;

            public DelayForSecondsRealTime(float delayDuration)
            {
                this.delayDuration = delayDuration;
            }

            public override bool IsDelayed()
            {
                return unscaledTime < this.finishTime;
            }

            public override void Init()
            {
                base.Init();
                this.finishTime = unscaledTime + this.delayDuration;
            }
        }

        public class DelayWhile : Delay
        {
            private Func<bool> predicate;

            public DelayWhile(Func<bool> predicate)
            {
                this.predicate = predicate;
            }

            public override bool IsDelayed()
            {
                return this.predicate.Invoke();
            }
        }

        public class DelayUntil : Delay
        {
            private Func<bool> predicate;

            public DelayUntil(Func<bool> predicate)
            {
                this.predicate = predicate;
            }

            public override bool IsDelayed()
            {
                return !this.predicate.Invoke();
            }
        }
    }
}
