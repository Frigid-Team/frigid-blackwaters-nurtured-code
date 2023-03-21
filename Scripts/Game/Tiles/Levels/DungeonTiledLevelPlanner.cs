using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "DungeonTiledLevelPlanner", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "DungeonTiledLevelPlanner")]
    public class DungeonTiledLevelPlanner : TiledLevelPlanner
    {
        [SerializeField]
        private DungeonGenerator dungeonGenerator;
        [SerializeField]
        private List<RoomContentBlueprintGroup> roomContentBlueprintGroups;
        [SerializeField]
        private TiledArea dungeonRoomPrefab;
        [SerializeField]
        private TiledAreaEntrance dungeonEntrancePrefab;
        [SerializeField]
        private TiledAreaEntrance bossEntrancePrefab;
        [SerializeField]
        private TiledAreaEntrance exitEntrancePrefab;
        [SerializeField]
        private TiledAreaMobGenerator dungeonMobGenerator;

        protected override TiledLevelPlan CreateInitialLevelPlan(Dictionary<TiledAreaEntrance, TiledArea> subLevelEntrancesAndContainedAreas)
        {
            Dictionary<RoomContentType, RelativeWeightPool<TiledAreaBlueprintGroup>> roomContentBlueprintGroupMap = new Dictionary<RoomContentType, RelativeWeightPool<TiledAreaBlueprintGroup>>();
            foreach (RoomContentBlueprintGroup roomContentBlueprintGroup in this.roomContentBlueprintGroups)
            {
                roomContentBlueprintGroupMap.Add(roomContentBlueprintGroup.RoomContentType, roomContentBlueprintGroup.BlueprintGroups);
            }

            GeneratedDungeon generatedDungeon = this.dungeonGenerator.GenerateDungeon();

            GeneratedDungeonRoom spawnRoom = generatedDungeon.RoomPerIndices[new Vector2Int(DungeonGenerator.SPAWN_X_POSITION_INDEX, DungeonGenerator.SPAWN_Y_POSITION_INDEX)];
            TiledLevelPlanArea spawnRoomPlanArea = new TiledLevelPlanArea(this.dungeonRoomPrefab, roomContentBlueprintGroupMap[spawnRoom.RoomContentType].Retrieve());

            TiledLevelPlan dungeonLevelPlan = new TiledLevelPlan(spawnRoomPlanArea, this.dungeonMobGenerator);

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
                        TiledLevelPlanArea newPlanArea = new TiledLevelPlanArea(this.dungeonRoomPrefab, blueprintGroupsRetrieval[blueprintGroupsRetrieval.Count - 1]);
                        blueprintGroupsRetrieval.RemoveAt(blueprintGroupsRetrieval.Count - 1);
                        visitedRooms.Add(adjacentRoom, newPlanArea);
                        dungeonLevelPlan.AddArea(newPlanArea);
                        roomCreationsToVisit.Enqueue((adjacentRoom, newPlanArea));
                    }

                    Vector2Int entryDirection = adjacentRoom.PositionIndices - currentRoomPair.generatedDungeonRoom.PositionIndices;
                    if (currentRoomPair.planArea.RemainingWallEntryDirections.Contains(entryDirection))
                    {
                        TiledAreaEntrance chosenEntrancePrefab = this.dungeonEntrancePrefab;
                        if (currentRoomPair.generatedDungeonRoom.RoomContentType == RoomContentType.Boss || adjacentRoom.RoomContentType == RoomContentType.Boss)
                        {
                            chosenEntrancePrefab = this.bossEntrancePrefab;
                        }
                        TiledLevelPlanEntrance currentRoomPlanEntrance = new TiledLevelPlanEntrance(currentRoomPair.planArea, chosenEntrancePrefab);
                        TiledLevelPlanEntrance adjacentRoomPlanEntrance = new TiledLevelPlanEntrance(visitedRooms[adjacentRoom], chosenEntrancePrefab);
                        dungeonLevelPlan.AddConnection(
                            new TiledLevelPlanConnection(
                                currentRoomPlanEntrance,
                                adjacentRoomPlanEntrance,
                                entryDirection
                                )
                            );
                    }
                }
            }

            if (subLevelEntrancesAndContainedAreas.Count > 1)
            {
                Debug.LogError("DungeonTiledLevelPlanners cannot support more than 1 sub level entrance.");
                return dungeonLevelPlan;
            }

            foreach (TiledAreaEntrance subLevelEntrance in subLevelEntrancesAndContainedAreas.Keys)
            {
                TiledArea containedArea = subLevelEntrancesAndContainedAreas[subLevelEntrance];
                dungeonLevelPlan.AddConnection(
                    new TiledLevelPlanConnection(
                        new TiledLevelPlanEntrance(subLevelEntrance),
                        new TiledLevelPlanEntrance(spawnRoomPlanArea, this.exitEntrancePrefab),
                        Vector2Int.up,
                        containedArea.NavigationGrid.TerrainAtTile(TilePositioning.TileIndicesFromPosition(subLevelEntrance.transform.position, containedArea.CenterPosition, containedArea.MainAreaDimensions))
                        )
                    );
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
