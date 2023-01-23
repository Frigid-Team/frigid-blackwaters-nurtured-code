using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "DungeonGenerator", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.DUNGEON_GENERATION + "DungeonGenerator")]
    public class DungeonGenerator : FrigidScriptableObject
    {
        public const int SPAWN_X_POSITION_INDEX = 0;
        public const int SPAWN_Y_POSITION_INDEX = 0;

        [SerializeField]
        private IntSerializedReference numberOfRooms;
        [SerializeField]
        private IntSerializedReference minCorridorSize;
        [SerializeField]
        private IntSerializedReference maxCorridorSize;
        [SerializeField]
        private FloatSerializedReference meanCorridorSize;
        [SerializeField]
        private FloatSerializedReference stdDevCorridorSize;
        [SerializeField]
        private IntSerializedReference minNumOfSplits;
        [SerializeField]
        private IntSerializedReference maxNumOfSplits;
        [SerializeField]
        private FloatSerializedReference meanNumOfSplits;
        [SerializeField]
        private FloatSerializedReference stdDevNumOfSplits;
        [SerializeField]
        private List<LayoutContentsWeighting> layoutContentsWeightings;
        [SerializeField]
        private DungeonFinaleOutcome finaleOutcome;

        public GeneratedDungeon GenerateDungeon()
        {
            GeneratedDungeon generatedDungeon = new GeneratedDungeon();
            GenerateLayout(generatedDungeon);
            GenerateContent(generatedDungeon);
            return generatedDungeon;
        }

        private void GenerateLayout(GeneratedDungeon generatedDungeon)
        {
            Queue<GeneratedDungeonRoom> intersectionsQueue = new Queue<GeneratedDungeonRoom>();
            Vector2Int spawnRoomIndices = new Vector2Int(SPAWN_X_POSITION_INDEX, SPAWN_Y_POSITION_INDEX);
            GeneratedDungeonRoom spawnRoom = new GeneratedDungeonRoom(spawnRoomIndices);
            spawnRoom.RoomLayoutType = RoomLayoutType.Spawn;
            generatedDungeon.RoomPerIndices.Add(spawnRoomIndices, spawnRoom);
            generatedDungeon.CurrentlyGeneratedNumberOfRooms++;

            intersectionsQueue.Enqueue(spawnRoom);

            while (generatedDungeon.CurrentlyGeneratedNumberOfRooms < this.numberOfRooms.ImmutableValue)
            {
                GeneratedDungeonRoom currentRoom = intersectionsQueue.Dequeue();

                CreateDungeonRoomBranches(generatedDungeon, currentRoom, intersectionsQueue);

                if (intersectionsQueue.Count == 0)
                {
                    intersectionsQueue = RepopulateintersectionRoomsQueue(generatedDungeon, intersectionsQueue);
                }
            }

            foreach (GeneratedDungeonRoom room in intersectionsQueue)
            {
                if (room.AdjacentRooms.Count == 1 && room.RoomLayoutType != RoomLayoutType.Spawn)
                {
                    room.RoomLayoutType = RoomLayoutType.EndOfCorridor;
                }
            }

            DesignateCompletionRooms(generatedDungeon);
            PopulateRoomsPerLayouts(generatedDungeon);
#if UNITY_EDITOR
            Debug.Log("Dungeon layout dungeon generation finished!");
#endif
        }

        private void CreateDungeonRoomBranches(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom currentRoom, Queue<GeneratedDungeonRoom> intersectionRoomsQueue)
        {
            List<Vector2Int> availableIndices = GetAvailableNearbyIndices(generatedDungeon, currentRoom);

            if (availableIndices.Count == 0)
            {
                if (currentRoom.AdjacentRooms.Count == 1)
                {
                    currentRoom.RoomLayoutType = RoomLayoutType.EndOfCorridor;
                }
                return;
            }

            int numOfSplits = Probability.NormalDistribution(
                this.meanNumOfSplits.ImmutableValue,
                this.stdDevNumOfSplits.ImmutableValue,
                this.minNumOfSplits.ImmutableValue,
                Mathf.Min(this.maxNumOfSplits.ImmutableValue, availableIndices.Count)
                );

            int roomsLeft = this.numberOfRooms.ImmutableValue - generatedDungeon.CurrentlyGeneratedNumberOfRooms;

            if (numOfSplits > roomsLeft)
            {
                numOfSplits = roomsLeft;
            }

            int splitsCreated = 0;

            while (splitsCreated < numOfSplits)
            {
                int randomDirection = UnityEngine.Random.Range(0, availableIndices.Count);
                Vector2Int newRoomIndices = availableIndices[randomDirection];

                GeneratedDungeonRoom intersectionRoom = CreateDungeonCorridor(generatedDungeon, currentRoom, newRoomIndices - currentRoom.PositionIndices);
                intersectionRoomsQueue.Enqueue(intersectionRoom);

                availableIndices.RemoveAt(randomDirection);
                splitsCreated++;
            }
        }

        private List<Vector2Int> GetAvailableNearbyIndices(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom currentRoom)
        {
            List<Vector2Int> availableIndices = new List<Vector2Int>();

            if (!generatedDungeon.RoomPerIndices.ContainsKey(new Vector2Int(currentRoom.PositionIndices.x, currentRoom.PositionIndices.y + 1)))
            {
                availableIndices.Add(new Vector2Int(currentRoom.PositionIndices.x, currentRoom.PositionIndices.y + 1));
            }
            if (!generatedDungeon.RoomPerIndices.ContainsKey(new Vector2Int(currentRoom.PositionIndices.x + 1, currentRoom.PositionIndices.y)))
            {
                availableIndices.Add(new Vector2Int(currentRoom.PositionIndices.x + 1, currentRoom.PositionIndices.y));
            }
            if (!generatedDungeon.RoomPerIndices.ContainsKey(new Vector2Int(currentRoom.PositionIndices.x, currentRoom.PositionIndices.y - 1)) && 
                currentRoom.RoomLayoutType != RoomLayoutType.Spawn)
            {
                // We reserve the down direction for the spawn room, as it will likely be the entry from other levels.
                availableIndices.Add(new Vector2Int(currentRoom.PositionIndices.x, currentRoom.PositionIndices.y - 1));
            }
            if (!generatedDungeon.RoomPerIndices.ContainsKey(new Vector2Int(currentRoom.PositionIndices.x - 1, currentRoom.PositionIndices.y)))
            {
                availableIndices.Add(new Vector2Int(currentRoom.PositionIndices.x - 1, currentRoom.PositionIndices.y));
            }

            return availableIndices;
        }

        private GeneratedDungeonRoom CreateDungeonCorridor(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom branchRoom, Vector2Int relativeDirection)
        {
            int corridorLength = Probability.NormalDistribution(
                this.meanCorridorSize.ImmutableValue,
                this.stdDevCorridorSize.ImmutableValue,
                this.minCorridorSize.ImmutableValue,
                this.maxCorridorSize.ImmutableValue
                );

            if (generatedDungeon.CurrentlyGeneratedNumberOfRooms + corridorLength >= this.numberOfRooms.ImmutableValue - 1)
            {
                corridorLength = this.numberOfRooms.ImmutableValue - generatedDungeon.CurrentlyGeneratedNumberOfRooms - 1;
            }

            int roomsCreated = 0;
            GeneratedDungeonRoom currentEndOfCorridorRoom = branchRoom;

            while (roomsCreated < corridorLength)
            {
                Vector2Int newRoomPositionIndices = currentEndOfCorridorRoom.PositionIndices + relativeDirection;
                GeneratedDungeonRoom newCorridorRoom;

                if (!generatedDungeon.RoomPerIndices.ContainsKey(newRoomPositionIndices))
                {
                    newCorridorRoom = new GeneratedDungeonRoom(newRoomPositionIndices);
                    newCorridorRoom.AddAdjacentRoom(currentEndOfCorridorRoom);
                    newCorridorRoom.RoomLayoutType = RoomLayoutType.Corridor;
                    generatedDungeon.RoomPerIndices.Add(newRoomPositionIndices, newCorridorRoom);
                    generatedDungeon.CurrentlyGeneratedNumberOfRooms++;
                }
                else if (generatedDungeon.RoomPerIndices[newRoomPositionIndices].RoomLayoutType != RoomLayoutType.Spawn)
                {
                    generatedDungeon.RoomPerIndices[newRoomPositionIndices].AddAdjacentRoom(generatedDungeon.RoomPerIndices[currentEndOfCorridorRoom.PositionIndices]);
                    newCorridorRoom = generatedDungeon.RoomPerIndices[newRoomPositionIndices];
                }
                else
                {
                    currentEndOfCorridorRoom.RoomLayoutType = RoomLayoutType.Intersection;
                    return currentEndOfCorridorRoom;
                }

                currentEndOfCorridorRoom.AddAdjacentRoom(newCorridorRoom);
                currentEndOfCorridorRoom = newCorridorRoom;
                roomsCreated++;
            }

            GeneratedDungeonRoom intersectionRoom = CreateIntersectionRoom(generatedDungeon, currentEndOfCorridorRoom, relativeDirection);

            return intersectionRoom;
        }

        private GeneratedDungeonRoom CreateIntersectionRoom(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom lastCorridorRoom, Vector2Int relativeDirection)
        {
            if (generatedDungeon.CurrentlyGeneratedNumberOfRooms < this.numberOfRooms.ImmutableValue)
            {
                Vector2Int newRoomPositionIndices = lastCorridorRoom.PositionIndices + relativeDirection;
                if (!generatedDungeon.RoomPerIndices.ContainsKey(new Vector2Int(newRoomPositionIndices.x, newRoomPositionIndices.y)))
                {
                    GeneratedDungeonRoom intersectionRoom = new GeneratedDungeonRoom(newRoomPositionIndices);
                    intersectionRoom.AddAdjacentRoom(lastCorridorRoom);
                    intersectionRoom.RoomLayoutType = RoomLayoutType.Intersection;
                    generatedDungeon.RoomPerIndices[newRoomPositionIndices] = intersectionRoom;
                    generatedDungeon.CurrentlyGeneratedNumberOfRooms++;
                }
                else if (generatedDungeon.RoomPerIndices[newRoomPositionIndices].RoomLayoutType == RoomLayoutType.Spawn)
                {
                    lastCorridorRoom.RoomLayoutType = RoomLayoutType.Intersection;
                    return lastCorridorRoom;
                }
                else
                {
                    generatedDungeon.RoomPerIndices[newRoomPositionIndices].RoomLayoutType = RoomLayoutType.Intersection;
                    generatedDungeon.RoomPerIndices[newRoomPositionIndices].AddAdjacentRoom(lastCorridorRoom);
                }
                generatedDungeon.RoomPerIndices[lastCorridorRoom.PositionIndices].AddAdjacentRoom(generatedDungeon.RoomPerIndices[newRoomPositionIndices]);
                return generatedDungeon.RoomPerIndices[newRoomPositionIndices];
            }
            lastCorridorRoom.RoomLayoutType = RoomLayoutType.Intersection;
            return lastCorridorRoom;
        }

        private void DesignateCompletionRooms(GeneratedDungeon generatedDungeon)
        {
            RoomLayoutType[] finaleLayoutTypes = null;
            switch (this.finaleOutcome)
            {
                case DungeonFinaleOutcome.None:
                    return;
                case DungeonFinaleOutcome.GenerateBoss:
                    finaleLayoutTypes = new RoomLayoutType[] { RoomLayoutType.Boss };
                    break;
                case DungeonFinaleOutcome.GenerateFloorExit:
                    finaleLayoutTypes = new RoomLayoutType[] { RoomLayoutType.FloorExit };
                    break;
                case DungeonFinaleOutcome.GenerateBossAndFloorExit:
                    finaleLayoutTypes = new RoomLayoutType[] { RoomLayoutType.Boss, RoomLayoutType.FloorExit };
                    break;
            }

            List<GeneratedDungeonRoom> endOfCorridorRooms = new List<GeneratedDungeonRoom>();
            foreach (GeneratedDungeonRoom room in generatedDungeon.Rooms)
            {
                if (room.RoomLayoutType == RoomLayoutType.EndOfCorridor)
                {
                    endOfCorridorRooms.Add(room);
                }
            }

            if (endOfCorridorRooms.Count > 0)
            {
                GeneratedDungeonRoom designatedRoom = endOfCorridorRooms[UnityEngine.Random.Range(0, endOfCorridorRooms.Count)];
                designatedRoom.RoomLayoutType = finaleLayoutTypes[0];

                List<Vector2Int> availableIndices = GetAvailableNearbyIndices(generatedDungeon, designatedRoom);
                if (finaleLayoutTypes.Length > 1 && availableIndices.Count > 0)
                {
                    GeneratedDungeonRoom floorExitRoom = new GeneratedDungeonRoom(availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)]);
                    floorExitRoom.RoomLayoutType = finaleLayoutTypes[1];
                    generatedDungeon.RoomPerIndices.Add(floorExitRoom.PositionIndices, floorExitRoom);
                    designatedRoom.AddAdjacentRoom(floorExitRoom);
                    floorExitRoom.AddAdjacentRoom(designatedRoom);
                }
                else
                {
                    Debug.LogWarning("Seems like there are no available indices to place the next floor room.");
                }

                return;
            }
