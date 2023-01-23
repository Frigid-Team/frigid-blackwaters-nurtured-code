using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "CombatantsTiledAreaMobGeneratorStrategy", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "CombatantsTiledAreaMobGeneratorStrategy")]
    public class CombatantsTiledAreaMobGeneratorStrategy : TiledAreaMobGeneratorStrategy
    {
        [SerializeField]
        private List<MobSpawnable> mobSpawnableSelection;
        [SerializeField]
        private FloatSerializedReference filterDistance;
        [SerializeField]
        private FloatSerializedReference uniquenessToMaxTierSumMultiplier;
        [SerializeField]
        private IntSerializedReference minPossibleTierSum;
        [SerializeField]
        private List<MaxTierSumBracket> maxTierSumBrackets;
        [SerializeField]
        private List<BlueprintGroupTierMultiplier> blueprintGroupTierMultipliers;
        [SerializeField]
        private List<BlueprintGroupWavesSetting> blueprintGroupWavesSettings;

        public override int GetMaxNumberWaves(TiledLevelPlanArea planArea, TiledArea tiledArea)
        {
            if (TryGetWavesSetting(planArea.BlueprintGroup, out BlueprintGroupWavesSetting foundWavesSetting))
            {
                return foundWavesSetting.MaxNumberWaves;
            }
            return base.GetMaxNumberWaves(planArea, tiledArea);
        }

        public override int GetCurrentNumberWaves(TiledLevelPlanArea planArea, TiledArea tiledArea)
        {
            if (TryGetWavesSetting(planArea.BlueprintGroup, out BlueprintGroupWavesSetting foundWavesSetting))
            {
                if (foundWavesSetting.MinNumberWaves < foundWavesSetting.MaxNumberWaves)
                {
                    float exploredPercent = GetExploredPercent(tiledArea);
                    if (exploredPercent >= foundWavesSetting.ExploredPercentUntilAdditionalWaves)
                    {
                        return Mathf.RoundToInt(Mathf.Lerp(foundWavesSetting.MinNumberWaves + 1, foundWavesSetting.MaxNumberWaves, (exploredPercent - foundWavesSetting.ExploredPercentUntilAdditionalWaves) / (1 - foundWavesSetting.ExploredPercentUntilAdditionalWaves)));
                    }
                }
                return foundWavesSetting.MinNumberWaves;
            }
            return base.GetCurrentNumberWaves(planArea, tiledArea);
        }

        public override bool CanAdvanceToNextWave(int waveIndex, List<Mob> mobsInPreviousWaves)
        {
            int numAliveHostileMobs = 0;
            foreach (Mob mob in mobsInPreviousWaves)
            {
                if (mob.Active && !mob.Dead)
                {
                    numAliveHostileMobs++;
                }
            }
            return waveIndex == 0 || numAliveHostileMobs == 0;
        }

        public override bool ShouldLockEntrances(List<Mob> mobsInPreviousWaves, List<Mob> mobsInWavesToAdvance)
        {
            foreach (Mob mobInPreviousWaves in mobsInPreviousWaves)
            {
                if (!mobInPreviousWaves.Dead) return true;
            }
            foreach (Mob mobInWavesToAdvance in mobsInWavesToAdvance)
            {
                if (!mobInWavesToAdvance.Dead) return true;
            }
            return false;
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawnsInTiledArea(TiledLevelPlanArea planArea, TiledArea tiledArea, HashSet<TiledAreaMobSpawnPoint> spawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();

            List<TiledAreaMobSpawnPoint> remainingSpawnPoints = FilterSpawnPointsByDistance(planArea, spawnPoints, waveIndex);
            Dictionary<int, List<MobSpawnable>> remainingSpawnables = new Dictionary<int, List<MobSpawnable>>();
            foreach (MobSpawnable mobSpawnable in this.mobSpawnableSelection)
            {
                if (!remainingSpawnables.ContainsKey(mobSpawnable.Tier))
                {
                    remainingSpawnables.Add(mobSpawnable.Tier, new List<MobSpawnable>());
                }
                remainingSpawnables[mobSpawnable.Tier].Add(mobSpawnable);
            }

            int maxTierSum = CalculateMaximumTierSum(planArea, waveIndex);
            int maxNumUniqueMobs = CalculateMaximumNumberOfUniqueMobs(maxTierSum);
            int currentTierSum = 0;
            List<MobSpawnable> uniqueSpawnables = new List<MobSpawnable>();

            while (remainingSpawnPoints.Count > 0)
            {
                // Choose a spawn point out of our remaining spawn points
                int chosenSpawnPointIndex = UnityEngine.Random.Range(0, remainingSpawnPoints.Count);
                TiledAreaMobSpawnPoint chosenSpawnPoint = remainingSpawnPoints[chosenSpawnPointIndex];
                remainingSpawnPoints.RemoveAt(chosenSpawnPointIndex);

                if (UnityEngine.Random.Range(0, maxNumUniqueMobs) >= uniqueSpawnables.Count)
                {
                    // We now add a new mob to our chosen mobs (which has a limit to reduce variety)
                    List<Tuple<MobSpawnable, int>> validSpawnablesAndIndexes = new List<Tuple<MobSpawnable, int>>();

                    List<int> remainingAndValidTiers = new List<int>();
                    foreach (int remainingTier in remainingSpawnables.Keys)
                    {
                        if (currentTierSum + remainingTier <= maxTierSum)
                        {
                            remainingAndValidTiers.Add(remainingTier);
                        }
                    }

                    if (remainingAndValidTiers.Count > 0)
                    {
                        int chosenTier = remainingAndValidTiers[UnityEngine.Random.Range(0, remainingAndValidTiers.Count)];
                        for (int i = 0; i < remainingSpawnables[chosenTier].Count; i++)
                        {
                            MobSpawnable remainingSpawnable = remainingSpawnables[chosenTier][i];
                            if (chosenSpawnPoint.CanSpawnHere(remainingSpawnable, tiledArea.AbsoluteCenterPosition))
                            {
                                // Spawnables with lower traversability have a higher chance to be picked
                                // to balance out opportunities that multi-terrain mobs have with other spawn points
                                for (int j = 0; j <= (int)TileTerrain.Count - remainingSpawnable.SpawnTraversableTerrain.TerrainCount; j++)
                                {
                                    validSpawnablesAndIndexes.Add(new Tuple<MobSpawnable, int>(remainingSpawnable, i));
                                }
                            }
                        }

                        if (validSpawnablesAndIndexes.Count > 0)
                        {
                            Tuple<MobSpawnable, int> chosenSpawnableAndIndex = validSpawnablesAndIndexes[UnityEngine.Random.Range(0, validSpawnablesAndIndexes.Count)];
                            remainingSpawnables[chosenTier].RemoveAt(chosenSpawnableAndIndex.Item2);
                            uniqueSpawnables.Add(chosenSpawnableAndIndex.Item1);
                        }
                    }
                }

                // Out of all our chosen mobs, we choose the ones that can spawn on the given spawn point
                List<MobSpawnable> possibleSpawnables = new List<MobSpawnable>();
                foreach (MobSpawnable uniqueSpawnable in uniqueSpawnables)
                {
                    if (chosenSpawnPoint.CanSpawnHere(uniqueSpawnable, tiledArea.AbsoluteCenterPosition) && currentTierSum + uniqueSpawnable.Tier <= maxTierSum)
                    {
                        possibleSpawnables.Add(uniqueSpawnable);
                    }
                }
                if (possibleSpawnables.Count > 0)
                {
                    MobSpawnable spawnableToSpawn = possibleSpawnables[UnityEngine.Random.Range(0, possibleSpawnables.Count)];
                    mobSpawns.Add(chosenSpawnPoint, spawnableToSpawn);
                    currentTierSum += spawnableToSpawn.Tier;
                }
            }

            return mobSpawns;
        }

        private float GetExploredPercent(TiledArea tiledArea)
        {
            if (TiledLevel.TryGetTiledLevelAtPosition(tiledArea.AbsoluteCenterPosition, out TiledLevel tiledLevel))
            {
                int exploredCount = TiledWorldExplorer.ExploredTiledAreas.Intersect(tiledLevel.SpawnedAreaPerPlanAreas.Values).Count<TiledArea>();
                return (float)exploredCount / tiledLevel.SpawnedAreaPerPlanAreas.Count;
            }
            return 0;
        }

        private bool TryGetWavesSetting(TiledAreaBlueprintGroup blueprintGroup, out BlueprintGroupWavesSetting foundWavesSetting)
        {
            foreach (BlueprintGroupWavesSetting blueprintGroupWavesSetting in this.blueprintGroupWavesSettings)
            {
                if (blueprintGroupWavesSetting.BlueprintGroup == blueprintGroup)
                {
                    foundWavesSetting = blueprintGroupWavesSetting;
                    return true;
                }
            }
            foundWavesSetting = default(BlueprintGroupWavesSetting);
            return false;
        }

        private int CalculateMaximumTierSum(TiledLevelPlanArea planArea, int waveIndex)
        {
            float tierMultiplier = 1;
            foreach (BlueprintGroupTierMultiplier blueprintGroupTierMultiplier in this.blueprintGroupTierMultipliers)
            {
                if (blueprintGroupTierMultiplier.BlueprintGroup == planArea.BlueprintGroup)
                {
                    tierMultiplier = blueprintGroupTierMultiplier.TierMultiplier;
                    break;
                }
            }
            foreach(MaxTierSumBracket maxTierSumBracket in this.maxTierSumBrackets)
            {
                if (planArea.NumberAreasFromStartPercent <= maxTierSumBracket.Percentage)
                {
                    return Mathf.FloorToInt((Mathf.Max(maxTierSumBracket.MaxTierSum, this.minPossibleTierSum.ImmutableValue) * tierMultiplier) + maxTierSumBracket.TierIncreasePerWave * waveIndex);
                }
            }
            return Mathf.FloorToInt(this.minPossibleTierSum.ImmutableValue * tierMultiplier);
        }

        private int CalculateMaximumNumberOfUniqueMobs(float maxTierSum)
        {
            return Mathf.Max(1, Mathf.FloorToInt(maxTierSum * this.uniquenessToMaxTierSumMultiplier.ImmutableValue));
        }

        private List<TiledAreaMobSpawnPoint> FilterSpawnPointsByDistance(TiledLevelPlanArea planArea, HashSet<TiledAreaMobSpawnPoint> spawnPoints, int waveIndex)
        {
            List<TiledAreaMobSpawnPoint> filteredSpawnPoints = new List<TiledAreaMobSpawnPoint>();
            if (waveIndex == 0)
            {
                foreach (TiledAreaMobSpawnPoint spawnPoint in spawnPoints)
                {
                    bool awayFromWallEntry = true;
                    foreach (Vector2Int occupiedWallEntryDirection in planArea.OccupiedWallEntryDirections)
                    {
                        Vector2 centerWallPosition = TilePositioning.LocalWallCenterPosition(occupiedWallEntryDirection, planArea.ChosenBlueprint.MainAreaDimensions);
                        if (Vector2.Distance(centerWallPosition, spawnPoint.LocalPosition) <= this.filterDistance.ImmutableValue)
                        {
                            awayFromWallEntry = false;
                            break;
                        }
                    }
                    if (awayFromWallEntry)
                    {
                        filteredSpawnPoints.Add(spawnPoint);
                    }
                }
            }
            else
            {
                foreach (TiledAreaMobSpawnPoint spawnPoint in spawnPoints)
                {
                    filteredSpawnPoints.Add(spawnPoint);
                }
            }
            return filteredSpawnPoints;
        }

        [Serializable]
        private struct MaxTierSumBracket
        {
            [SerializeField]
            private float percentage;
            [SerializeField]
            private int maxTierSum;
            [SerializeField]
            private float tierIncreasePerWave;

            public float Percentage
            {
                get
                {
                    return this.percentage;
                }
            }

            public int MaxTierSum
            {
                get
                {
                    return this.maxTierSum;
                }
            }

            public float TierIncreasePerWave
            {
                get
                {
                    return this.tierIncreasePerWave;
                }
            }
        }

        [Serializable]
        private struct BlueprintGroupTierMultiplier
        {
            [SerializeField]
            private TiledAreaBlueprintGroup blueprintGroup;
            [SerializeField]
            private float tierMultiplier;

            public TiledAreaBlueprintGroup BlueprintGroup
            {
                get
                {
                    return this.blueprintGroup;
                }
            }
            public float TierMultiplier
            {
                get
                {
                    return this.tierMultiplier;
                }
            }
        }

        [Serializable]
        private struct BlueprintGroupWavesSetting
        {
            [SerializeField]
            private TiledAreaBlueprintGroup blueprintGroup;
            [SerializeField]
            private IntSerializedReference minNumberWaves;
            [SerializeField]
            private IntSerializedReference maxNumberWaves;
            [SerializeField]
            private FloatSerializedReference exploredPercentUntilAdditionalWaves;

            public TiledAreaBlueprintGroup BlueprintGroup
            {
                get
                {
                    return this.blueprintGroup;
                }
            }

            public int MinNumberWaves
            {
                get
                {
                    return this.minNumberWaves.ImmutableValue;
                }
            }

            public int MaxNumberWaves
            {
                get
                {
                    return this.maxNumberWaves.ImmutableValue;
                }
            }

            public float ExploredPercentUntilAdditionalWaves
            {
                get
                {
                    return this.exploredPercentUntilAdditionalWaves.ImmutableValue;
                }
            }
        }
    }
}
