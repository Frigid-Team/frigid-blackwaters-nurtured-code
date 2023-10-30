using System;
using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "DungeonGenerator", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.DungeonGeneration + "DungeonGenerator")]
    public class DungeonGenerator : FrigidScriptableObject
    {
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

        public GeneratedDungeon GenerateDungeon(Vector2Int startingIndexPosition, List<Vector2Int> restrictedIndexPositions)
        {
            GeneratedDungeon generatedDungeon = new GeneratedDungeon();
            this.GenerateLayout(generatedDungeon, startingIndexPosition, restrictedIndexPositions);
            this.GenerateContent(generatedDungeon);
            return generatedDungeon;
        }

        private void GenerateLayout(GeneratedDungeon generatedDungeon, Vector2Int startingIndexPosition, List<Vector2Int> restrictedIndexPositions)
        {
            Queue<GeneratedDungeonRoom> intersectionsQueue = new Queue<GeneratedDungeonRoom>();
            Vector2Int spawnRoomIndexPosition = startingIndexPosition;
            GeneratedDungeonRoom spawnRoom = new GeneratedDungeonRoom(spawnRoomIndexPosition);
            spawnRoom.RoomLayoutType = RoomLayoutType.Spawn;
            generatedDungeon.RoomPerIndexPosition.Add(spawnRoomIndexPosition, spawnRoom);
            generatedDungeon.CurrentlyGeneratedNumberOfRooms++;

            intersectionsQueue.Enqueue(spawnRoom);

            while (generatedDungeon.CurrentlyGeneratedNumberOfRooms < this.numberOfRooms.ImmutableValue)
            {
                GeneratedDungeonRoom currentRoom = intersectionsQueue.Dequeue();

                this.CreateDungeonRoomBranches(generatedDungeon, currentRoom, intersectionsQueue, restrictedIndexPositions);

                if (intersectionsQueue.Count == 0)
                {
                    intersectionsQueue = this.RepopulateintersectionRoomsQueue(generatedDungeon, intersectionsQueue);
                }
            }

            foreach (GeneratedDungeonRoom room in intersectionsQueue)
            {
                if (room.AdjacentRooms.Count == 1 && room.RoomLayoutType != RoomLayoutType.Spawn)
                {
                    room.RoomLayoutType = RoomLayoutType.EndOfCorridor;
                }
            }

            this.DesignateCompletionRooms(generatedDungeon, restrictedIndexPositions);
            this.PopulateRoomsPerLayouts(generatedDungeon);
#if UNITY_EDITOR
            Debug.Log("Dungeon layout generation finished!");
#endif
        }

        private void CreateDungeonRoomBranches(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom currentRoom, Queue<GeneratedDungeonRoom> intersectionRoomsQueue, List<Vector2Int> restrictedIndexPositions)
        {
            List<Vector2Int> availableIndexPositions = this.GetAvailableNearbyIndexPositions(generatedDungeon, currentRoom, restrictedIndexPositions);

            if (availableIndexPositions.Count == 0)
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
                Mathf.Min(this.maxNumOfSplits.ImmutableValue, availableIndexPositions.Count)
                );

            int roomsLeft = this.numberOfRooms.ImmutableValue - generatedDungeon.CurrentlyGeneratedNumberOfRooms;

            if (numOfSplits > roomsLeft)
            {
                numOfSplits = roomsLeft;
            }

            int splitsCreated = 0;

            while (splitsCreated < numOfSplits)
            {
                int randomDirection = UnityEngine.Random.Range(0, availableIndexPositions.Count);
                Vector2Int newRoomIndexPosition = availableIndexPositions[randomDirection];

                GeneratedDungeonRoom intersectionRoom = this.CreateDungeonCorridor(generatedDungeon, currentRoom, newRoomIndexPosition - currentRoom.IndexPosition);
                intersectionRoomsQueue.Enqueue(intersectionRoom);

                availableIndexPositions.RemoveAt(randomDirection);
                splitsCreated++;
            }
        }

        private List<Vector2Int> GetAvailableNearbyIndexPositions(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom currentRoom, List<Vector2Int> restrictedIndexPositions)
        {
            List<Vector2Int> availableIndexPositions = new List<Vector2Int>();

            if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(new Vector2Int(currentRoom.IndexPosition.x, currentRoom.IndexPosition.y + 1)))
            {
                availableIndexPositions.Add(new Vector2Int(currentRoom.IndexPosition.x, currentRoom.IndexPosition.y + 1));
            }
            if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(new Vector2Int(currentRoom.IndexPosition.x + 1, currentRoom.IndexPosition.y)))
            {
                availableIndexPositions.Add(new Vector2Int(currentRoom.IndexPosition.x + 1, currentRoom.IndexPosition.y));
            }
            if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(new Vector2Int(currentRoom.IndexPosition.x, currentRoom.IndexPosition.y - 1)))
            {
                availableIndexPositions.Add(new Vector2Int(currentRoom.IndexPosition.x, currentRoom.IndexPosition.y - 1));
            }
            if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(new Vector2Int(currentRoom.IndexPosition.x - 1, currentRoom.IndexPosition.y)))
            {
                availableIndexPositions.Add(new Vector2Int(currentRoom.IndexPosition.x - 1, currentRoom.IndexPosition.y));
            }

            availableIndexPositions.RemoveAll((Vector2Int nearbyIndexPosition) => restrictedIndexPositions.Contains(nearbyIndexPosition));

            return availableIndexPositions;
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
                Vector2Int newRoomIndexPosition = currentEndOfCorridorRoom.IndexPosition + relativeDirection;
                GeneratedDungeonRoom newCorridorRoom;

                if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(newRoomIndexPosition))
                {
                    newCorridorRoom = new GeneratedDungeonRoom(newRoomIndexPosition);
                    newCorridorRoom.AddAdjacentRoom(currentEndOfCorridorRoom);
                    newCorridorRoom.RoomLayoutType = RoomLayoutType.Corridor;
                    generatedDungeon.RoomPerIndexPosition.Add(newRoomIndexPosition, newCorridorRoom);
                    generatedDungeon.CurrentlyGeneratedNumberOfRooms++;
                }
                else if (generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition].RoomLayoutType != RoomLayoutType.Spawn)
                {
                    generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition].AddAdjacentRoom(generatedDungeon.RoomPerIndexPosition[currentEndOfCorridorRoom.IndexPosition]);
                    newCorridorRoom = generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition];
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

            GeneratedDungeonRoom intersectionRoom = this.CreateIntersectionRoom(generatedDungeon, currentEndOfCorridorRoom, relativeDirection);

            return intersectionRoom;
        }

        private GeneratedDungeonRoom CreateIntersectionRoom(GeneratedDungeon generatedDungeon, GeneratedDungeonRoom lastCorridorRoom, Vector2Int relativeDirection)
        {
            if (generatedDungeon.CurrentlyGeneratedNumberOfRooms < this.numberOfRooms.ImmutableValue)
            {
                Vector2Int newRoomIndexPosition = lastCorridorRoom.IndexPosition + relativeDirection;
                if (!generatedDungeon.RoomPerIndexPosition.ContainsKey(new Vector2Int(newRoomIndexPosition.x, newRoomIndexPosition.y)))
                {
                    GeneratedDungeonRoom intersectionRoom = new GeneratedDungeonRoom(newRoomIndexPosition);
                    intersectionRoom.AddAdjacentRoom(lastCorridorRoom);
                    intersectionRoom.RoomLayoutType = RoomLayoutType.Intersection;
                    generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition] = intersectionRoom;
                    generatedDungeon.CurrentlyGeneratedNumberOfRooms++;
                }
                else if (generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition].RoomLayoutType == RoomLayoutType.Spawn)
                {
                    lastCorridorRoom.RoomLayoutType = RoomLayoutType.Intersection;
                    return lastCorridorRoom;
                }
                else
                {
                    generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition].RoomLayoutType = RoomLayoutType.Intersection;
                    generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition].AddAdjacentRoom(lastCorridorRoom);
                }
                generatedDungeon.RoomPerIndexPosition[lastCorridorRoom.IndexPosition].AddAdjacentRoom(generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition]);
                return generatedDungeon.RoomPerIndexPosition[newRoomIndexPosition];
            }
            lastCorridorRoom.RoomLayoutType = RoomLayoutType.Intersection;
            return lastCorridorRoom;
        }

        private void DesignateCompletionRooms(GeneratedDungeon generatedDungeon, List<Vector2Int> restrictedIndexPositions)
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

                List<Vector2Int> availableIndexPositions = this.GetAvailableNearbyIndexPositions(generatedDungeon, designatedRoom, restrictedIndexPositions);
                if (finaleLayoutTypes.Length > 1)
                {
                    if (availableIndexPositions.Count > 0)
                    {
                        GeneratedDungeonRoom floorExitRoom = new GeneratedDungeonRoom(availableIndexPositions[UnityEngine.Random.Range(0, availableIndexPositions.Count)]);
                        floorExitRoom.RoomLayoutType = finaleLayoutTypes[1];
                        generatedDungeon.RoomPerIndexPosition.Add(floorExitRoom.IndexPosition, floorExitRoom);
                        designatedRoom.AddAdjacentRoom(floorExitRoom);
                        floorExitRoom.AddAdjacentRoom(designatedRoom);
                    }
                    else
                    {
                        Debug.LogWarning("Seems like there are no available index positions to place the next floor room.");
                    }
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

            this.PopulateRoomsPerContents(generatedDungeon);
#if UNITY_EDITOR
            Debug.Log("Dungeon room content generation finished!");
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
