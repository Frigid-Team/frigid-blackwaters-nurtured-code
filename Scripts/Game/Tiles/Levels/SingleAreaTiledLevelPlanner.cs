using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "SingleAreaTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "SingleAreaTiledLevelPlanner")]
    public class SingleAreaTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private RelativeWeightPool<TiledAreaBlueprintGroup> desiredBlueprintGroups;
        [SerializeField]
        private TiledArea singleAreaPrefab;
        [SerializeField]
        private TiledAreaEntrance exitEntrancePrefab;
        [SerializeField]
        private TiledAreaMobGenerator areaMobGenerator;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TiledLevelPlanArea planArea = new TiledLevelPlanArea(this.singleAreaPrefab, this.desiredBlueprintGroups.Retrieve());
            TiledLevelPlan levelPlan = new TiledLevelPlan(planArea, this.areaMobGenerator);

            if (subLevelEntrancesAndContainedAreas.Count > 1)
            {
                Debug.LogError("SingleAreaTiledLevelPlanners cannot support more than 1 sub level entrance.");
                return levelPlan;
            }

            foreach (TiledAreaEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                TiledArea containedArea = subLevelEntrancesAndContainedAreas[subLevelEntrance];
                levelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(planArea, this.exitEntrancePrefab),
                        Vector2Int.up,
                        containedArea.NavigationGrid.TerrainAtTile(TilePositioning.TileIndicesFromPosition(subLevelEntrance.transform.position, containedArea.CenterPosition, containedArea.MainAreaDimensions))
                        )
                    );
            }

            return levelPlan;
        }
    }
}
