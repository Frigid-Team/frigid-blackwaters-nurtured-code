using System.Collections.Generic;
using System;
using UnityEngine;

namespace FrigidBlackwaters
{
    public class FrigidCoroutine
    {
        private IEnumerator<Delay> enumerator;
        private GameObject targetObject;
        private int index;
        private bool updated;
        private bool running;
        private float localTime;
        private float localTimeUnscaled;
        private bool paused;

        public FrigidCoroutine(IEnumerator<Delay> enumerator, GameObject targetObject, int index, bool paused)
        {
            this.enumerator = enumerator;
            this.targetObject = targetObject;
            this.index = index;
            this.updated = false;
            this.running = true;
            this.paused = paused;
        }

        public bool Paused
        {
            get
            {
                return this.paused;
            }
            set
            {
                this.paused = value;
            }
        }

        public static FrigidCoroutine Run(IEnumerator<Delay> enumerator, GameObject targetObject, bool initiallyPaused = false)
        {
            return Updater.Instance.RunCoroutine(enumerator, targetObject, initiallyPaused);
        }

        public static void Kill(FrigidCoroutine coroutine)
        {
            Updater.Instance.KillCoroutine(coroutine);
        }

        private bool ShouldRun
        {
            get
            {
                return !this.paused && this.targetObject.activeInHierarchy;
            }
        }

        public abstract class Delay
        {
            public abstract bool IsDelayed(float localTime, float localTimeUnscaled);

            public virtual void Init(float localTime, float localTimeUnscaled) { }
        }

        public class DelayForSeconds : Delay
        {
            private float finishTime;
            private float delayDuration;

            public DelayForSeconds(float delayDuration)
            {
                this.delayDuration = delayDuration;
            }

            public override bool IsDelayed(float localTime, float localTimeUnscaled)
            {
                return localTime < this.finishTime;
            }

            public override void Init(float localTime, float localTimeUnscaled)
            {
                base.Init(localTime, localTimeUnscaled);
                this.finishTime = localTime + this.delayDuration;
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

            public override bool IsDelayed(float localTime, float localTimeUnscaled)
            {
                return localTimeUnscaled < this.finishTime;
            }

            public override void Init(float localTime, float localTimeUnscaled)
            {
                base.Init(localTime, localTimeUnscaled);
                this.finishTime = localTimeUnscaled + this.delayDuration;
            }
        }

        public class DelayWhile : Delay
        {
            private Func<bool> predicate;

            public DelayWhile(Func<bool> predicate)
            {
                this.predicate = predicate;
            }

            public override bool IsDelayed(float localTime, float localTimeUnscaled)
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

            public override bool IsDelayed(float localTime, float localTimeUnscaled)
            {
                return !this.predicate.Invoke();
            }
        }

        private class Updater : MonoBehaviour
        {
            private const int INITIAL_BUFFER_SIZE = 256;
            private const float RESIZE_FACTOR = 1.4142f;
            private const short FRAMES_UNTIL_MAINTENANCE = 64;

            private static Updater instance;

            private FrigidCoroutine[] coroutines;
            private int nextRoutineIndex;
            private short numFramesSinceLastMaintenance;

            static Updater()
            {
                GameObject instanceObject = new GameObject();
                instance = instanceObject.AddComponent<Updater>();
                instanceObject.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(instanceObject);
            }

            public static Updater Instance
            {
                get
                {
                    return instance;
                }
            }

