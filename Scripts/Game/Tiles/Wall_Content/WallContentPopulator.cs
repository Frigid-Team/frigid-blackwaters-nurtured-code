using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class WallContentPopulator : FrigidMonoBehaviour
    {
        public void PopulateWallContent(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
            {
                SpawnWallContent(
                    tiledAreaBlueprint.GetWallContentAssetAt(Vector2Int.up, x),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAt(Vector2Int.up, x),
                    contentsTransform,
                    new Vector2Int(x, 0), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x, tiledAreaBlueprint.MainAreaDimensions.y + 2),
                    0
                    );

                SpawnWallContent(
                    tiledAreaBlueprint.GetWallContentAssetAt(Vector2Int.down, x),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAt(Vector2Int.down, x),
                    contentsTransform,
                    new Vector2Int(x, tiledAreaBlueprint.MainAreaDimensions.y + 1), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x, tiledAreaBlueprint.MainAreaDimensions.y + 2),
                    180
                    );
            }

            for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
            {
                SpawnWallContent(
                    tiledAreaBlueprint.GetWallContentAssetAt(Vector2Int.left, y),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAt(Vector2Int.left, y),
                    contentsTransform,
                    new Vector2Int(0, y), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 2, tiledAreaBlueprint.MainAreaDimensions.y),
                    90
                    );

                SpawnWallContent(
                    tiledAreaBlueprint.GetWallContentAssetAt(Vector2Int.right, y),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAt(Vector2Int.right, y),
                    contentsTransform,
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 1, y), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 2, tiledAreaBlueprint.MainAreaDimensions.y),
                    270
                    );
            }
        }

        private void SpawnWallContent(
            WallContentAsset wallContentAsset,
            Vector2 orientationDirection,
            Transform contentsTransform,
            Vector2Int tileIndices,
            Vector2Int adjustedAreaDimensions,
            float rotationDeg
            )
        {
            if (wallContentAsset.IsNone) return;

            WallContent chosenWallContentPrefab = wallContentAsset.WallContentPrefabs[Random.Range(0, wallContentAsset.WallContentPrefabs.Count)];
            Vector2 spawnPosition = TilePositioning.RectPositionFromIndices(tileIndices, contentsTransform.position, adjustedAreaDimensions, new Vector2Int(chosenWallContentPrefab.Width, 1));
            Vector2 pivotPosition = TilePositioning.TilePositionFromIndices(tileIndices, contentsTransform.position, adjustedAreaDimensions);
            float rotationRad = rotationDeg * Mathf.Deg2Rad;
            spawnPosition = spawnPosition.RotateAround(pivotPosition, rotationRad);
            WallContent spawnedWallContent = FrigidInstancing.CreateInstance<WallContent>(
                chosenWallContentPrefab,
                spawnPosition,
                Quaternion.Euler(0, 0, rotationDeg),
                contentsTransform
                );
            spawnedWallContent.Populated(orientationDirection);
        }
    }
}
