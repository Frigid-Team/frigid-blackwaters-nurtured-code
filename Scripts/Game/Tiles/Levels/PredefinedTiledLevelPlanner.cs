using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "PredefinedTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "PredefinedTiledLevelPlanner")]
    public class PredefinedTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private TiledArea areaPrefab;
        [SerializeField]
        private List<AreaPlacement> placements;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            if (this.placements.Count == 0)
            {
                throw new Exception("No placements for PredefinedTiledLevelPlanner " + this.name + ".");
            }

            TiledLevelPlanArea mainPlanArea = new TiledLevelPlanArea(this.placements[0].BlueprintGroup);
            TiledLevelPlan levelPlan = new TiledLevelPlan(mainPlanArea);

            Dictionary<Vector2Int, TiledLevelPlanArea> areaPositionMap = new Dictionary<Vector2Int, TiledLevelPlanArea>();
            areaPositionMap.Add(this.placements[0].IndexPosition, mainPlanArea);

            if (subLevelEntrancesAndContainedAreas.Count > 0)
            {
                KeyValuePair<TiledEntrance, TiledArea> entranceAndContainedArea = subLevelEntrancesAndContainedAreas.First();
                TiledEntrance subLevelEntrance = entranceAndContainedArea.Key;
                TiledArea containedArea = entranceAndContainedArea.Value;
                levelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance), 
                        new TiledLevelPlanEntrance(mainPlanArea), 
                        subLevelEntrance.LocalEntryIndexDirection, 
                        containedArea.NavigationGrid[AreaTiling.TileIndexPositionFromPosition(subLevelEntrance.EntryPosition, containedArea.CenterPosition, containedArea.MainAreaDimensions)].Terrain
                        )
                    );
                if (subLevelEntrancesAndContainedAreas.Count > 1)
                {
                    Debug.LogWarning("PredefinedTiledLevelPlanners cannot support more than 1 sub level entrance.");
                }
            }

            foreach (AreaPlacement placement in this.placements)
            {
                if (placement.Equals(this.placements[0])) continue;

                TiledLevelPlanArea placementArea = new TiledLevelPlanArea(placement.BlueprintGroup);
                levelPlan.AddArea(placementArea);

                areaPositionMap.TryAdd(placement.IndexPosition, placementArea);
                foreach (Vector2Int wallIndexDirection in WallTiling.GetAllWallIndexDirections())
                {
                    if (areaPositionMap.TryGetValue(placement.IndexPosition + wallIndexDirection, out TiledLevelPlanArea adjacentArea))
                    {
                        levelPlan.AddConnection(new TiledLevelPlanConnection(new TiledLevelPlanEntrance(placementArea), new TiledLevelPlanEntrance(adjacentArea), wallIndexDirection));
                    }
                }
            }

            return levelPlan;
        }

        [Serializable]
        private struct AreaPlacement
        {
            [SerializeField]
            private TiledAreaBlueprintGroup blueprintGroup;
            [SerializeField]
            private Vector2Int indexPosition;

            public TiledAreaBlueprintGroup BlueprintGroup
            {
                get
                {
                    return this.blueprintGroup;
                }
            }

            public Vector2Int IndexPosition
            {
                get
                {
                    return this.indexPosition;
                }
            }
        }
    }
}
