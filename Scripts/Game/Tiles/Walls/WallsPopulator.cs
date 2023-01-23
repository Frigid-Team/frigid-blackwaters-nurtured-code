using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WallsPopulator : FrigidMonoBehaviour
    {
        public const int MAX_WALL_DEPTH = 4;

        [SerializeField]
        private WallTileAssetGroup wallTileAssetGroup;
        [SerializeField]
        private WallColliders wallCollidersPrefab;

        public void PopulateWalls(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            for (int i = 0; i <= MAX_WALL_DEPTH; i++)
            {
                PopulateWallsOnLayer(i, tiledAreaBlueprint, contentsTransform);
            }

            WallColliders spawnedWallColliders = FrigidInstancing.CreateInstance<WallColliders>(this.wallCollidersPrefab, contentsTransform.position, contentsTransform);
            spawnedWallColliders.PositionColliders(tiledAreaBlueprint.MainAreaDimensions);
        }

        private void SpawnEdgeWall(int layer, TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform, Vector2Int positionIndices, Vector2Int dimensions, float rotationDeg)
        {
            if (GetWallTilePrefab(layer, tiledAreaBlueprint, positionIndices, out WallTile wallTilePrefab))
            {
                WallTile spawnedWall = FrigidInstancing.CreateInstance<WallTile>(wallTilePrefab, TilePositioning.TileAbsolutePositionFromIndices(positionIndices, contentsTransform.position, dimensions), Quaternion.Euler(0, 0, rotationDeg), contentsTransform);
                spawnedWall.Populated(true);
            }
        }

        private void SpawnCornerWall(int layer, TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform, Vector2Int positionIndices, Vector2Int dimensions, float rotationDeg)
        {
            if (GetWallTilePrefab(layer, tiledAreaBlueprint, positionIndices, out WallTile wallTilePrefab))
            {
                WallTile spawnedWall = FrigidInstancing.CreateInstance<WallTile>(wallTilePrefab, TilePositioning.TileAbsolutePositionFromIndices(positionIndices, contentsTransform.position, dimensions), Quaternion.Euler(0, 0, rotationDeg), contentsTransform);
                spawnedWall.Populated(false);
            }
        }

        private bool GetWallTilePrefab(int layer, TiledAreaBlueprint tiledAreaBlueprint, Vector2Int positionIndices, out WallTile wallTilePrefab)
        {
            if (this.wallTileAssetGroup.TryGetAsset(tiledAreaBlueprint.WallTileID, out WallTileAsset wallTileAsset) && layer <= wallTileAsset.Depth)
            {
                if (layer > 0)
                {
                    wallTilePrefab = wallTileAsset.GetWallTilePrefab(layer);
                    return true;
                }
                else
                {
                    return wallTileAsset.TryGetBoundaryTilePrefab(tiledAreaBlueprint.GetTerrainTileIDAtTile(positionIndices), out wallTilePrefab);
                }
            }
            wallTilePrefab = null;
            return false;
        }

        private void PopulateWallsOnLayer(int layer, TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            Vector2Int distance = new Vector2Int(tiledAreaBlueprint.MainAreaDimensions.x + 2 * layer, tiledAreaBlueprint.MainAreaDimensions.y + 2 * layer);

            SpawnCornerWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(0, 0), distance, 0);
            SpawnCornerWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(distance.x - 1, 0), distance, 270);
            SpawnCornerWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(0, distance.y - 1), distance, 90);
            SpawnCornerWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(distance.x - 1, distance.y - 1), distance, 180);

            for (int x = 1; x < distance.x - 1; x++)
            {
                SpawnEdgeWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(x, 0), distance, 0);
                SpawnEdgeWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(x, distance.y - 1), distance, 180);
            }

            for (int y = 1; y < distance.y - 1; y++)
            {
                SpawnEdgeWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(distance.x - 1, y), distance, 270);
                SpawnEdgeWall(layer, tiledAreaBlueprint, contentsTransform, new Vector2Int(0, y), distance, 90);
            }
        }
    }
}
