using UnityEngine;
using System;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    [CreateAssetMenu(fileName = "WallContentAsset", menuName = FrigidPaths.CreateAssetMenu.Game + FrigidPaths.CreateAssetMenu.Tiles + "WallContentAsset")]
    public class WallContentAsset : FrigidScriptableObject
    {
        [SerializeField]
        private int baseWidth;
        [SerializeField]
        private List<Vector2> dimensionDirections;
        [SerializeField]
        private List<PrefabEntry> prefabEntries;

        public bool TryGetWallContentPrefab(TileTerrain terrain, out WallContent wallContentPrefab)
        {
            int index = this.prefabEntries.FindIndex((PrefabEntry prefabEntry) => prefabEntry.Terrain == terrain);
            if (index != -1)
            {
                wallContentPrefab = this.prefabEntries[index].WallContentPrefab;
                return true;
            }
            wallContentPrefab = null;
            return false;
        }

        public int GetWidth(Vector2 orientationDirection)
        {
            Vector2 chosenAxisDirection = Vector2.zero;
            foreach (Vector2 axisDirection in this.dimensionDirections)
            {
                Vector2 axisDirectionNorm = axisDirection.normalized;
                if (Mathf.Abs(Vector2.Dot(orientationDirection, axisDirectionNorm)) > Mathf.Abs(Vector2.Dot(orientationDirection, chosenAxisDirection)))
                {
                    chosenAxisDirection = axisDirectionNorm;
                }
            }

            int width = this.baseWidth;
            if (chosenAxisDirection != Vector2.zero)
            {
                Vector2 leftExtent = new Vector2(-this.baseWidth / 2f, 0);
                Vector2 rightExtent = -leftExtent;
                float angle = Vector2.Angle(Vector2.down, chosenAxisDirection);
                leftExtent = leftExtent.RotateAround(angle);
                rightExtent = rightExtent.RotateAround(angle);
                width = Mathf.CeilToInt(Mathf.Abs(rightExtent.x - leftExtent.x));
            }

            return width;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Debug.Assert(this.baseWidth > 0, "Width of WallContentAsset " + this.name + " is less or equal to 0.");
        }

        [Serializable]
        private struct PrefabEntry
        {
            [SerializeField]
            private TileTerrain terrain;
            [SerializeField]
            private WallContent wallContentPrefab;

            public TileTerrain Terrain
            {
                get
                {
                    return this.terrain;
                }
            }

            public WallContent WallContentPrefab
            {
                get
                {
                    return this.wallContentPrefab;
                }
            }
        }
    }
}