#if UNITY_EDITOR
            Debug.LogWarning("No end of corridor rooms in dungeon " + this.name + ".");
#endif
        }

        private Queue<GeneratedDungeonRoom> RepopulateintersectionRoomsQueue(GeneratedDungeon generatedDungeon, Queue<GeneratedDungeonRoom> intersectionRoomsQueue)
        {
            foreach (GeneratedDungeonRoom room in generatedDungeon.Rooms)
            {
                if (room.RoomLayoutType == RoomLayoutType.Intersection)
                {
                    intersectionRoomsQueue.Enqueue(room);
                }
            }

#if UNITY_EDITOR
            Debug.Log("CPR initiated... giving new branches to the empty dungeon...");
#endif
            return intersectionRoomsQueue;
        }

        private void PopulateRoomsPerLayouts(GeneratedDungeon generatedDungeon)
        {
            foreach (GeneratedDungeonRoom room in generatedDungeon.Rooms)
            {
                generatedDungeon.RoomsPerLayouts[room.RoomLayoutType].Add(room);
            }
        }

        private void GenerateContent(GeneratedDungeon generatedDungeon)
        {
            Dictionary<RoomLayoutType, RelativeWeightPool<RoomContentType>> layoutToContentsMap = new Dictionary<RoomLayoutType, RelativeWeightPool<RoomContentType>>();
            foreach (LayoutContentsWeighting layoutContentsWeighting in this.layoutContentsWeightings)
            {
                layoutToContentsMap.Add(layoutContentsWeighting.LayoutType, layoutContentsWeighting.ContentTypes);
            }

            foreach (KeyValuePair<RoomLayoutType, List<GeneratedDungeonRoom>> roomsPerLayout in generatedDungeon.RoomsPerLayouts)
            {
                switch (roomsPerLayout.Key)
                {
                    case RoomLayoutType.None:
                        continue;
                    case RoomLayoutType.Spawn:
                        foreach (GeneratedDungeonRoom room in roomsPerLayout.Value)
                        {
                            room.RoomContentType = RoomContentType.Spawn;
                        }
                        continue;
                    case RoomLayoutType.Boss:
                        foreach (GeneratedDungeonRoom room in roomsPerLayout.Value)
                        {
                            room.RoomContentType = RoomContentType.Boss;
                        }
                        continue;
                    case RoomLayoutType.FloorExit:
                        foreach (GeneratedDungeonRoom room in roomsPerLayout.Value)
                        {
                            room.RoomContentType = RoomContentType.FloorExit;
                        }
                        continue;
                }

                RoomContentType[] contentTypes = layoutToContentsMap[roomsPerLayout.Key].Retrieve(roomsPerLayout.Value.Count);
                for (int i = 0; i < roomsPerLayout.Value.Count; i++)
                {
                    roomsPerLayout.Value[i].RoomContentType = contentTypes[i];
                }
            }

            PopulateRoomsPerContents(generatedDungeon);
#if UNITY_EDITOR
            Debug.Log("Room content dungeon generation finished!");
#endif
        }

        private void PopulateRoomsPerContents(GeneratedDungeon generatedDungeon)
        {
            foreach (GeneratedDungeonRoom room in generatedDungeon.Rooms)
            {
                generatedDungeon.RoomsPerContents[room.RoomContentType].Add(room);
            }
        }

        [Serializable]
        private struct LayoutContentsWeighting
        {
            [SerializeField]
            private RoomLayoutType layoutType;
            [SerializeField]
            private RelativeWeightPool<RoomContentType> contentTypes;

            public RoomLayoutType LayoutType
            {
                get
                {
                    return this.layoutType;
                }
                set
                {
                    this.layoutType = value;
                }
            }

            public RelativeWeightPool<RoomContentType> ContentTypes
            {
                get
                {
                    return this.contentTypes;
                }
                set
                {
                    this.contentTypes = value;
                }
            }
        }

        private enum DungeonFinaleOutcome
        {
            None,
            GenerateBoss,
            GenerateFloorExit,
            GenerateBossAndFloorExit
        }
    }
}
