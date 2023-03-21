using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AfterImageSFX : SFX
    {
        [SerializeField]
        private ParticleSystem afterImageParticlePrefab;
        [SerializeField]
        private FloatSerializedReference spawnRateOverDistance;
        [SerializeField]
        private float offset;

        private FrigidCoroutine spawnAfterImagesRoutine;
        private RecyclePool<ParticleSystem> particlePool;

        protected override void Played(AnimatorBody animatorBody)
        {
            this.spawnAfterImagesRoutine = FrigidCoroutine.Run(SpawnAfterImages(animatorBody), this.gameObject);
        }

        protected override void Stopped() 
        {
            FrigidCoroutine.Kill(this.spawnAfterImagesRoutine);
        }

        protected override void Awake()
        {
            base.Awake();
            this.particlePool = new RecyclePool<ParticleSystem>(
                () => Object.Instantiate<ParticleSystem>(this.afterImageParticlePrefab), 
                (ParticleSystem particle) => Object.Destroy(particle)
                );
        }

        private IEnumerator<FrigidCoroutine.Delay> SpawnAfterImages(AnimatorBody animatorBody)
        {
            float distanceThreshold = 1f / Mathf.Max(GameConstants.SMALLEST_TIME_INTERVAL, this.spawnRateOverDistance.ImmutableValue);
            float distanceTraveled = distanceThreshold;
            Vector3 lastPosition = animatorBody.transform.position;
            while (true)
            {
                Vector3 delta = animatorBody.transform.position - lastPosition;
                lastPosition = animatorBody.transform.position;
                distanceTraveled += Vector2.Distance(Vector2.zero, delta);

                if (distanceTraveled >= distanceThreshold)
                {
                    distanceTraveled = distanceTraveled % distanceThreshold;
                    foreach (SpriteAnimatorProperty spriteProperty in animatorBody.GetCurrentProperties<SpriteAnimatorProperty>())
                    {
                        ParticleSystem particleInstance = this.particlePool.Retrieve();
                        Sprite sprite = spriteProperty.CurrentSprite;

                        ParticleSystemRenderer particleRendererer = particleInstance.GetComponent<ParticleSystemRenderer>();
                        particleRendererer.sortingOrder = spriteProperty.CurrentSortingOrder;
                        particleRendererer.material.mainTexture = sprite.texture;

                        ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = particleInstance.textureSheetAnimation;
                        textureSheetAnimation.numTilesX = Mathf.RoundToInt(sprite.texture.width / sprite.rect.width);
                        textureSheetAnimation.numTilesY = Mathf.RoundToInt(sprite.texture.height / sprite.rect.height);
                        int widthIndex = Mathf.RoundToInt(sprite.rect.position.x / sprite.rect.width);
                        int heightIndex = textureSheetAnimation.numTilesY - 1 - Mathf.RoundToInt(sprite.rect.position.y / sprite.rect.height);
                        textureSheetAnimation.startFrame = (float)(heightIndex * textureSheetAnimation.numTilesX + widthIndex) / (textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY);

                        ParticleSystem.MainModule main = particleInstance.main;
                        main.startSizeX = sprite.rect.width / sprite.pixelsPerUnit;
                        main.startSizeY = sprite.rect.height / sprite.pixelsPerUnit;

                        particleInstance.transform.SetParent(spriteProperty.transform);
                        particleInstance.transform.localPosition = (sprite.rect.size / 2 - sprite.pivot) / sprite.pixelsPerUnit + animatorBody.Direction * this.offset;
                        particleInstance.Play();

                        FrigidCoroutine.Run(
                            TweenCoroutine.DelayedCall(
                                particleInstance.main.duration,
                                () =>
                                {
                                    particleInstance.Stop();

                                    particleInstance.transform.SetParent(this.transform);
                                    particleInstance.transform.localPosition = Vector2.zero;
                                    this.particlePool.Pool(particleInstance);
                                }
                                ),
                            this.gameObject
                            );
                    }
                }

                yield return null;
            }
        }
    }
}