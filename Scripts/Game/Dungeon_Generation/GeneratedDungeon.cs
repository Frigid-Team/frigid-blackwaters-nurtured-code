using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class GeneratedDungeon
    {
        private Dictionary<Vector2Int, GeneratedDungeonRoom> roomPerIndexPositions;
        private Dictionary<RoomLayoutType, List<GeneratedDungeonRoom>> roomsPerLayouts;
        private Dictionary<RoomContentType, List<GeneratedDungeonRoom>> roomsPerContents;
        private int currentlyGeneratedNumberOfRooms;

        public GeneratedDungeon()
        {
            this.roomPerIndexPositions = new Dictionary<Vector2Int, GeneratedDungeonRoom>();
            this.roomsPerLayouts = new Dictionary<RoomLayoutType, List<GeneratedDungeonRoom>>();
            for (int i = 0; i < (int)RoomLayoutType.Count; i++)
            {
                this.roomsPerLayouts.Add((RoomLayoutType)i, new List<GeneratedDungeonRoom>());
            }
            this.roomsPerContents = new Dictionary<RoomContentType, List<GeneratedDungeonRoom>>();
            for (int i = 0; i < (int)RoomContentType.Count; i++)
            {
                this.roomsPerContents.Add((RoomContentType)i, new List<GeneratedDungeonRoom>());
            }
            this.currentlyGeneratedNumberOfRooms = 0;
        }

        public List<GeneratedDungeonRoom> Rooms
        {
            get
            {
                return this.roomPerIndexPositions.Values.ToList();
            }
        }

        public Dictionary<Vector2Int, GeneratedDungeonRoom> RoomPerIndexPosition
        {
            get
            {
                return this.roomPerIndexPositions;
            }
        }

        public Dictionary<RoomLayoutType, List<GeneratedDungeonRoom>> RoomsPerLayouts
        {
            get
            {
                return this.roomsPerLayouts;
            }
        }

        public Dictionary<RoomContentType, List<GeneratedDungeonRoom>> RoomsPerContents
        {
            get
            {
                return this.roomsPerContents;
            }
        }

        public int CurrentlyGeneratedNumberOfRooms
        {
            get
            {
                return this.currentlyGeneratedNumberOfRooms;
            }
            set
            {
                this.currentlyGeneratedNumberOfRooms = value;
            }
        }
    }
}
