using UnityEngine;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "SingleAreaTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "SingleAreaTiledLevelPlanner")]
    public class SingleAreaTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private RelativeWeightPool<TiledAreaBlueprintGroup> desiredBlueprintGroups;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            TiledLevelPlanArea planArea = new TiledLevelPlanArea(this.desiredBlueprintGroups.Retrieve());
            TiledLevelPlan levelPlan = new TiledLevelPlan(planArea);

            if (subLevelEntrancesAndContainedAreas.Count > 1)
            {
                Debug.LogError("SingleAreaTiledLevelPlanners cannot support more than 1 sub level entrance.");
                return levelPlan;
            }

            if (subLevelEntrancesAndContainedAreas.Count > 0)
            {
                KeyValuePair<TiledEntrance, TiledArea> entranceAndContainedArea = subLevelEntrancesAndContainedAreas.First();
                TiledEntrance subLevelEntrance = entranceAndContainedArea.Key;
                TiledArea containedArea = entranceAndContainedArea.Value;
                levelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(planArea),
                        subLevelEntrance.LocalEntryIndexDirection,
                        containedArea.NavigationGrid[AreaTiling.TileIndexPositionFromPosition(subLevelEntrance.EntryPosition, containedArea.CenterPosition, containedArea.MainAreaDimensions)].Terrain
                        )
                    );
                if (subLevelEntrancesAndContainedAreas.Count > 1)
                {
                    Debug.LogWarning("PredefinedTiledLevelPlanners cannot support more than 1 sub level entrance.");
                }
            }

            return levelPlan;
        }
    }
}
