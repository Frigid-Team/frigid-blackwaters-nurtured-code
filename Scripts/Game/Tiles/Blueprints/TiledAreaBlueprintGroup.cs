using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledAreaBlueprintGroup", menuName = FrigidPaths.CreateAssetMenu.GAME + FrigidPaths.CreateAssetMenu.TILES + "TiledAreaBlueprintGroup")]
    public class TiledAreaBlueprintGroup : FrigidScriptableObject
    {
        [SerializeField]
        private List<TiledAreaBlueprint> allBlueprints;

        private Dictionary<int, List<TiledAreaBlueprint>> entranceTerrainMappedBlueprints;

        public int NumberAvailableEntranceTerrains
        {
            get
            {
                return this.entranceTerrainMappedBlueprints.Count;
            }
        }

        public bool Includes(TiledAreaBlueprint blueprint)
        {
            return this.allBlueprints.Contains(blueprint);
        }

        public List<TiledAreaBlueprint> GetMatchingEntranceTerrainBlueprints(TileTerrain[] entranceTerrains)
        {
            List<TiledAreaBlueprint> matchingBlueprints = new List<TiledAreaBlueprint>();
            this.VisitMatchingEntranceTerrainCodes(
                0,
                entranceTerrains, 
                (int code) => 
                {
                    if (this.entranceTerrainMappedBlueprints.ContainsKey(code))
                    {
                        matchingBlueprints.AddRange(this.entranceTerrainMappedBlueprints[code]);
                    }
                }
                );
            return matchingBlueprints;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.entranceTerrainMappedBlueprints = new Dictionary<int, List<TiledAreaBlueprint>>();

            foreach (TiledAreaBlueprint blueprint in this.allBlueprints)
            {
                TileTerrain[] entranceTerrains = new TileTerrain[4];
                foreach (Vector2Int wallIndexDirection in WallTiling.GetAllWallIndexDirections())
                {
                    int wallIndex = WallTiling.WallIndexFromWallIndexDirection(wallIndexDirection);
                    entranceTerrains[wallIndex] = TileTerrain.None;
                    if (!blueprint.TryGetWallEntranceAssetAndIndexAndWidth(wallIndexDirection, out _, out int tileIndex, out _)) 
                    {
                        continue;
                    }
                    entranceTerrains[wallIndex] = blueprint.GetTerrainTileAssetAt(WallTiling.WallIndexDirectionAndTileIndexToInnerTileIndexPosition(wallIndexDirection, tileIndex, blueprint.MainAreaDimensions)).Terrain;
                }

                int code = this.GetEntranceTerrainCode(entranceTerrains);
                if (!this.entranceTerrainMappedBlueprints.ContainsKey(code))
                {
                    this.entranceTerrainMappedBlueprints.Add(code, new List<TiledAreaBlueprint>());
                }
                this.entranceTerrainMappedBlueprints[code].Add(blueprint);
            }
        }

        private void VisitMatchingEntranceTerrainCodes(int index, TileTerrain[] entranceTerrains, Action<int> onVisited)
        {
            if (index >= entranceTerrains.Length)
            {
                onVisited?.Invoke(this.GetEntranceTerrainCode(entranceTerrains));
                return;
            }

            if (entranceTerrains[index] == TileTerrain.None)
            {
                for (int i = (int)TileTerrain.None; i < (int)TileTerrain.Count; i++)
                {
                    entranceTerrains[index] = (TileTerrain)i;
                    this.VisitMatchingEntranceTerrainCodes(index + 1, entranceTerrains, onVisited);
                    entranceTerrains[index] = TileTerrain.None;
                }
            }
            else
            {
                this.VisitMatchingEntranceTerrainCodes(index + 1, entranceTerrains, onVisited);
            }
        }

        private int GetEntranceTerrainCode(TileTerrain[] entranceTerrains)
        {
            int code = 0;
            for (int i = 0; i < entranceTerrains.Length; i++)
            {
                code += Mathf.RoundToInt(Mathf.Pow((int)TileTerrain.Count, i)) * (int)entranceTerrains[i];
            }
            return code;
        }
    }
}
