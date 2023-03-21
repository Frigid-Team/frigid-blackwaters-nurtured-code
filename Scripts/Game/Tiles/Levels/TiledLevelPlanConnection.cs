using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanConnection
    {
        private TiledLevelPlanEntrance firstEntrance;
        private TiledLevelPlanEntrance secondEntrance;
        private Vector2Int direction;
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
                return;
            }

            if (firstEntrance.Area == secondEntrance.Area)
            {
                Debug.LogError("Connection connects to the same area!");
                return;
            }

            this.firstEntrance = firstEntrance;
            this.secondEntrance = secondEntrance;
            this.direction = direction;
            this.connectionTerrain = connectionTerrain;

            this.isSubLevelConnection = this.firstEntrance.IsSubLevelEntrance || this.secondEntrance.IsSubLevelEntrance;
            if (!this.firstEntrance.IsSubLevelEntrance)
            {
                this.firstEntrance.Area.AddWallEntry(this.direction, this.connectionTerrain);
            }
            if (!this.secondEntrance.IsSubLevelEntrance)
            {
                this.secondEntrance.Area.AddWallEntry(-this.direction, this.connectionTerrain);
            }
        }

        public TiledLevelPlanEntrance FirstEntrance
        {
            get
            {
                return this.firstEntrance;
            }
        }

        public TiledLevelPlanEntrance SecondEntrance
        {
            get
            {
                return this.secondEntrance;
            }
        }

        public Vector2Int Direction
        {
            get
            {
                return this.direction;
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
                if (!this.firstEntrance.IsSubLevelEntrance)
                {
                    this.firstEntrance.Area.SetWallEntry(this.direction, this.connectionTerrain);
                }
                if (!this.secondEntrance.IsSubLevelEntrance)
                {
                    this.secondEntrance.Area.SetWallEntry(-this.direction, this.connectionTerrain);
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
