using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaMobGenerator", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaMobGenerator")]
    public class TiledAreaMobGenerator : FrigidScriptableObject
    {
        [SerializeField]
        private List<TiledAreaMobGeneratorStrategy> mobGeneratorStrategies;

        public void GenerateMobs(TiledLevelPlanArea planArea, TiledArea spawnedArea)
        {
            foreach (TiledAreaMobGeneratorStrategy mobGeneratorStrategy in this.mobGeneratorStrategies)
            {
                new WavesTracker(mobGeneratorStrategy, planArea, spawnedArea);
            }
        }

        private class WavesTracker
        {
            private TiledAreaMobGeneratorStrategy mobGeneratorStrategy;
            private TiledLevelPlanArea planArea;
            private TiledArea tiledArea;
            private RecyclePool<TiledAreaWaveSignal> waveSignalPool;
            private List<List<Mob>> mobsInWaves;
            private int nextWaveIndex;
            private bool isAdvancingWaves;
            private bool isLocked;
            private bool allAdvanced;

            public WavesTracker(TiledAreaMobGeneratorStrategy mobGeneratorStrategy, TiledLevelPlanArea planArea, TiledArea tiledArea)
            {
                this.mobGeneratorStrategy = mobGeneratorStrategy;
                this.planArea = planArea;
                this.tiledArea = tiledArea;
                this.waveSignalPool = new RecyclePool<TiledAreaWaveSignal>(
                    () => FrigidInstancing.CreateInstance<TiledAreaWaveSignal>(this.mobGeneratorStrategy.WaveSignalPrefab),
                    (TiledAreaWaveSignal waveSignal) => FrigidInstancing.DestroyInstance(waveSignal)
                    );
                this.mobsInWaves = new List<List<Mob>>();
                this.nextWaveIndex = 1;
                this.isAdvancingWaves = false;
                this.isLocked = false;
                this.allAdvanced = false;

                GenerateWaves();

                FrigidCoroutine updateRoutine = null;
                tiledArea.OnOpened += () => { updateRoutine = FrigidCoroutine.Run(UpdateWaves(), tiledArea.gameObject); };
                tiledArea.OnClosed += () => { FrigidCoroutine.Kill(updateRoutine); };
                if (tiledArea.IsOpened) updateRoutine = FrigidCoroutine.Run(UpdateWaves(), tiledArea.gameObject);
            }

            private void GenerateWaves()
            {
                if (this.planArea.ChosenBlueprint.MobGenerationPresets.Count == 0) return;

                for (int i = 0; (i == 0 || this.mobGeneratorStrategy.CanSpawnAdditionalWaves) && i < this.mobGeneratorStrategy.GetMaxNumberWaves(this.planArea, this.tiledArea); i++)
                {
                    HashSet<TiledAreaMobSpawnPoint> spawnPoints = new HashSet<TiledAreaMobSpawnPoint>();
                    TiledAreaMobGenerationPreset chosenMobGenerationPreset = this.planArea.ChosenBlueprint.MobGenerationPresets[UnityEngine.Random.Range(0, this.planArea.ChosenBlueprint.MobGenerationPresets.Count)];
                    foreach (TiledAreaMobGenerationPreset.SpawnPointPreset spawnPointPreset in chosenMobGenerationPreset.SpawnPointPresets)
                    {
                        if (spawnPointPreset.StrategyID == this.mobGeneratorStrategy.PresetID)
                        {
                            spawnPoints.Add(spawnPointPreset.SpawnPoint);
                        }
                    }

                    if (this.mobsInWaves.Count < this.mobGeneratorStrategy.GetMaxNumberWaves(this.planArea, this.tiledArea))
                    {
                        List<Mob> mobsInWave = this.mobGeneratorStrategy.DoGenerationStrategy(this.planArea, this.tiledArea, spawnPoints, this.mobsInWaves.Count);
                        foreach (Mob mobInWave in mobsInWave)
                        {
                            mobInWave.SetActive(this.mobsInWaves.Count == 0);
                        }
                        this.mobsInWaves.Add(mobsInWave);
                    }
                }
            }

            private IEnumerator<FrigidCoroutine.Delay> UpdateWaves()
            {
                while (true)
                {
                    AdvanceWaves();
                    CheckLockedEntrances();
                    yield return null;
                }
            }

            private void AdvanceWaves()
            {
                if (this.allAdvanced) return;

                int currentNumberWaves = this.mobGeneratorStrategy.GetCurrentNumberWaves(this.planArea, this.tiledArea);
                if (!this.isAdvancingWaves &&
                    this.mobsInWaves.Count > this.nextWaveIndex &&
                    currentNumberWaves > this.nextWaveIndex &&
                    this.mobGeneratorStrategy.CanAdvanceToNextWave(this.nextWaveIndex, this.MobsInPreviousWaves))
                {
                    List<Mob> mobsInNextWave = this.mobsInWaves[this.nextWaveIndex];
                    if (mobsInNextWave.Count != 0)
                    {
                        this.isAdvancingWaves = true;
                        int numberSpawnsCompleted = 0;
                        foreach (Mob mob in mobsInNextWave)
                        {
                            Vector2Int size = Vector2Int.one; // TODO Mobs V2, hook up actual new mobs and size
                            float delayDuration = this.mobGeneratorStrategy.WaveSpawnDelayDuration;
                            int numberSignalsWindupFinished = 0;
                            int numberSignalsComplete = 0;
                            int numberSignals = size.x * size.y;
                            for (int x = 0; x < size.x; x++)
                            {
                                for (int y = 0; y < size.y; y++)
                                {
                                    TiledAreaWaveSignal waveSignal = this.waveSignalPool.Retrieve();
                                    Vector2 signalPosition = 
                                        (Vector2)mob.AbsolutePosition - 
                                        new Vector2(size.x * GameConstants.UNIT_WORLD_SIZE, size.y * GameConstants.UNIT_WORLD_SIZE) / 2 +
                                        new Vector2(x * GameConstants.UNIT_WORLD_SIZE, y * GameConstants.UNIT_WORLD_SIZE);
                                    waveSignal.DoSignal(
                                        signalPosition,
                                        delayDuration, 
                                        () => 
                                        {
                                            numberSignalsWindupFinished++;
                                            if (numberSignalsWindupFinished >= numberSignals)
                                            {
                                                mob.SetActive(true);
                                            }
                                        }, 
                                        () =>
                                        {
                                            numberSignalsComplete++;
                                            if (numberSignalsComplete >= numberSignals)
                                            {
                                                numberSpawnsCompleted++;
                                                if (numberSpawnsCompleted == mobsInNextWave.Count) this.isAdvancingWaves = false;
                                            }
                                            this.waveSignalPool.Pool(waveSignal);
                                        }
                                        );
                                }
                            }
                        }
                    }
                    this.nextWaveIndex++;
                }

                if (this.nextWaveIndex >= currentNumberWaves) this.allAdvanced = true;
            }

            private void CheckLockedEntrances()
            {
                bool shouldLock = this.mobGeneratorStrategy.ShouldLockEntrances(this.MobsInPreviousWaves, this.MobsInWavesToAdvance);
                if (this.isLocked != shouldLock)
                {
                    this.isLocked = shouldLock;
                    if (shouldLock)
                    {
                        foreach (TiledAreaEntrance placedEntrance in this.tiledArea.PlacedEntrances) placedEntrance.Locked.Request();
                    }
                    else
                    {
                        foreach (TiledAreaEntrance placedEntrance in this.tiledArea.PlacedEntrances) placedEntrance.Locked.Release();
                    }
                }
            }

            private List<Mob> MobsInPreviousWaves
            {
                get
                {
                    List<Mob> mobsInPreviousWaves = new List<Mob>();
                    for (int i = 0; i < Mathf.Min(this.nextWaveIndex, this.mobsInWaves.Count); i++) mobsInPreviousWaves.AddRange(this.mobsInWaves[i]);
                    return mobsInPreviousWaves;
                }
            }

            private List<Mob> MobsInWavesToAdvance
            {
                get
                {
                    List<Mob> mobsInWavesToAdvance = new List<Mob>();
                    if (this.allAdvanced) return mobsInWavesToAdvance;
                    for (int i = this.nextWaveIndex; i < Mathf.Min(this.mobGeneratorStrategy.GetCurrentNumberWaves(this.planArea, this.tiledArea), this.mobsInWaves.Count); i++) mobsInWavesToAdvance.AddRange(this.mobsInWaves[i]);
                    return mobsInWavesToAdvance;
                }
            }
        }
    }
}
