using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanConnection
    {
        private TiledLevelPlanEntrance firstEntrance;
        private TiledLevelPlanEntrance secondEntrance;
        private Vector2Int indexDirection;
        private TileTerrain connectionTerrain;
        private bool isSubLevelConnection;

        public TiledLevelPlanConnection(
            TiledLevelPlanEntrance firstEntrance,
            TiledLevelPlanEntrance secondEntrance,
            Vector2Int indexDirection,
            TileTerrain connectionTerrain = TileTerrain.None
            )
        {
            Debug.Assert(WallTiling.IsValidWallIndexDirection(indexDirection), "TiledLevelPlanConnection has invalid wall direction.");
            Debug.Assert(firstEntrance.Area != secondEntrance.Area, "TiledLevelPlanConnection connects to the same area.");

            this.firstEntrance = firstEntrance;
            this.secondEntrance = secondEntrance;
            this.indexDirection = indexDirection;
            this.connectionTerrain = connectionTerrain;

            this.isSubLevelConnection = this.firstEntrance.IsSubLevelEntrance || this.secondEntrance.IsSubLevelEntrance;
            if (!this.firstEntrance.IsSubLevelEntrance)
            {
                this.firstEntrance.Area.AddWallEntry(this.indexDirection, this.connectionTerrain);
            }
            if (!this.secondEntrance.IsSubLevelEntrance)
            {
                this.secondEntrance.Area.AddWallEntry(-this.indexDirection, this.connectionTerrain);
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

        public Vector2Int IndexDirection
        {
            get
            {
                return this.indexDirection;
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
                    this.firstEntrance.Area.SetWallEntry(this.indexDirection, this.connectionTerrain);
                }
                if (!this.secondEntrance.IsSubLevelEntrance)
                {
                    this.secondEntrance.Area.SetWallEntry(-this.indexDirection, this.connectionTerrain);
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
