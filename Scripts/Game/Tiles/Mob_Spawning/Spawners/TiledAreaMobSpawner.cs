using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Utility;
using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public abstract class TiledAreaMobSpawner : FrigidScriptableObject
    {
        [SerializeField]
        private bool canSpawnAdditionalWaves;
        [SerializeField]
        [ShowIfBool("canSpawnAdditionalWaves", true)]
        private TiledAreaWaveSignal waveSignalPrefab;
        [SerializeField]
        [ShowIfBool("canSpawnAdditionalWaves", true)]
        private FloatSerializedReference waveSpawnDelayDuration;

        public void SpawnMobs(TiledLevelPlanArea planArea, TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints)
        {
            List<HashSet<Mob>> mobsInWaves = new List<HashSet<Mob>>();
            for (int waveIndex = 0; (waveIndex == 0 || this.canSpawnAdditionalWaves) && waveIndex < this.GetMaxNumberWaves(planArea, area); waveIndex++)
            {
                if (mobsInWaves.Count < this.GetMaxNumberWaves(planArea, area))
                {
                    HashSet<Mob> mobsInWave = new HashSet<Mob>();
                    Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = this.DetermineMobSpawns(planArea, area, mobSpawnPoints, waveIndex);
                    foreach (KeyValuePair<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawn in mobSpawns)
                    {
                        TiledAreaMobSpawnPoint spawnPoint = mobSpawn.Key;
                        MobSpawnable mobSpawnable = mobSpawn.Value;
                        if (spawnPoint.CanSpawnHere(mobSpawnable, area))
                        {
                            mobsInWave.Add(mobSpawnable.SpawnMob(area.CenterPosition + spawnPoint.LocalPosition, -spawnPoint.LocalPosition.normalized));
                        }
                    }
                    foreach (Mob mobInWave in mobsInWave)
                    {
                        if (mobsInWaves.Count > 0)
                        {
                            mobInWave.ToInactive();
                        }
                    }
                    mobsInWaves.Add(mobsInWave);
                }
            }
            FrigidCoroutine.Run(this.SpawnWaves(planArea, area, mobsInWaves), area.ContentsTransform.gameObject);
        }

        public virtual int GetMaxNumberWaves(TiledLevelPlanArea planArea, TiledArea area) { return 1; }

        public virtual int GetCurrentNumberWaves(TiledLevelPlanArea planArea, TiledArea area) { return 1; }

        public virtual bool CanAdvanceToNextWave(int waveIndex, HashSet<Mob> mobsInPreviousWaves) { return true; }

        public virtual bool ShouldLockEntrances(HashSet<Mob> mobsInPreviousWaves, HashSet<Mob> mobsInWavesToAdvance) { return false; }

        protected abstract Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawns(TiledLevelPlanArea planArea, TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints, int waveIndex);

        private IEnumerator<FrigidCoroutine.Delay> SpawnWaves(TiledLevelPlanArea planArea, TiledArea area, List<HashSet<Mob>> mobsInWaves)
        {
            RecyclePool<TiledAreaWaveSignal> waveSignalPool = new RecyclePool<TiledAreaWaveSignal>(
                () => FrigidMonoBehaviour.CreateInstance<TiledAreaWaveSignal>(this.waveSignalPrefab),
                (TiledAreaWaveSignal waveSignal) => FrigidMonoBehaviour.DestroyInstance(waveSignal)
                );

            int nextWaveIndex = 1;
            bool isLocked = false;
            HashSet<Mob> mobsInPreviousWaves;
            HashSet<Mob> mobsInWavesToAdvance;
            while (true)
            {
                mobsInPreviousWaves = new HashSet<Mob>();
                for (int i = 0; i < Mathf.Min(nextWaveIndex, mobsInWaves.Count); i++) mobsInPreviousWaves.UnionWith(mobsInWaves[i]);
                mobsInWavesToAdvance = new HashSet<Mob>();
                for (int i = nextWaveIndex; i < Mathf.Min(this.GetCurrentNumberWaves(planArea, area), mobsInWaves.Count); i++) mobsInWavesToAdvance.UnionWith(mobsInWaves[i]);

                bool shouldLock = this.ShouldLockEntrances(mobsInPreviousWaves, mobsInWavesToAdvance);
                if (isLocked != shouldLock)
                {
                    isLocked = shouldLock;
                    if (shouldLock)
                    {
                        foreach (TiledEntrance containingEntrance in area.ContainingEntrances) containingEntrance.Locked.Request();
                    }
                    else
                    {
                        foreach (TiledEntrance containingEntrance in area.ContainingEntrances) containingEntrance.Locked.Release();
                    }
                }

                if (mobsInWaves.Count <= nextWaveIndex || nextWaveIndex >= this.GetCurrentNumberWaves(planArea, area)) break;

                if (this.CanAdvanceToNextWave(nextWaveIndex, mobsInPreviousWaves))
                {
                    HashSet<Mob> mobsInWave = mobsInWaves[nextWaveIndex];
                    if (mobsInWave.Count != 0)
                    {
                        int numberSpawnsCompleted = 0;
                        foreach (Mob mobInWave in mobsInWave)
                        {
                            Vector2Int tileSize = mobInWave.TileSize;
                            int numberSignalsWindupFinished = 0;
                            int numberSignalsComplete = 0;
                            int numberSignals = tileSize.x * tileSize.y;
                            for (int x = 0; x < tileSize.x; x++)
                            {
                                for (int y = 0; y < tileSize.y; y++)
                                {
                                    TiledAreaWaveSignal waveSignal = waveSignalPool.Retrieve();
                                    Vector2 signalPosition =
                                        mobInWave.Position -
                                        new Vector2((tileSize.x - 1) * FrigidConstants.UNIT_WORLD_SIZE, (tileSize.y - 1) * FrigidConstants.UNIT_WORLD_SIZE) / 2 +
                                        new Vector2(x * FrigidConstants.UNIT_WORLD_SIZE, y * FrigidConstants.UNIT_WORLD_SIZE);
                                    float delayDuration = this.waveSpawnDelayDuration.MutableValue;
                                    waveSignal.DoSignal(
                                        signalPosition,
                                        delayDuration,
                                        () =>
                                        {
                                            numberSignalsWindupFinished++;
                                            if (numberSignalsWindupFinished >= numberSignals) mobInWave.ToActive();
                                        },
                                        () =>
                                        {
                                            numberSignalsComplete++;
                                            if (numberSignalsComplete >= numberSignals) numberSpawnsCompleted++;
                                            waveSignalPool.Pool(waveSignal);
                                        }
                                        );
                                }
                            }
                        }
                        yield return new FrigidCoroutine.DelayUntil(() => numberSpawnsCompleted == mobsInWave.Count);
                    }
                    nextWaveIndex++;
                }
                yield return null;
            }

            mobsInPreviousWaves = new HashSet<Mob>();
            for (int i = 0; i < Mathf.Min(nextWaveIndex, mobsInWaves.Count); i++) mobsInPreviousWaves.UnionWith(mobsInWaves[i]);
            mobsInWavesToAdvance = new HashSet<Mob>();
            while (true)
            {
                bool shouldLock = this.ShouldLockEntrances(mobsInPreviousWaves, mobsInWavesToAdvance);
                if (isLocked != shouldLock)
                {
                    isLocked = shouldLock;
                    if (shouldLock)
                    {
                        foreach (TiledEntrance containingEntrance in area.ContainingEntrances) containingEntrance.Locked.Request();
                    }
                    else
                    {
                        foreach (TiledEntrance containingEntrance in area.ContainingEntrances) containingEntrance.Locked.Release();
                    }
                }
                yield return null;
            }
        }
    }
}
