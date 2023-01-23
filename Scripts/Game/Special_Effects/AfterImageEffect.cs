using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class AfterImageEffect : SpecialEffect
    {
        private const float AFTER_IMAGE_Y_SPAWN_OFFSET = 0.001f;

        [SerializeField]
        private TweenCoroutineTemplate spawnAfterImagesRoutineTemplate;
        [SerializeField]
        private List<SpriteRenderer> parentSegmentRenderers;
        [SerializeField]
        private AfterImage afterImagePrefab;

        private FrigidCoroutine spawnAfterImagesRoutine;
        private RecyclePool<AfterImage> afterImagePool;

        protected override void Played(float effectsTimeScaling = 1)
        {
            this.spawnAfterImagesRoutine = FrigidCoroutine.Run(
                this.spawnAfterImagesRoutineTemplate.GetRoutine(
                    onIterationComplete: 
                    () =>
                    {
                        Vector2 absoluteSpawnPosition = this.transform.position + new Vector3(0, AFTER_IMAGE_Y_SPAWN_OFFSET, 0);
                        AfterImage spawnedAfterImage = this.afterImagePool.Retrieve();
                        spawnedAfterImage.PlayAfterImage(absoluteSpawnPosition, this.parentSegmentRenderers, () => this.afterImagePool.Pool(spawnedAfterImage));
                    }
                    ), 
                this.gameObject
                );
        }

        protected override void Stopped() 
        {
            FrigidCoroutine.Kill(this.spawnAfterImagesRoutine);
        }

        protected override void Awake()
        {
            base.Awake();
            this.afterImagePool = new RecyclePool<AfterImage>(
                () => FrigidInstancing.CreateInstance<AfterImage>(this.afterImagePrefab), 
                (AfterImage afterImage) => FrigidInstancing.DestroyInstance(afterImage)
                );
        }
    }
}
