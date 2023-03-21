using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainPopulator : FrigidMonoBehaviour
    {
        public void PopulateTerrain(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
            {
                for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
                {
                    Vector2Int terrainTileIndices = new Vector2Int(x, y);
                    TerrainTileAsset terrainTileAssetAtCurrentTile = tiledAreaBlueprint.GetTerrainTileAssetAt(terrainTileIndices);
                    List<Vector2Int> terrainTileCorners = new List<Vector2Int>() { new Vector2Int(2, 2), new Vector2Int(2, -2), new Vector2Int(-2, -2), new Vector2Int(-2, 2) };

                    Vector2Int currentPosition = Vector2Int.zero;
                    Vector2Int previousPosition;
                    TerrainTileAsset[] previousTerrainTileAssets = new TerrainTileAsset[3];
                    bool pushEnabled = false;

                    for (int i = 0; i < 10; i++)
                    {
                        previousPosition = currentPosition;
                        previousTerrainTileAssets[0] = previousTerrainTileAssets[1];
                        previousTerrainTileAssets[1] = previousTerrainTileAssets[2];

                        float currentDirection = i * Mathf.PI / 4;
                        currentPosition = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(currentDirection)), Mathf.RoundToInt(Mathf.Sin(currentDirection)));

                        if (TilePositioning.TileIndicesWithinBounds(new Vector2Int(x, y) + currentPosition, tiledAreaBlueprint.MainAreaDimensions))
                        {
                            previousTerrainTileAssets[2] = tiledAreaBlueprint.GetTerrainTileAssetAt(new Vector2Int(x + currentPosition.x, y + currentPosition.y));
                        }
                        else
                        {
                            previousTerrainTileAssets[2] = null;
                        }

                        if (i >= 2)
                        {
                            Vector2Int crossoverTileIndices = new Vector2Int(x + previousPosition.x, y + previousPosition.y);
                            Vector2 crossoverDirection = ((crossoverTileIndices - terrainTileIndices) * new Vector2(1, -1)).normalized;
                            if (TilePositioning.TileIndicesWithinBounds(crossoverTileIndices, tiledAreaBlueprint.MainAreaDimensions) && previousTerrainTileAssets[1] != terrainTileAssetAtCurrentTile)
                            {
                                pushEnabled = true;
                                if (i % 2 == 0)
                                {
                                    if (previousTerrainTileAssets[0] != terrainTileAssetAtCurrentTile && previousTerrainTileAssets[2] != terrainTileAssetAtCurrentTile)
                                    {
                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            false,
                                            crossoverDirection
                                            );

                                        int cornerIndex = 4 - (i % 8 / 2);

                                        float rightAngleCornerAngle = ((cornerIndex + 1) * 90 - 45) * Mathf.Deg2Rad;
                                        Vector2Int rightAngleCornerPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(rightAngleCornerAngle)) * 2, Mathf.RoundToInt(Mathf.Sin(rightAngleCornerAngle)) * 2);
                                        int startingIndex = terrainTileCorners.IndexOf(rightAngleCornerPoint);
                                        terrainTileCorners.RemoveAt(startingIndex);

                                        float firstSlantAngle = rightAngleCornerAngle + 45 * Mathf.Deg2Rad;
                                        float secondSlantAngle = rightAngleCornerAngle - 45 * Mathf.Deg2Rad;
                                        Vector2Int firstSlantPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(firstSlantAngle)) * 2 + Mathf.RoundToInt(Mathf.Sin(firstSlantAngle)), Mathf.RoundToInt(Mathf.Sin(firstSlantAngle)) * 2 - Mathf.RoundToInt(Mathf.Cos(firstSlantAngle)));
                                        Vector2Int secondSlantPoint = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(secondSlantAngle)) * 2 - Mathf.RoundToInt(Mathf.Sin(secondSlantAngle)), Mathf.RoundToInt(Mathf.Sin(secondSlantAngle)) * 2 + Mathf.RoundToInt(Mathf.Cos(secondSlantAngle)));

                                        if (!terrainTileCorners.Contains(firstSlantPoint))
                                        {
                                            terrainTileCorners.Insert(startingIndex, firstSlantPoint);
                                            startingIndex++;
                                        }

                                        if (!terrainTileCorners.Contains(secondSlantPoint))
                                        {
                                            terrainTileCorners.Insert(startingIndex, secondSlantPoint);
                                        }
                                    }
                                    else if (previousTerrainTileAssets[0] == terrainTileAssetAtCurrentTile && previousTerrainTileAssets[2] == terrainTileAssetAtCurrentTile)
                                    {
                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            true,
                                            crossoverDirection
                                            );
                                    }
                                }
                                else
                                {
                                    if ((previousTerrainTileAssets[0] != terrainTileAssetAtCurrentTile && previousTerrainTileAssets[2] != terrainTileAssetAtCurrentTile) ||
                                        (previousTerrainTileAssets[0] == null && previousTerrainTileAssets[2] != terrainTileAssetAtCurrentTile) ||
                                        (previousTerrainTileAssets[0] != terrainTileAssetAtCurrentTile && previousTerrainTileAssets[2] == null))
                                    {

                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            false,
                                            crossoverDirection
                                            );
                                    }
                                }
                            }
                        }
                    }

                    SpawnTerrainTile(
                        terrainTileIndices, 
                        tiledAreaBlueprint, 
                        contentsTransform,
                        pushEnabled,
                        terrainTileCorners
                        );
                }
            }
        }

        private void SpawnTerrainTile(
            Vector2Int terrainTileIndices,
            TiledAreaBlueprint tiledAreaBlueprint,
            Transform contentsTransform,
            bool pushEnabled,
            List<Vector2Int> cornerPoints
            )
        {
            TerrainTileAsset terrainTileAsset = tiledAreaBlueprint.GetTerrainTileAssetAt(terrainTileIndices);
            TerrainTile spawnedTerrainTile = FrigidInstancing.CreateInstance<TerrainTile>(
                terrainTileAsset.TerrainTilePrefab,
                TilePositioning.TilePositionFromIndices(terrainTileIndices, contentsTransform.position, tiledAreaBlueprint.MainAreaDimensions),
                contentsTransform
                );
            spawnedTerrainTile.Populated(pushEnabled, cornerPoints);
        }

        private void SpawnCrossoverTile(
            Vector2Int crossoverTileIndices,
            Vector2Int terrainTileIndices,
            TiledAreaBlueprint tiledAreaBlueprint,
            Transform contentsTransform,
            bool isOuter,
            Vector2 direction
            )
        {
            TerrainTileAsset terrainTileAsset = tiledAreaBlueprint.GetTerrainTileAssetAt(terrainTileIndices);
            if (terrainTileAsset.TryGetTerrainCrossoverTilePrefab(tiledAreaBlueprint.GetTerrainTileAssetAt(crossoverTileIndices), out TerrainCrossoverTile terrainCrossoverTilePrefab))
            {
                TerrainCrossoverTile spawnedCrossoverTile = FrigidInstancing.CreateInstance<TerrainCrossoverTile>(
                    terrainCrossoverTilePrefab,
                    TilePositioning.TilePositionFromIndices(crossoverTileIndices, contentsTransform.position, tiledAreaBlueprint.MainAreaDimensions),
                    contentsTransform
                    );
                spawnedCrossoverTile.Populated(direction, isOuter);
            }
        }
    }
}
