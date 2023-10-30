using System;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class Busy<B> : FrigidMonoBehaviour where B : Busy<B>
    {
        private static B instance;
        private static Action onStarted;
        private static Action onFinished;

        [SerializeField]
        private CanvasGroup overlayCanvasGroup;
        [SerializeField]
        private FloatSerializedReference fadeDuration;

        private FrigidCoroutine screenRoutine;
        private ControlCounter isBusy;
        private Action onFullyShown;

        public static Action OnStarted
        {
            get
            {
                return onStarted;
            }
            set
            {
                onStarted = value;
            }
        }

        public static Action OnFinished
        {
            get
            {
                return onFinished;
            }
            set
            {
                onFinished = value;
            }
        }

        public static void Request(Action toDoBusyWork)
        {
            if (instance.IsScreenFullyShown)
            {
                toDoBusyWork?.Invoke();
            }
            else
            {
                instance.onFullyShown += toDoBusyWork;
            }
            instance.isBusy.Request();
        }

        public static void Release()
        {
            instance.isBusy.Release();
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyInstanceOnLoad(this);
            instance = (B)this;
            this.isBusy = new ControlCounter();
            this.isBusy.OnFirstRequest += this.FirstRequested;
            this.isBusy.OnLastRelease += this.LastReleased;
        }

        protected abstract void Started();

        protected abstract void Finished();

        private bool IsScreenFullyShown
        {
            get
            {
                return this.overlayCanvasGroup.alpha >= 1;
            }
        }

        private bool IsScreenPartiallyShown
        {
            get
            {
                return this.overlayCanvasGroup.alpha > 0;
            }
        }

        private void FirstRequested()
        {
            if (!this.IsScreenPartiallyShown)
            {
                this.Started();
                onStarted?.Invoke();
            }
            FrigidCoroutine.Kill(this.screenRoutine);
            this.screenRoutine =
                FrigidCoroutine.Run(
                    Tween.Value(
                        this.fadeDuration.ImmutableValue * (1 - this.overlayCanvasGroup.alpha),
                        this.overlayCanvasGroup.alpha,
                        1,
                        useRealTime: true,
                        onValueUpdated: (float alpha) => { this.overlayCanvasGroup.alpha = alpha; },
                        onComplete: () => { this.onFullyShown?.Invoke(); this.onFullyShown = null; }
                        ),
                    this.gameObject
                    );
        }

        private void LastReleased()
        {
            FrigidCoroutine.Kill(this.screenRoutine);
            this.screenRoutine =
                FrigidCoroutine.Run(
                    Tween.Value(
                        this.fadeDuration.ImmutableValue * this.overlayCanvasGroup.alpha,
                        this.overlayCanvasGroup.alpha,
                        0,
                        useRealTime: true,
                        onValueUpdated: (float alpha) => { this.overlayCanvasGroup.alpha = alpha; },
                        onComplete: () =>
                        {
                            this.Finished();
                            onFinished?.Invoke();
                        }
                        ),
                    this.gameObject
                    );
        }
    }
}
