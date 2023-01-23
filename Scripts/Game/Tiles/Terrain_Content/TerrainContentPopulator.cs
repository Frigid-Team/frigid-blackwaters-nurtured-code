using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainContentPopulator : FrigidMonoBehaviour
    {
        [SerializeField]
        private TerrainContentAssetGroup terrainContentAssetGroup;

        public void PopulateTerrainContent(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform, NavigationGrid navigationGrid)
        {
            for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
            {
                int[] numSpawnedOnRowAtHeight = new int[(int)TerrainContentHeight.Count];
                for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
                {
                    for (int i = 0; i < (int)TerrainContentHeight.Count; i++)
                    {
                        TerrainContentHeight height = (TerrainContentHeight)i;
                        Vector2Int positionIndices = new Vector2Int(x, y);
                        string id = tiledAreaBlueprint.GetTerrainContentIDAtTile(positionIndices, height);
                        Vector2 orientationDirection = tiledAreaBlueprint.GetTerrainContentOrientationDirectionAtTile(positionIndices, height);
                        if (this.terrainContentAssetGroup.TryGetTerrainContentAsset(height, tiledAreaBlueprint.GetTerrainAtTile(new Vector2Int(x, y)), id, out TerrainContentAsset terrainContentAsset))
                        {
                            TerrainContent chosenTerrainContentPrefab = terrainContentAsset.TerrainContentPrefabs[Random.Range(0, terrainContentAsset.TerrainContentPrefabs.Count)];
                            Vector2 spawnTileAbsolutePosition = TilePositioning.RectAbsolutePositionFromIndices(new Vector2Int(x, y), contentsTransform.position, tiledAreaBlueprint.MainAreaDimensions, chosenTerrainContentPrefab.Dimensions);
                            TerrainContent spawnedTerrainContent = FrigidInstancing.CreateInstance<TerrainContent>(
                                chosenTerrainContentPrefab,
                                spawnTileAbsolutePosition + Vector2.up * (numSpawnedOnRowAtHeight[i] % 2 * GameConstants.SMALLEST_WORLD_SIZE),
                                contentsTransform
                                );
                            List<Vector2Int> populatedTileIndices = new List<Vector2Int>();
                            TilePositioning.VisitTileIndicesInRect(
                                new Vector2Int(x, y),
                                chosenTerrainContentPrefab.Dimensions,
                                tiledAreaBlueprint.MainAreaDimensions,
                                (Vector2Int tileIndices) => populatedTileIndices.Add(tileIndices)
                                );
                            spawnedTerrainContent.Populated(orientationDirection, navigationGrid, populatedTileIndices);
                            numSpawnedOnRowAtHeight[i]++;
                        }
                    }
                }
            }
        }
    }
}
