using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class GeneratedDungeonRoom
    {
        private Vector2Int indexPosition;
        private Dictionary<Vector2Int, GeneratedDungeonRoom> adjacentRooms;
        private RoomLayoutType roomLayoutType;
        private RoomContentType roomContentType;

        public GeneratedDungeonRoom(Vector2Int indexPosition)
        {
            this.indexPosition = indexPosition;
            this.adjacentRooms = new Dictionary<Vector2Int, GeneratedDungeonRoom>();
            this.roomLayoutType = RoomLayoutType.None;
            this.roomContentType = RoomContentType.None;
        }

        public Vector2Int IndexPosition
        {
            get
            {
                return this.indexPosition;
            }
        }

        public RoomLayoutType RoomLayoutType
        {
            get
            {
                return this.roomLayoutType;
            }
            set
            {
                this.roomLayoutType = value;
            }
        }

        public RoomContentType RoomContentType
        {
            get
            {
                return this.roomContentType;
            }
            set
            {
                this.roomContentType = value;
            }
        }

        public Dictionary<Vector2Int, GeneratedDungeonRoom> AdjacentRooms
        {
            get
            {
                return this.adjacentRooms;
            }
        }

        public void AddAdjacentRoom(GeneratedDungeonRoom adjacentRoom)
        {
            if (this.adjacentRooms.ContainsKey(adjacentRoom.IndexPosition))
            {
                Debug.LogWarning("Room already in position " + adjacentRoom.IndexPosition);
            }
            else
            {
                this.adjacentRooms.Add(adjacentRoom.IndexPosition, adjacentRoom);
            }
        }
    }
}
