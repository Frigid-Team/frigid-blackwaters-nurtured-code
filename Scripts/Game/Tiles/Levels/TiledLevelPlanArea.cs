using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanArea
    {
        private TiledAreaBlueprintGroup blueprintGroup;
        private TileTerrain[] entranceTerrains;
        private List<Vector2Int> remainingWallEntryIndexDirections;

        private int numberAreasFromStart;
        private float numberAreasFromStartPercent;
        private TiledAreaBlueprint chosenBlueprint;

        public TiledLevelPlanArea(TiledAreaBlueprintGroup blueprintGroup, TileTerrain[] entranceTerrains = null)
        {
            this.blueprintGroup = blueprintGroup;
            this.entranceTerrains = entranceTerrains != null ? entranceTerrains : (new TileTerrain[4] { TileTerrain.None, TileTerrain.None, TileTerrain.None, TileTerrain.None });
            this.remainingWallEntryIndexDirections = new List<Vector2Int>(WallTiling.GetAllWallIndexDirections());

            this.numberAreasFromStart = int.MaxValue;
            this.chosenBlueprint = null;
        }

        public TiledAreaBlueprintGroup BlueprintGroup
        {
            get
            {
                return this.blueprintGroup;
            }
        }

        public TileTerrain[] EntranceTerrains
        {
            get
            {
                return this.entranceTerrains;
            }
        }

        public List<Vector2Int> RemainingWallEntryIndexDirections
        {
            get
            {
                return this.remainingWallEntryIndexDirections;
            }
        }

        public List<Vector2Int> OccupiedWallEntryIndexDirections
        {
            get
            {
                return WallTiling.GetAllWallIndexDirections().Except(this.remainingWallEntryIndexDirections).ToList();
            }
        }

        public int NumberAreasFromStart
        {
            get
            {
                return this.numberAreasFromStart;
            }
            set
            {
                this.numberAreasFromStart = value;
            }
        }

        public float NumberAreasFromStartPercent
        {
            get
            {
                return this.numberAreasFromStartPercent;
            }
            set
            {
                this.numberAreasFromStartPercent = value;
            }
        }

        public TiledAreaBlueprint ChosenBlueprint
        {
            get
            {
                return this.chosenBlueprint;
            }
            set
            {
                this.chosenBlueprint = value;
            }
        }

        public void AddWallEntry(Vector2Int entryIndexDirection, TileTerrain entranceTerrain)
        {
            if (this.remainingWallEntryIndexDirections.Contains(entryIndexDirection))
            {
                this.remainingWallEntryIndexDirections.Remove(entryIndexDirection);
                this.SetWallEntry(entryIndexDirection, entranceTerrain);
                return;
            }
            Debug.LogError("Adding an entry that already exists: " + entryIndexDirection);
        }

        public void SetWallEntry(Vector2Int entryIndexDirection, TileTerrain entranceTerrain)
        {
            if (!this.remainingWallEntryIndexDirections.Contains(entryIndexDirection))
            {
                this.entranceTerrains[WallTiling.WallIndexFromWallIndexDirection(entryIndexDirection)] = entranceTerrain;
                return;
            }
            Debug.LogError("Setting an entry that doesn't exist: " + entryIndexDirection);
        }
    }
}
