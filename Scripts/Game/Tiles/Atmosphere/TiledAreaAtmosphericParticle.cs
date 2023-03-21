using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaAtmosphericParticle : FrigidMonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private List<Sprite> animationSprites;
        [SerializeField]
        private AnimationCurve upwardVelocityOverTime;
        [SerializeField]
        private float animationDuration;
        [SerializeField]
        private Light2D light2D;
        [SerializeField]
        private float maxLightRadius;
        [SerializeField]
        private float minLightRadius;
        [SerializeField]
        List<Color> lightColours;

        public void RunParticle(Vector3 startPosition, Action onComplete)
        {
            this.transform.localPosition = startPosition;
            FrigidCoroutine.Run(ParticleLifetime(onComplete), this.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
            this.light2D.color = this.lightColours[UnityEngine.Random.Range(0, this.lightColours.Count)];
        }

        private IEnumerator<FrigidCoroutine.Delay> ParticleLifetime(Action onComplete)
        {
            float timeElapsed = 0;
            while (timeElapsed < this.animationDuration)
            {
                timeElapsed += FrigidCoroutine.DeltaTime;
                float percentComplete = timeElapsed / this.animationDuration;

                this.transform.localPosition += Vector3.up * FrigidCoroutine.DeltaTime * this.upwardVelocityOverTime.Evaluate(timeElapsed / this.animationDuration);
                this.spriteRenderer.sprite = this.animationSprites[Mathf.Clamp(Mathf.FloorToInt(this.animationSprites.Count * percentComplete), 0, this.animationSprites.Count - 1)];
                this.light2D.pointLightOuterRadius = this.minLightRadius + (this.maxLightRadius - this.minLightRadius) * (percentComplete < 0.5f ? percentComplete : 1 - percentComplete) / 0.5f;
                this.light2D.color = this.lightColours[UnityEngine.Random.Range(0, this.lightColours.Count)];
                yield return null;
            }
            onComplete?.Invoke();
        }
    }
}
