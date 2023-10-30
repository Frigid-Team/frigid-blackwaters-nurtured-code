using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "DungeonTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "DungeonTiledLevelPlanner")]
    public class DungeonTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private DungeonGenerator dungeonGenerator;
        [SerializeField]
        private List<RoomContentBlueprintGroup> roomContentBlueprintGroups;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            Dictionary<RoomContentType, RelativeWeightPool<TiledAreaBlueprintGroup>> roomContentBlueprintGroupMap = new Dictionary<RoomContentType, RelativeWeightPool<TiledAreaBlueprintGroup>>();
            foreach (RoomContentBlueprintGroup roomContentBlueprintGroup in this.roomContentBlueprintGroups)
            {
                roomContentBlueprintGroupMap.Add(roomContentBlueprintGroup.RoomContentType, roomContentBlueprintGroup.BlueprintGroups);
            }

            List<Vector2Int> subLevelIndexDirections = new List<Vector2Int>();
            foreach (TiledEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                subLevelIndexDirections.Add(-subLevelEntrance.LocalEntryIndexDirection);
            }

            GeneratedDungeon generatedDungeon = this.dungeonGenerator.GenerateDungeon(Vector2Int.zero, subLevelIndexDirections);

            GeneratedDungeonRoom spawnRoom = generatedDungeon.RoomPerIndexPosition[Vector2Int.zero];
            TiledLevelPlanArea spawnRoomPlanArea = new TiledLevelPlanArea(roomContentBlueprintGroupMap[spawnRoom.RoomContentType].Retrieve());

            TiledLevelPlan dungeonLevelPlan = new TiledLevelPlan(spawnRoomPlanArea);

            foreach (TiledEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                TiledArea containedArea = subLevelEntrancesAndContainedAreas[subLevelEntrance];
                dungeonLevelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(spawnRoomPlanArea),
                        subLevelEntrance.LocalEntryIndexDirection,
                        containedArea.NavigationGrid[AreaTiling.TileIndexPositionFromPosition(subLevelEntrance.EntryPosition, containedArea.CenterPosition, containedArea.MainAreaDimensions)].Terrain
                        )
                    );
            }

            Dictionary<GeneratedDungeonRoom, TiledLevelPlanArea> visitedRooms = new Dictionary<GeneratedDungeonRoom, TiledLevelPlanArea>();
            Queue<(GeneratedDungeonRoom generatedDungeonRoom, TiledLevelPlanArea planArea)> roomCreationsToVisit = new Queue<(GeneratedDungeonRoom generatedDungeonRoom, TiledLevelPlanArea planArea)>();
            roomCreationsToVisit.Enqueue((spawnRoom, spawnRoomPlanArea));
            visitedRooms.Add(spawnRoom, spawnRoomPlanArea);

            Dictionary<RoomContentType, List<TiledAreaBlueprintGroup>> blueprintGroupsRetrievals = new Dictionary<RoomContentType, List<TiledAreaBlueprintGroup>>();
            foreach (KeyValuePair<RoomContentType, List<GeneratedDungeonRoom>> roomsPerContent in generatedDungeon.RoomsPerContents)
            {
                if (roomContentBlueprintGroupMap.ContainsKey(roomsPerContent.Key))
                {
                    blueprintGroupsRetrievals.Add(roomsPerContent.Key, roomContentBlueprintGroupMap[roomsPerContent.Key].Retrieve(roomsPerContent.Value.Count).ToList());
                }
                else
                {
                    blueprintGroupsRetrievals.Add(roomsPerContent.Key, roomContentBlueprintGroupMap[RoomContentType.Combat].Retrieve(roomsPerContent.Value.Count).ToList());
                }
            }

            while (roomCreationsToVisit.Count > 0)
            {
                (GeneratedDungeonRoom generatedDungeonRoom, TiledLevelPlanArea planArea) currentRoomPair = roomCreationsToVisit.Dequeue();

                foreach (GeneratedDungeonRoom adjacentRoom in currentRoomPair.generatedDungeonRoom.AdjacentRooms.Values)
                {
                    if (!visitedRooms.ContainsKey(adjacentRoom))
                    {
                        List<TiledAreaBlueprintGroup> blueprintGroupsRetrieval = blueprintGroupsRetrievals[adjacentRoom.RoomContentType];
                        TiledLevelPlanArea newPlanArea = new TiledLevelPlanArea(blueprintGroupsRetrieval[blueprintGroupsRetrieval.Count - 1]);
                        blueprintGroupsRetrieval.RemoveAt(blueprintGroupsRetrieval.Count - 1);
                        visitedRooms.Add(adjacentRoom, newPlanArea);
                        dungeonLevelPlan.AddArea(newPlanArea);
                        roomCreationsToVisit.Enqueue((adjacentRoom, newPlanArea));
                    }

                    Vector2Int entryIndexDirection = adjacentRoom.IndexPosition - currentRoomPair.generatedDungeonRoom.IndexPosition;
                    if (currentRoomPair.planArea.RemainingWallEntryIndexDirections.Contains(entryIndexDirection))
                    {
                        TiledLevelPlanEntrance currentRoomPlanEntrance = new TiledLevelPlanEntrance(currentRoomPair.planArea);
                        TiledLevelPlanEntrance adjacentRoomPlanEntrance = new TiledLevelPlanEntrance(visitedRooms[adjacentRoom]);
                        dungeonLevelPlan.AddConnection(
                            new TiledLevelPlanConnection(
                                currentRoomPlanEntrance,
                                adjacentRoomPlanEntrance,
                                entryIndexDirection
                                )
                            );
                    }
                }
            }

            return dungeonLevelPlan;
        }

        [Serializable]
        private struct RoomContentBlueprintGroup
        {
            [SerializeField]
            private RoomContentType roomContentType;
            [SerializeField]
            private RelativeWeightPool<TiledAreaBlueprintGroup> blueprintGroups;

            public RoomContentType RoomContentType
            {
                get
                {
                    return this.roomContentType;
                }
            }

            public RelativeWeightPool<TiledAreaBlueprintGroup> BlueprintGroups
            {
                get
                {
                    return this.blueprintGroups;
                }
            }
        }
    }
}