            public FrigidCoroutine RunCoroutine(IEnumerator<Delay> enumerator, GameObject targetObject, bool initiallyPaused)
            {
                if (enumerator == null) return null;

                if (this.nextRoutineIndex == this.coroutines.Length)
                {
                    FrigidCoroutine[] previousRoutines = this.coroutines;
                    this.coroutines = new FrigidCoroutine[Mathf.RoundToInt(this.coroutines.Length * RESIZE_FACTOR)];
                    for (int i = 0; i < this.nextRoutineIndex; i++)
                    {
                        this.coroutines[i] = previousRoutines[i];
                    }
                }

                FrigidCoroutine coroutine = new FrigidCoroutine(enumerator, targetObject, this.nextRoutineIndex, initiallyPaused);
                this.coroutines[coroutine.index] = coroutine;
                this.nextRoutineIndex++;

                if (!coroutine.targetObject || coroutine.ShouldRun && !coroutine.enumerator.MoveNext())
                {
                    this.coroutines[coroutine.index] = null;
                }

                coroutine.updated = true;
                coroutine.running = coroutine.ShouldRun;

                coroutine.localTime += Time.deltaTime;
                coroutine.localTimeUnscaled += Time.unscaledDeltaTime;

                coroutine.enumerator.Current?.Init(coroutine.localTime, coroutine.localTimeUnscaled);

                return coroutine;
            }

            public void KillCoroutine(FrigidCoroutine coroutine)
            {
                if (coroutine == null) return;
                if (coroutine.index < this.nextRoutineIndex && this.coroutines[coroutine.index] == coroutine)
                {
                    this.coroutines[coroutine.index] = null;
                }
            }

            private void Awake()
            {
                this.coroutines = new FrigidCoroutine[INITIAL_BUFFER_SIZE];
                this.nextRoutineIndex = 0;
                this.numFramesSinceLastMaintenance = 0;
#if UNITY_EDITOR
                this.hideFlags = HideFlags.HideInHierarchy;
#endif
            }

            private void Update()
            {
                for (int i = 0; i < this.nextRoutineIndex; i++)
                {
                    if (this.coroutines[i] == null)
                    {
                        continue;
                    }

                    if (!this.coroutines[i].targetObject)
                    {
                        this.coroutines[i] = null;
                        continue;
                    }

                    if (!this.coroutines[i].updated && this.coroutines[i].running && this.coroutines[i].ShouldRun)
                    {

                        this.coroutines[i].localTime += Time.deltaTime;
                        this.coroutines[i].localTimeUnscaled += Time.unscaledDeltaTime;

                        if (this.coroutines[i].enumerator.Current == null || 
                            !this.coroutines[i].enumerator.Current.IsDelayed(this.coroutines[i].localTime, this.coroutines[i].localTimeUnscaled))
                        {
                            if (!this.coroutines[i].enumerator.MoveNext())
                            {
                                this.coroutines[i] = null;
                                continue;
                            }

                            this.coroutines[i].enumerator.Current?.Init(this.coroutines[i].localTime, this.coroutines[i].localTimeUnscaled);
                        }
                    }
                }

                if (this.numFramesSinceLastMaintenance >= FRAMES_UNTIL_MAINTENANCE)
                {
                    int inner;
                    int outer;
                    for (inner = outer = 0; outer < this.nextRoutineIndex; outer++)
                    {
                        if (this.coroutines[outer] != null)
                        {
                            if (outer != inner)
                            {
                                this.coroutines[inner] = this.coroutines[outer];
                                this.coroutines[inner].index = inner;
                                this.coroutines[outer] = null;
                            }
                            inner++;
                        }
                    }
                    this.nextRoutineIndex = inner;

                    int resizeLength = Mathf.RoundToInt(this.coroutines.Length / (RESIZE_FACTOR * RESIZE_FACTOR));
                    if (resizeLength >= INITIAL_BUFFER_SIZE && this.nextRoutineIndex <= resizeLength)
                    {
                        FrigidCoroutine[] previousRoutines = this.coroutines;
                        this.coroutines = new FrigidCoroutine[resizeLength];
                        for (int i = 0; i < this.nextRoutineIndex; i++)
                        {
                            this.coroutines[i] = previousRoutines[i];
                        }
                    }

                    this.numFramesSinceLastMaintenance = 0;
                }
                else
                {
                    this.numFramesSinceLastMaintenance++;
                }
            }

            private void LateUpdate()
            {
                for (int i = 0; i < this.nextRoutineIndex; i++)
                {
                    if (this.coroutines[i] != null)
                    {
                        this.coroutines[i].updated = false;
                        this.coroutines[i].running = this.coroutines[i].ShouldRun;
                    }
                }
            }
        }
    }
}
