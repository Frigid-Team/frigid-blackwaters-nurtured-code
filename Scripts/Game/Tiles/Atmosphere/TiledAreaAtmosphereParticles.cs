using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class TiledAreaAtmosphereParticles : TiledAreaAtmosphere
    {
        [SerializeField]
        private TiledAreaAtmosphericParticle atmosphericParticlePrefab;
        [SerializeField]
        private FloatSerializedReference spawnRate;

        private FrigidCoroutine currentSpawnRoutine;
        private List<TiledAreaAtmosphericParticle> currentParticles;
        private RecyclePool<TiledAreaAtmosphericParticle> particlePool;

        public override void StartAtmosphere(Vector2Int mainAreaDimensions, Transform contentsTransform)
        {
            this.currentSpawnRoutine = FrigidCoroutine.Run(this.SpawnParticles(mainAreaDimensions, contentsTransform), this.gameObject);
        }

        public override void StopAtmosphere()
        {
            FrigidCoroutine.Kill(this.currentSpawnRoutine);
            this.particlePool.Pool(this.currentParticles);
            this.currentParticles.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            this.particlePool = new RecyclePool<TiledAreaAtmosphericParticle>(
                () => CreateInstance<TiledAreaAtmosphericParticle>(this.atmosphericParticlePrefab),
                (TiledAreaAtmosphericParticle particle) => DestroyInstance(particle)
                );
            this.currentParticles = new List<TiledAreaAtmosphericParticle>();
        }

        private IEnumerator<FrigidCoroutine.Delay> SpawnParticles(Vector2Int dimensions, Transform contentsTransform)
        {
            FrigidCoroutine.DelayForSeconds spawnDelay = new FrigidCoroutine.DelayForSeconds(1 / this.spawnRate.ImmutableValue);
            while (true)
            {
                TiledAreaAtmosphericParticle spawnedParticle = this.particlePool.Retrieve();
                spawnedParticle.transform.SetParent(contentsTransform);
                this.currentParticles.Add(spawnedParticle);
                spawnedParticle.RunParticle(
                    AreaTiling.RandomTileLocalPosition(dimensions), 
                    () => 
                    {
                        this.currentParticles.Remove(spawnedParticle);
                        this.particlePool.Pool(spawnedParticle); 
                    }
                    );
                yield return spawnDelay;
            }
        }
    }
}
