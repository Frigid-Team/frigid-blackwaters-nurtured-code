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
        private Targeter originTargeter;
        [SerializeField]
        private IntSerializedReference numberRipples;
        [Space]
        [SerializeField]
        private Vector2Int rippleDimensions;
        [SerializeField]
        private TraversableTerrain traversableTerrain;
        [SerializeField]
        private Resistance traversableResistance;
        [SerializeField]
        private FloatSerializedReference rippleSpeed;
        [SerializeField]
        private FloatSerializedReference arcLength;

        protected override List<ExplosionSpawnParameters> GetExplosionSpawnParameters(TiledArea tiledArea, float elapsedDuration)
        {
            List<ExplosionSpawnParameters> explosionSpawnParameters = new List<ExplosionSpawnParameters>();

            int currNumberRipples = this.numberRipples.MutableValue;
            Vector2[] originPositions = this.originTargeter.Retrieve(new Vector2[currNumberRipples], elapsedDuration, 0);
            foreach (Vector2 originPosition in originPositions)
            {
                float distance = this.rippleSpeed.ImmutableValue * elapsedDuration;
                int numberIterations = Mathf.RoundToInt(Mathf.PI * 2 / (this.arcLength.ImmutableValue / distance));
                float angleBetweenRad = Mathf.PI * 2 / numberIterations;
                for (int i = 0; i < numberIterations; i++)
                {
                    float spawnAngleRad = angleBetweenRad * i;
                    Vector2 spawnPosition = originPosition + new Vector2(Mathf.Cos(spawnAngleRad), Mathf.Sin(spawnAngleRad)) * distance;
                    if (AreaTiling.TilePositionWithinBounds(spawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions))
                    {
                        Vector2Int tileIndexPosition = AreaTiling.RectIndexPositionFromPosition(spawnPosition, tiledArea.CenterPosition, tiledArea.MainAreaDimensions, this.rippleDimensions);
                        if (tiledArea.NavigationGrid.IsTraversable(tileIndexPosition, this.rippleDimensions, this.traversableTerrain, this.traversableResistance))
                        {
                            explosionSpawnParameters.Add(new ExplosionSpawnParameters(spawnPosition, spawnAngleRad * Mathf.Rad2Deg, 0, this.explosionPrefab));
                        }
                    }
                }
            }

            return explosionSpawnParameters;
        }
    }
}
