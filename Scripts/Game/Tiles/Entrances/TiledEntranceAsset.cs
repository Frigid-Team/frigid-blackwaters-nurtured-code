using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TiledEntranceAsset", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "TiledEntranceAsset")]
    public class TiledEntranceAsset : FrigidScriptableObject
    {
        [SerializeField]
        private int minWidth;
        [SerializeField]
        private int maxWidth;
        [SerializeField]
        private List<PrefabEntry> defaultPrefabEntries;
        [SerializeField]
        private List<EntranceContext> entranceContexts;

        private Dictionary<(TiledAreaBlueprintGroup, TiledAreaBlueprintGroup), List<PrefabEntry>> contextMap;

        public int MinWidth
        {
            get
            {
                return this.minWidth;
            }
        }

        public int MaxWidth
        {
            get
            {
                return this.maxWidth;
            }
        }

        public bool TryGetEntrancePrefab(TileTerrain terrain, TiledAreaBlueprintGroup fromBlueprintGroup, TiledAreaBlueprintGroup toBlueprintGroup, out TiledEntrance entrancePrefab)
        {
            List<PrefabEntry> prefabEntries = this.defaultPrefabEntries;
            if (this.contextMap.ContainsKey((fromBlueprintGroup, toBlueprintGroup)))
            {
                prefabEntries = this.contextMap[(fromBlueprintGroup, toBlueprintGroup)];
            }

            int index = prefabEntries.FindIndex((PrefabEntry prefabEntry) => prefabEntry.Terrain == terrain);
            if (index != -1)
            {
                entrancePrefab = prefabEntries[index].EntrancePrefab;
                return true;
            }
            entrancePrefab = null;
            return false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Debug.Assert(this.minWidth > 0 && this.maxWidth > 0, "Widths of TiledEntranceAsset " + this.name + " are less or equal to 0.");
            Debug.Assert(this.maxWidth >= this.minWidth, "Max width is lower than min width of TiledEntranceAsset " + this.name + ".");

            this.contextMap = new Dictionary<(TiledAreaBlueprintGroup, TiledAreaBlueprintGroup), List<PrefabEntry>>();
            foreach (EntranceContext entranceContext in this.entranceContexts)
            {
                foreach (TiledAreaBlueprintGroup fromBlueprintGroup in entranceContext.FromBlueprintGroups)
                {
                    foreach (TiledAreaBlueprintGroup toBlueprintGroup in entranceContext.ToBlueprintGroups)
                    {
                        this.contextMap.TryAdd((fromBlueprintGroup, toBlueprintGroup), entranceContext.PrefabEntries);
                        if (!entranceContext.OtherDirectionIncluded)
                        {
                            continue;
                        }
                        this.contextMap.TryAdd((toBlueprintGroup, fromBlueprintGroup), entranceContext.PrefabEntries);
                    }
                }
            }
        }


        [Serializable]
        private struct EntranceContext
        {
            [SerializeField]
            private List<TiledAreaBlueprintGroup> fromBlueprintGroups;
            [SerializeField]
            private List<TiledAreaBlueprintGroup> toBlueprintGroups;
            [SerializeField]
            private bool otherDirectionIncluded;
            [SerializeField]
            private List<PrefabEntry> prefabEntries;

            public List<TiledAreaBlueprintGroup> FromBlueprintGroups
            {
                get
                {
                    return this.fromBlueprintGroups;
                }
            }

            public List<TiledAreaBlueprintGroup> ToBlueprintGroups
            {
                get
                {
                    return this.toBlueprintGroups;
                }
            }

            public bool OtherDirectionIncluded
            {
                get
                {
                    return this.otherDirectionIncluded;
                }
            }

            public List<PrefabEntry> PrefabEntries
            {
                get
                {
                    return this.prefabEntries;
                }
            }
        }

        [Serializable]
        private struct PrefabEntry
        {
            [SerializeField]
            private TileTerrain terrain;
            [SerializeField]
            private TiledEntrance entrancePrefab;

            public TileTerrain Terrain
            {
                get
                {
                    return this.terrain;
                }
            }

            public TiledEntrance EntrancePrefab
            {
                get
                {
                    return this.entrancePrefab;
                }
            }
        }
    }
}
