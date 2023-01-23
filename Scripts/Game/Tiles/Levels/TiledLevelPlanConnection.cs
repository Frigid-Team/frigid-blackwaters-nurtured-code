using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanConnection
    {
        private int numberEntrances;
        private TiledLevelPlanEntrance[] planEntrances;
        private Vector2Int[] entryDirections;
        private TileTerrain connectionTerrain;
        private bool isSubLevelConnection;

        public TiledLevelPlanConnection(
            TiledLevelPlanEntrance firstEntrance,
            TiledLevelPlanEntrance secondEntrance,
            Vector2Int direction,
            TileTerrain connectionTerrain = TileTerrain.None
            )
        {
            if (!TilePositioning.IsValidWallDirection(direction))
            {
                Debug.LogError("Connection has invalid direction: " + direction);
            }

            this.numberEntrances = 2;
            this.planEntrances = new TiledLevelPlanEntrance[2] { firstEntrance, secondEntrance };
            this.entryDirections = new Vector2Int[2] { direction, -direction };
            this.connectionTerrain = connectionTerrain;

            this.isSubLevelConnection = false;
            for (int i = 0; i < this.numberEntrances; i++)
            {
                if (!this.planEntrances[i].IsSubLevelEntrance) this.planEntrances[i].Area.AddWallEntry(this.entryDirections[i], this.connectionTerrain);
                else this.isSubLevelConnection = true;
            }
        }

        public int NumberEntrances
        {
            get
            {
                return this.numberEntrances;
            }
        }

        public TiledLevelPlanEntrance[] PlanEntrances
        {
            get
            {
                return this.planEntrances;
            }
        }

        public Vector2Int[] EntryDirections
        {
            get
            {
                return this.entryDirections;
            }
        }

        public TileTerrain ConnectionTerrain
        {
            get
            {
                return this.connectionTerrain;
            }
            set
            {
                this.connectionTerrain = value;
                for (int i = 0; i < this.numberEntrances; i++)
                {
                    if (this.planEntrances[i].IsSubLevelEntrance) continue;
                    this.planEntrances[i].Area.SetWallEntry(this.entryDirections[i], this.connectionTerrain);
                }
            }
        }

        public bool IsSubLevelConnection
        {
            get
            {
                return this.isSubLevelConnection;
            }
        }
    }
}
