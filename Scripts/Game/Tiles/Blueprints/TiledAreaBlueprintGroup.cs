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

        public List<TiledAreaBlueprint> GetMatchingEntranceTerrainBlueprints(TileTerrain[] entranceTerrains)
        {
            List<TiledAreaBlueprint> matchingBlueprints = new List<TiledAreaBlueprint>();
            VisitMatchingEntranceTerrainCodes(
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

        protected override void Init()
        {
            base.Init();
            this.entranceTerrainMappedBlueprints = new Dictionary<int, List<TiledAreaBlueprint>>();

            foreach (TiledAreaBlueprint tiledAreaBlueprint in this.allBlueprints)
            {
                int code = GetEntranceTerrainCode(tiledAreaBlueprint.EntranceTerrains);
                if (!this.entranceTerrainMappedBlueprints.ContainsKey(code))
                {
                    this.entranceTerrainMappedBlueprints.Add(code, new List<TiledAreaBlueprint>());
                }
                this.entranceTerrainMappedBlueprints[code].Add(tiledAreaBlueprint);
            }
        }

        private void VisitMatchingEntranceTerrainCodes(int index, TileTerrain[] entranceTerrains, Action<int> onVisited)
        {
            if (index >= entranceTerrains.Length)
            {
                onVisited?.Invoke(GetEntranceTerrainCode(entranceTerrains));
                return;
            }

            if (entranceTerrains[index] == TileTerrain.None)
            {
                for (int i = (int)TileTerrain.None + 1; i < (int)TileTerrain.Count; i++)
                {
                    entranceTerrains[index] = (TileTerrain)i;
                    VisitMatchingEntranceTerrainCodes(index + 1, entranceTerrains, onVisited);
                    entranceTerrains[index] = TileTerrain.None;
                }
            }
            else
            {
                VisitMatchingEntranceTerrainCodes(index + 1, entranceTerrains, onVisited);
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
