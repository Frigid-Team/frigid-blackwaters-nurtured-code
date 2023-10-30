using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game 
{
    [CreateAssetMenu(fileName = "TrialTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "TrialTiledLevelPlanner")]
    public class TrialTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private RelativeWeightPool<TrialPair> trialPairs;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TrialPair trialPair = this.trialPairs.Retrieve();
            TiledLevelPlanArea combatPlanArea = new TiledLevelPlanArea(trialPair.CombatBlueprintGroup);
            TiledLevelPlan levelPlan = new TiledLevelPlan(combatPlanArea);

            TiledLevelPlanArea rewardPlanArea = new TiledLevelPlanArea(trialPair.RewardBlueprintGroup);
            levelPlan.AddArea(rewardPlanArea);
            levelPlan.AddConnection(
                new TiledLevelPlanConnection(
                    new TiledLevelPlanEntrance(combatPlanArea),
                    new TiledLevelPlanEntrance(rewardPlanArea),
                    Vector2Int.up
                    )
                );

            if (subLevelEntrancesAndContainedAreas.Count > 1)
            {
                Debug.LogError("TrialTiledLevelPlanners cannot support more than 1 sub level entrance.");
                return levelPlan;
            }

            foreach (TiledEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                TiledArea containedArea = subLevelEntrancesAndContainedAreas[subLevelEntrance];
                levelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(combatPlanArea),
                        Vector2Int.up,
                        containedArea.NavigationGrid[AreaTiling.TileIndexPositionFromPosition(subLevelEntrance.transform.position, containedArea.CenterPosition, containedArea.MainAreaDimensions)].Terrain
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
