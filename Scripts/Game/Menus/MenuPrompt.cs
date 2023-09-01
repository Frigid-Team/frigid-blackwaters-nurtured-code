using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class MenuPrompt : FrigidMonoBehaviourWithUpdate
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private float fadeDuration;
        [SerializeField]
        private float bounceDistance;
        [SerializeField]
        private float bounceDuration;

        private bool shown;
        private FrigidCoroutine fadeRoutine;
        private Vector2 trackedPosition;

        public void ShowPrompt(Sprite icon, Vector2 trackedPosition)
        {
            this.spriteRenderer.sprite = icon;
            this.trackedPosition = trackedPosition;

            if (!this.shown)
            {
                FrigidCoroutine.Kill(this.fadeRoutine);
                this.fadeRoutine = FrigidCoroutine.Run(
                    Tween.Value(
                        this.fadeDuration * (1 - this.spriteRenderer.color.a), 
                        this.spriteRenderer.color.a,
                        1,
                        onValueUpdated: 
                        (float alpha) => 
                        { 
                            Color color = this.spriteRenderer.color;
                            color.a = alpha;
                            this.spriteRenderer.color = color;
                        }
                        ), 
                    this.gameObject
                    );
                this.shown = true;
            }
        }

        public void HidePrompt()
        {
            if (this.shown)
            {
                FrigidCoroutine.Kill(this.fadeRoutine);
                this.fadeRoutine = FrigidCoroutine.Run(
                    Tween.Value(
                        this.fadeDuration * this.spriteRenderer.color.a,
                        this.spriteRenderer.color.a,
                        0,
                        onValueUpdated:
                        (float alpha) =>
                        {
                            Color color = this.spriteRenderer.color;
                            color.a = alpha;
                            this.spriteRenderer.color = color;
                        }
                        ),
                    this.gameObject
                    );
                this.shown = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.shown = false;
        }

        protected override void Update()
        {
            base.Update();
            this.transform.position = 
                this.trackedPosition + 
                EasingFunctions.EaseOutQuart(0, 1, Mathf.PingPong(Time.time / this.bounceDuration, 1)) * Vector2.up * this.bounceDistance;
        }

#if UNITY_EDITOR
        protected override bool OwnsGameObject() { return true; }
#endif
    }
}
