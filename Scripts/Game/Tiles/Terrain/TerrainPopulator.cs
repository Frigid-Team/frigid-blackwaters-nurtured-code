using System.Collections.Generic;
using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class TerrainPopulator : FrigidMonoBehaviour
    {
        [SerializeField]
        private TerrainTileAssetGroup terrainTileAssetGroup;

        public void PopulateTerrain(TiledAreaBlueprint tiledAreaBlueprint, Transform contentsTransform)
        {
            for (int x = 0; x < tiledAreaBlueprint.MainAreaDimensions.x; x++)
            {
                for (int y = 0; y < tiledAreaBlueprint.MainAreaDimensions.y; y++)
                {
                    Vector2Int terrainTileIndices = new Vector2Int(x, y);
                    TileTerrain terrainAtCurrentTile = tiledAreaBlueprint.GetTerrainAtTile(terrainTileIndices);
                    string terrainTileIdAtCurrentTile = tiledAreaBlueprint.GetTerrainTileIDAtTile(terrainTileIndices);
                    List<Vector2Int> terrainTileCorners = new List<Vector2Int>() { new Vector2Int(2, 2), new Vector2Int(2, -2), new Vector2Int(-2, -2), new Vector2Int(-2, 2) };

                    Vector2Int currentPosition = Vector2Int.zero;
                    Vector2Int previousPosition;
                    string[] previousTerrainTileIds = new string[3];
                    bool pushEnabled = false;

                    for (int i = 0; i < 10; i++)
                    {
                        previousPosition = currentPosition;
                        previousTerrainTileIds[0] = previousTerrainTileIds[1];
                        previousTerrainTileIds[1] = previousTerrainTileIds[2];

                        float currentDirection = i * Mathf.PI / 4;
                        currentPosition = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(currentDirection)), Mathf.RoundToInt(Mathf.Sin(currentDirection)));

                        if (TilePositioning.TileIndicesWithinBounds(new Vector2Int(x, y) + currentPosition, tiledAreaBlueprint.MainAreaDimensions))
                        {
                            previousTerrainTileIds[2] = tiledAreaBlueprint.GetTerrainTileIDAtTile(new Vector2Int(x + currentPosition.x, y + currentPosition.y));
                        }
                        else
                        {
                            previousTerrainTileIds[2] = "";
                        }

                        if (i >= 2)
                        {
                            Vector2Int crossoverTileIndices = new Vector2Int(x + previousPosition.x, y + previousPosition.y);
                            Vector2 crossoverDirection = ((crossoverTileIndices - terrainTileIndices) * new Vector2(1, -1)).normalized;
                            if (TilePositioning.TileIndicesWithinBounds(crossoverTileIndices, tiledAreaBlueprint.MainAreaDimensions) && previousTerrainTileIds[1] != terrainTileIdAtCurrentTile)
                            {
                                pushEnabled = true;
                                if (i % 2 == 0)
                                {
                                    if (previousTerrainTileIds[0] != terrainTileIdAtCurrentTile && previousTerrainTileIds[2] != terrainTileIdAtCurrentTile)
                                    {
                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            terrainAtCurrentTile,
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
                                    else if (previousTerrainTileIds[0] == terrainTileIdAtCurrentTile && previousTerrainTileIds[2] == terrainTileIdAtCurrentTile)
                                    {
                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            terrainAtCurrentTile,
                                            true,
                                            crossoverDirection
                                            );
                                    }
                                }
                                else
                                {
                                    if ((previousTerrainTileIds[0] != terrainTileIdAtCurrentTile && previousTerrainTileIds[2] != terrainTileIdAtCurrentTile) ||
                                        (previousTerrainTileIds[0] == "" && previousTerrainTileIds[2] != terrainTileIdAtCurrentTile) ||
                                        (previousTerrainTileIds[0] != terrainTileIdAtCurrentTile && previousTerrainTileIds[2] == ""))
                                    {

                                        SpawnCrossoverTile(
                                            crossoverTileIndices,
                                            terrainTileIndices,
                                            tiledAreaBlueprint,
                                            contentsTransform,
                                            terrainAtCurrentTile,
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
                        terrainAtCurrentTile,
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
            TileTerrain terrain,
            bool pushEnabled,
            List<Vector2Int> cornerPoints
            )
        {
            if (this.terrainTileAssetGroup.TryGetTerrainTileAsset(terrain, tiledAreaBlueprint.GetTerrainTileIDAtTile(terrainTileIndices), out TerrainTileAsset terrainTileAsset))
            {
                TerrainTile spawnedTerrainTile = FrigidInstancing.CreateInstance<TerrainTile>(
                    terrainTileAsset.TerrainTilePrefab,
                    TilePositioning.TileAbsolutePositionFromIndices(terrainTileIndices, contentsTransform.position, tiledAreaBlueprint.MainAreaDimensions),
                    contentsTransform
                    );
                spawnedTerrainTile.Populated(pushEnabled, cornerPoints);
            }
        }

        private void SpawnCrossoverTile(
            Vector2Int crossoverTileIndices,
            Vector2Int terrainTileIndices,
            TiledAreaBlueprint tiledAreaBlueprint,
            Transform contentsTransform,
            TileTerrain terrain,
            bool isOuter,
            Vector2 direction
            )
        {
            if (this.terrainTileAssetGroup.TryGetTerrainTileAsset(terrain, tiledAreaBlueprint.GetTerrainTileIDAtTile(terrainTileIndices), out TerrainTileAsset terrainTileAsset))
            {
                if (terrainTileAsset.TryGetTerrainCrossoverTilePrefab(tiledAreaBlueprint.GetTerrainTileIDAtTile(crossoverTileIndices), out TerrainCrossoverTile terrainCrossoverTilePrefab))
                {
                    TerrainCrossoverTile spawnedCrossoverTile = FrigidInstancing.CreateInstance<TerrainCrossoverTile>(
                        terrainCrossoverTilePrefab,
                        TilePositioning.TileAbsolutePositionFromIndices(crossoverTileIndices, contentsTransform.position, tiledAreaBlueprint.MainAreaDimensions),
                        contentsTransform
                        );
                    spawnedCrossoverTile.Populated(direction, isOuter);
                }
            }
        }
    }
}
