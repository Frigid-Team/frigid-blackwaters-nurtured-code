using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "TerrainContentAsset", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "TerrainContentAsset")]
    public class TerrainContentAsset : FrigidScriptableObject
    {
        [SerializeField]
        private TerrainContentHeight height;
        [SerializeField]
        private Vector2Int baseDimensions;
        [SerializeField]
        private List<Vector2> dimensionDirections;
        [SerializeField]
        private List<PrefabEntry> prefabEntries;

        public TerrainContentHeight Height
        {
            get
            {
                return this.height;
            }
        }

        public bool TryGetTerrainContentPrefab(TileTerrain terrain, out TerrainContent terrainContentPrefab)
        {
            int index = this.prefabEntries.FindIndex((PrefabEntry prefabEntry) => prefabEntry.Terrain == terrain);
            if (index != -1)
            {
                terrainContentPrefab = this.prefabEntries[index].TerrainContentPrefab;
                return true;
            }
            terrainContentPrefab = null;
            return false;
        }

        public Vector2Int GetDimensions(Vector2 orientationDirection)
        {
            Vector2 chosenDimensionDirection = Vector2.zero;
            foreach (Vector2 dimensionDirection in this.dimensionDirections)
            {
                Vector2 dimensionDirectionNorm = dimensionDirection.normalized;
                if (Mathf.Abs(Vector2.Dot(orientationDirection, dimensionDirectionNorm)) > Mathf.Abs(Vector2.Dot(orientationDirection, chosenDimensionDirection)))
                {
                    chosenDimensionDirection = dimensionDirectionNorm;
                }
            }

            Vector2Int dimensions = this.baseDimensions;
            if (chosenDimensionDirection != Vector2.zero)
            {
                Vector2 bottomLeft = -this.baseDimensions;
                bottomLeft /= 2f;
                Vector2 topRight = this.baseDimensions;
                topRight /= 2f;
                float angle = Vector2.Angle(Vector2.down, chosenDimensionDirection);
                bottomLeft = bottomLeft.RotateAround(angle);
                topRight = topRight.RotateAround(angle);
                dimensions = new Vector2Int(Mathf.CeilToInt(Mathf.Abs(topRight.x - bottomLeft.x)), Mathf.CeilToInt(Mathf.Abs(topRight.y - bottomLeft.y)));
            }

            return dimensions;
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (this.baseDimensions.x <= 0 || this.baseDimensions.y <= 0)
            {
                Debug.LogError("Dimensions of TerrainContentAsset " + this.name + " are less or equal to 0.");
            }
        }

        [Serializable]
        private struct PrefabEntry
        {
            [SerializeField]
            private TileTerrain terrain;
            [SerializeField]
            private TerrainContent terrainContentPrefab;

            public TileTerrain Terrain
            {
                get
                {
                    return this.terrain;
                }
            }

            public TerrainContent TerrainContentPrefab
            {
                get
                {
                    return this.terrainContentPrefab;
                }
            }
        }
    }
}
