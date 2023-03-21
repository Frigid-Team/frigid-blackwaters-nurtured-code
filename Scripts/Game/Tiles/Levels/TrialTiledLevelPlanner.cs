using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game 
{
    [CreateAssetMenu(fileName = "TrialTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TrialTiledLevelPlanner")]
    public class TrialTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private RelativeWeightPool<TrialPair> trialPairs;
        [SerializeField]
        private TiledArea trialAreaPrefab;
        [SerializeField]
        private TiledAreaEntrance exitEntrancePrefab;
        [SerializeField]
        private TiledAreaEntrance rewardEntrancePrefab;
        [SerializeField]
        private TiledAreaMobGenerator trialMobGenerator;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TrialPair trialPair = this.trialPairs.Retrieve();
            TiledLevelPlanArea combatPlanArea = new TiledLevelPlanArea(this.trialAreaPrefab, trialPair.CombatBlueprintGroup);
            TiledLevelPlan levelPlan = new TiledLevelPlan(combatPlanArea, this.trialMobGenerator);

            TiledLevelPlanArea rewardPlanArea = new TiledLevelPlanArea(this.trialAreaPrefab, trialPair.RewardBlueprintGroup);
            levelPlan.AddArea(rewardPlanArea);
            levelPlan.AddConnection(
                new TiledLevelPlanConnection(
                    new TiledLevelPlanEntrance(combatPlanArea, this.rewardEntrancePrefab),
                    new TiledLevelPlanEntrance(rewardPlanArea, this.rewardEntrancePrefab),
                    Vector2Int.up
                    )
                );

            if (subLevelEntrancesAndContainedAreas.Count > 1)
            {
                Debug.LogError("TrialTiledLevelPlanners cannot support more than 1 sub level entrance.");
                return levelPlan;
            }

            foreach (TiledAreaEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                TiledArea containedArea = subLevelEntrancesAndContainedAreas[subLevelEntrance];
                levelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(combatPlanArea, this.exitEntrancePrefab),
                        Vector2Int.up,
                        containedArea.NavigationGrid.TerrainAtTile(TilePositioning.TileIndicesFromPosition(subLevelEntrance.transform.position, containedArea.CenterPosition, containedArea.MainAreaDimensions))
                        )
                    );
            }

            return levelPlan;
        }

        [Serializable]
        private struct TrialPair
        {
            [SerializeField]
            private TiledAreaBlueprintGroup combatBlueprintGroup;
            [SerializeField]
            private TiledAreaBlueprintGroup rewardBlueprintGroup;

            public TiledAreaBlueprintGroup CombatBlueprintGroup
            {
                get
                {
                    return this.combatBlueprintGroup;
                }
            }
            public TiledAreaBlueprintGroup RewardBlueprintGroup
            {
                get
                {
                    return this.rewardBlueprintGroup;
                }
            }
        }
    }
}
