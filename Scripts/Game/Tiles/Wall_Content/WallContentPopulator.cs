using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class WallContentPopulator : FrigidMonoBehaviour
    {
        [SerializeField]
        private WallContentAssetGroup wallContentAssetGroup;

        public void PopulateWallContent(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
            {
                SpawnWallContent(
                    tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(x, 0)),
                    tiledAreaBlueprint.GetWallContentIDAtTile(Vector2Int.up, x),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAtTile(Vector2Int.up, x),
                    contentsTransform,
                    new Vector2Int(x, 0), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x, tiledAreaBlueprint.MainAreaDimensions.y + 2),
                    0
                    );

                SpawnWallContent(
                    tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(x, tiledAreaBlueprint.MainAreaDimensions.y - 1)),
                    tiledAreaBlueprint.GetWallContentIDAtTile(Vector2Int.down, x),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAtTile(Vector2Int.down, x),
                    contentsTransform,
                    new Vector2Int(x, tiledAreaBlueprint.MainAreaDimensions.y + 1), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x, tiledAreaBlueprint.MainAreaDimensions.y + 2),
                    180
                    );
            }

            for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
            {
                SpawnWallContent(
                    tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(0, y)),
                    tiledAreaBlueprint.GetWallContentIDAtTile(Vector2Int.left, y),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAtTile(Vector2Int.left, y),
                    contentsTransform,
                    new Vector2Int(0, y), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 2, tiledAreaBlueprint.MainAreaDimensions.y),
                    90
                    );

                SpawnWallContent(
                    tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x - 1, y)),
                    tiledAreaBlueprint.GetWallContentIDAtTile(Vector2Int.right, y),
                    tiledAreaBlueprint.GetWallContentOrientationDirectionAtTile(Vector2Int.right, y),
                    contentsTransform,
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 1, y), 
                    new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 2, tiledAreaBlueprint.MainAreaDimensions.y),
                    270
                    );
            }
        }

        private void SpawnWallContent(
            TileTerrain terrain,
            string id,
            Vector2 orientationDirection,
            Transform contentsTransform,
            Vector2Int positionIndices,
            Vector2Int adjustedAreaDimensions,
            float rotationDeg
            )
        {
            if (this.wallContentAssetGroup.TryGetWallContentAsset(terrain, id, out WallContentAsset wallContentAsset))
            {
                WallContent chosenWallContentPrefab = wallContentAsset.WallContentPrefabs[Random.Range(0, wallContentAsset.WallContentPrefabs.Count)];
                Vector2 spawnPosition = TilePositioning.RectAbsolutePositionFromIndices(positionIndices, contentsTransform.position, adjustedAreaDimensions, new Vector2Int(chosenWallContentPrefab.Width, 1));
                Vector2 pivotPosition = TilePositioning.TileAbsolutePositionFromIndices(positionIndices, contentsTransform.position, adjustedAreaDimensions);
                float rotationRad = rotationDeg * Mathf.Deg2Rad;
                spawnPosition.RotateAround(pivotPosition, rotationRad);
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
}
