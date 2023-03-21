using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FrigidBlackwaters
{
    public class FrigidCoroutine
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

        private class Updater : MonoBehaviour
        {
            private const int INITIAL_BUFFER_SIZE = 256;
            private const float RESIZE_FACTOR = 1.4142f;
            private const short FRAMES_UNTIL_MAINTENANCE = 64;

            private static Updater instance;

            private FrigidCoroutine[] coroutines;
            private int nextRoutineIndex;
            private short numFramesSinceLastMaintenance;

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
            public static void InitializeInstance()
            {
                if (instance == null)
                {
                    GameObject instanceObject = new GameObject();
                    instance = instanceObject.AddComponent<Updater>();
                    instanceObject.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(instanceObject);
                }
            }

            public static Updater Instance
            {
                get
                {
                    return instance;
                }
            }

            public FrigidCoroutine RunCoroutine(IEnumerator<Delay> enumerator, GameObject targetObject, float localTimeScale)
            {
                if (enumerator == null) return null;

                if (targetObject == null) targetObject = instance.gameObject;

                if (this.nextRoutineIndex == this.coroutines.Length)
                {
                    FrigidCoroutine[] previousRoutines = this.coroutines;
                    this.coroutines = new FrigidCoroutine[Mathf.RoundToInt(this.coroutines.Length * RESIZE_FACTOR)];
                    for (int i = 0; i < this.nextRoutineIndex; i++)
                    {
                        this.coroutines[i] = previousRoutines[i];
                    }
                }

                FrigidCoroutine coroutine = new FrigidCoroutine(enumerator, targetObject, this.nextRoutineIndex, localTimeScale);
                this.coroutines[coroutine.index] = coroutine;
                this.nextRoutineIndex++;

                deltaTime = UnityEngine.Time.deltaTime * coroutine.localTimeScale;
                unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime * coroutine.localTimeScale;
                time = coroutine.localTime;
                unscaledTime = coroutine.localTimeUnscaled;

                if (!coroutine.targetObject || coroutine.targetObject.activeInHierarchy && !coroutine.enumerator.MoveNext())
                {
                    this.coroutines[coroutine.index] = null;
                }

                coroutine.localTime += deltaTime;
                coroutine.localTimeUnscaled += unscaledDeltaTime;

                coroutine.updated = true;
                coroutine.running = coroutine.targetObject.activeInHierarchy;

                coroutine.enumerator.Current?.Init();

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

                    if (!this.coroutines[i].updated && this.coroutines[i].running && this.coroutines[i].targetObject.activeInHierarchy)
                    {
                        deltaTime = UnityEngine.Time.deltaTime * this.coroutines[i].localTimeScale;
                        unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime * this.coroutines[i].localTimeScale;
                        time = this.coroutines[i].localTime;
                        unscaledTime = this.coroutines[i].localTimeUnscaled;

                        if (this.coroutines[i].enumerator.Current == null || 
                            !this.coroutines[i].enumerator.Current.IsDelayed())
                        {
                            if (!this.coroutines[i].enumerator.MoveNext())
                            {
                                this.coroutines[i] = null;
                                continue;
                            }

                            deltaTime = UnityEngine.Time.deltaTime * this.coroutines[i].localTimeScale;
                            unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime * this.coroutines[i].localTimeScale;
                            time = this.coroutines[i].localTime;
                            unscaledTime = this.coroutines[i].localTimeUnscaled;

                            this.coroutines[i].enumerator.Current?.Init();
                        }

                        this.coroutines[i].localTime += deltaTime;
                        this.coroutines[i].localTimeUnscaled += unscaledDeltaTime;
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
                        this.coroutines[i].running = this.coroutines[i].targetObject.activeInHierarchy;
                    }
                }
            }
        }
    }
}
