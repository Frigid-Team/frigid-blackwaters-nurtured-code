using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomReachableTileAroundMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedReference mob;
        [SerializeField]
        private FloatSerializedReference minPathCost;
        [SerializeField]
        private FloatSerializedReference maxPathCost;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Mob mob = this.mob.ImmutableValue;
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> reachableIndices = mob.TiledArea.NavigationGrid.FindReachableIndices(
                    mob.PositionIndices,
                    mob.TileSize,
                    mob.TraversableTerrain,
                    this.minPathCost.MutableValue,
                    this.maxPathCost.MutableValue
                    );
                if (reachableIndices.Count > 0)
                {
                    positions[i] = TilePositioning.RectPositionFromIndices(
                        reachableIndices[Random.Range(0, reachableIndices.Count)],
                        mob.TiledArea.CenterPosition,
                        mob.TiledArea.MainAreaDimensions,
                        mob.TileSize
                        );
                }
                else
                {
                    positions[i] = mob.Position;
                }
            }
            return positions;
        }
    }
}
