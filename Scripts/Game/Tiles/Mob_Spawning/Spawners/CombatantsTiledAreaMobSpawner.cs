using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "CombatantsTiledAreaMobSpawner", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "CombatantsTiledAreaMobSpawner")]
    public class CombatantsTiledAreaMobSpawner : TiledAreaMobSpawner
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

        public override int GetMaxNumberWaves(TiledLevelPlanArea planArea, TiledArea area)
        {
            if (this.TryGetWavesSetting(planArea.BlueprintGroup, out BlueprintGroupWavesSetting foundWavesSetting))
            {
                return foundWavesSetting.MaxNumberWaves;
            }
            return base.GetMaxNumberWaves(planArea, area);
        }

        public override int GetCurrentNumberWaves(TiledLevelPlanArea planArea, TiledArea area)
        {
            if (this.TryGetWavesSetting(planArea.BlueprintGroup, out BlueprintGroupWavesSetting foundWavesSetting))
            {
                if (foundWavesSetting.MinNumberWaves < foundWavesSetting.MaxNumberWaves)
                {
                    float exploredPercent = this.GetExploredPercent(area);
                    if (exploredPercent >= foundWavesSetting.ExploredPercentUntilAdditionalWaves)
                    {
                        return Mathf.RoundToInt(Mathf.Lerp(foundWavesSetting.MinNumberWaves + 1, foundWavesSetting.MaxNumberWaves, (exploredPercent - foundWavesSetting.ExploredPercentUntilAdditionalWaves) / (1 - foundWavesSetting.ExploredPercentUntilAdditionalWaves)));
                    }
                }
                return foundWavesSetting.MinNumberWaves;
            }
            return base.GetCurrentNumberWaves(planArea, area);
        }

        public override bool CanAdvanceToNextWave(int waveIndex, HashSet<Mob> mobsInPreviousWaves)
        {
            return waveIndex == 0 || mobsInPreviousWaves.All((Mob mobInPreviousWave) => mobInPreviousWave.Status == MobStatus.Dead);
        }

        public override bool ShouldLockEntrances(HashSet<Mob> mobsInPreviousWaves, HashSet<Mob> mobsInWavesToAdvance)
        {
            return mobsInPreviousWaves.Any((Mob mobInPreviousWave) => mobInPreviousWave.Status != MobStatus.Dead) || mobsInWavesToAdvance.Any((Mob mobInPreviousWave) => mobInPreviousWave.Status != MobStatus.Dead);
        }

        protected override Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> DetermineMobSpawns(TiledLevelPlanArea planArea, TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints, int waveIndex)
        {
            Dictionary<TiledAreaMobSpawnPoint, MobSpawnable> mobSpawns = new Dictionary<TiledAreaMobSpawnPoint, MobSpawnable>();

            List<TiledAreaMobSpawnPoint> remainingSpawnPoints = this.FilterSpawnPointsByDistance(area, mobSpawnPoints, waveIndex);
            Dictionary<int, List<MobSpawnable>> remainingSpawnables = new Dictionary<int, List<MobSpawnable>>();
            foreach (MobSpawnable mobSpawnable in this.mobSpawnableSelection)
            {
                if (!remainingSpawnables.ContainsKey(mobSpawnable.Tier))
                {
                    remainingSpawnables.Add(mobSpawnable.Tier, new List<MobSpawnable>());
                }
                remainingSpawnables[mobSpawnable.Tier].Add(mobSpawnable);
            }

            int maxTierSum = this.CalculateMaximumTierSum(planArea, waveIndex);
            int maxNumUniqueMobs = this.CalculateMaximumNumberOfUniqueMobs(maxTierSum);
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
                    List<(MobSpawnable spawnable, int index)> validSpawnablesAndIndexes = new List<(MobSpawnable spawnable, int index)>();

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
                            int numberSpawnPointsThatCanSpawn = 0;
                            foreach (TiledAreaMobSpawnPoint remainingSpawnPoint in remainingSpawnPoints)
                            {
                                if (remainingSpawnPoint.CanSpawnHere(remainingSpawnable, area))
                                {
                                    numberSpawnPointsThatCanSpawn++;
                                }
                            }

                            if (chosenSpawnPoint.CanSpawnHere(remainingSpawnable, area))
                            {
                                // Spawnables with lower number of available spawn points have a higher chance to be picked
                                // to balance out opportunities that multi-terrain mobs have with other spawn points
                                for (int j = 0; j <= Mathf.Max(1, remainingSpawnPoints.Count - numberSpawnPointsThatCanSpawn); j++)
                                {
                                    validSpawnablesAndIndexes.Add((remainingSpawnable, i));
                                }
                            }
                        }

                        if (validSpawnablesAndIndexes.Count > 0)
                        {
                            (MobSpawnable spawnable, int index) chosenSpawnableAndIndex = validSpawnablesAndIndexes[UnityEngine.Random.Range(0, validSpawnablesAndIndexes.Count)];
                            remainingSpawnables[chosenTier].RemoveAt(chosenSpawnableAndIndex.index);
                            uniqueSpawnables.Add(chosenSpawnableAndIndex.spawnable);
                        }
                    }
                }

                // Out of all our chosen mobs, we choose the ones that can spawn on the given spawn point
                List<MobSpawnable> possibleSpawnables = new List<MobSpawnable>();
                foreach (MobSpawnable uniqueSpawnable in uniqueSpawnables)
                {
                    if (chosenSpawnPoint.CanSpawnHere(uniqueSpawnable, area) && currentTierSum + uniqueSpawnable.Tier <= maxTierSum)
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

        private float GetExploredPercent(TiledArea area)
        {
            int exploredCount = TiledWorldExplorer.ExploredAreas.Intersect(area.ContainedLevel.ContainingAreas).Count<TiledArea>();
            return (float)exploredCount / area.ContainedLevel.ContainingAreas.Count;
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

            float totalPortionSum = 0;
            foreach (MaxTierSumBracket maxTierSumBracket in this.maxTierSumBrackets)
            {
                totalPortionSum += maxTierSumBracket.PortionOfAllBrackets;
            }
            float cumulativePortionSum = 0;
            foreach (MaxTierSumBracket maxTierSumBracket in this.maxTierSumBrackets)
            {
                cumulativePortionSum += maxTierSumBracket.PortionOfAllBrackets;
                if (planArea.NumberAreasFromStartPercent <= cumulativePortionSum / totalPortionSum)
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

        private List<TiledAreaMobSpawnPoint> FilterSpawnPointsByDistance(TiledArea area, HashSet<TiledAreaMobSpawnPoint> mobSpawnPoints, int waveIndex)
        {
            List<TiledAreaMobSpawnPoint> filteredSpawnPoints = new List<TiledAreaMobSpawnPoint>();
            if (waveIndex == 0)
            {
                foreach (TiledAreaMobSpawnPoint spawnPoint in mobSpawnPoints)
                {
                    bool awayFromWallEntry = true;
                    foreach (TiledEntrance entrance in area.ContainingEntrances)
                    {
                        if (Vector2.Distance(entrance.EntryPosition - area.CenterPosition, spawnPoint.LocalPosition) <= this.filterDistance.ImmutableValue)
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
                foreach (TiledAreaMobSpawnPoint spawnPoint in mobSpawnPoints)
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
            private float portionOfAllBrackets;
            [SerializeField]
            private int maxTierSum;
            [SerializeField]
            private float tierIncreasePerWave;

            public float PortionOfAllBrackets
            {
                get
                {
                    return this.portionOfAllBrackets;
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
