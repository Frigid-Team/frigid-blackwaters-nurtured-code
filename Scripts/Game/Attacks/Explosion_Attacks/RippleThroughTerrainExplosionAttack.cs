using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RippleThroughTerrainExplosionAttack : ExplosionAttack
    {
        [SerializeField]
        private Explosion explosionPrefab;
        [SerializeField]
        private Vector2Int rippleDimensions;
        [SerializeField]
        private TraversableTerrain traversableTerrain;
        [SerializeField]
        private FloatSerializedReference rippleSpeed;
        [SerializeField]
        private FloatSerializedReference arcLength;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(Vector2 baseSpawnPosition, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();
            if (TiledArea.TryGetTiledAreaAtPosition(baseSpawnPosition, out TiledArea tiledArea))
            {
                float distance = this.rippleSpeed.ImmutableValue * elapsedDuration;
                int numberIterations = Mathf.RoundToInt(Mathf.PI * 2 / (this.arcLength.ImmutableValue / distance));
                float angleBetweenRad = Mathf.PI * 2 / numberIterations;
                for (int i = 0; i < numberIterations; i++)
                {
                    float spawnAngleRad = angleBetweenRad * i;
                    Vector2 spawnPosition = baseSpawnPosition + new Vector2(Mathf.Cos(spawnAngleRad), Mathf.Sin(spawnAngleRad)) * distance;
                    if (TilePositioning.TileAbsolutePositionWithinBounds(spawnPosition, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions))
                    {
                        Vector2Int tileIndices = TilePositioning.RectIndicesFromAbsolutePosition(spawnPosition, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions, this.rippleDimensions);
                        if (tiledArea.NavigationGrid.IsTraversable(tileIndices, this.rippleDimensions, this.traversableTerrain))
                        {
                            explosionSpawnParameters.Add(new ExplosionSpawnParameters(spawnPosition, spawnAngleRad * Mathf.Rad2Deg, this.explosionPrefab));
                        }
                    }
                }
            }
            return explosionSpawnParameters;
        }
    }
}
