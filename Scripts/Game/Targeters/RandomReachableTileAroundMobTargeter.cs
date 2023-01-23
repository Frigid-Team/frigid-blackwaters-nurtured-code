using UnityEngine;
using System.Collections.Generic;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomReachableTileAroundMobTargeter : Targeter
    {
        [SerializeField]
        private Mob mob;
        [SerializeField]
        private FloatSerializedReference minPathCost;
        [SerializeField]
        private FloatSerializedReference maxPathCost;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> reachableIndices = this.mob.TiledArea.NavigationGrid.FindReachableIndices(
                    this.mob.PositionIndices, 
                    this.mob.TileSize, 
                    this.mob.TraversableTerrain,
                    this.minPathCost.MutableValue,
                    this.maxPathCost.MutableValue
                    );
                if (reachableIndices.Count > 0)
                {
                    positions[i] = TilePositioning.RectAbsolutePositionFromIndices(
                        reachableIndices[Random.Range(0, reachableIndices.Count)],
                        this.mob.TiledArea.AbsoluteCenterPosition,
                        this.mob.TiledArea.MainAreaDimensions,
                        this.mob.TileSize
                        );
                }
                else
                {
                    positions[i] = this.mob.AbsolutePosition;
                }
            }
            return positions;
        }
    }
}
