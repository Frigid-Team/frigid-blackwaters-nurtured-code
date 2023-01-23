using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FrigidBlackwaters.Game
{
    public class TiledLevelPlanArea
    {
        private TiledArea tiledAreaPrefab;
        private TiledAreaBlueprintGroup blueprintGroup;
        private TileTerrain[] entranceTerrains;
        private List<Vector2Int> remainingWallEntryDirections;

        private int numberAreasFromStart;
        private float numberAreasFromStartPercent;
        private TiledAreaBlueprint chosenBlueprint;

        public TiledLevelPlanArea(TiledArea tiledAreaPrefab, TiledAreaBlueprintGroup blueprintGroup, TileTerrain[] entranceTerrains = null)
        {
            this.tiledAreaPrefab = tiledAreaPrefab;
            this.blueprintGroup = blueprintGroup;
            this.entranceTerrains = entranceTerrains != null ? entranceTerrains : (new TileTerrain[4] { TileTerrain.None, TileTerrain.None, TileTerrain.None, TileTerrain.None });
            this.remainingWallEntryDirections = TilePositioning.GetAllWallDirections();

            this.numberAreasFromStart = int.MaxValue;
            this.chosenBlueprint = null;
        }

        public TiledArea TiledAreaPrefab
        {
            get
            {
                return this.tiledAreaPrefab;
            }
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

        public List<Vector2Int> RemainingWallEntryDirections
        {
            get
            {
                return this.remainingWallEntryDirections;
            }
        }

        public List<Vector2Int> OccupiedWallEntryDirections
        {
            get
            {
                return TilePositioning.GetAllWallDirections().Except(this.remainingWallEntryDirections).ToList();
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

        public void AddWallEntry(Vector2Int entryDirection, TileTerrain entranceTerrain)
        {
            if (this.remainingWallEntryDirections.Contains(entryDirection))
            {
                this.remainingWallEntryDirections.Remove(entryDirection);
                SetWallEntry(entryDirection, entranceTerrain);
                return;
            }
            Debug.LogError("Adding an entrance that already exists: " + entryDirection);
        }

        public void SetWallEntry(Vector2Int entryDirection, TileTerrain entranceTerrain)
        {
            if (!this.remainingWallEntryDirections.Contains(entryDirection))
            {
                this.entranceTerrains[TilePositioning.WallArrayIndex(entryDirection)] = entranceTerrain;
                return;
            }
            Debug.LogError("Setting an entrance that doesn't exist: " + entryDirection);
        }
    }
}
