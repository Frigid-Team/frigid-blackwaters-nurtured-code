using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomTraversableTileTargeter : Targeter
    {
        [SerializeField]
        private Targeter originTargeter;
        [SerializeField]
        private Vector2Int rectDimensions;
        [SerializeField]
        private TraversableTerrain traversableTerrain;
        [SerializeField]
        private FloatSerializedReference avoidanceDistance;
        [SerializeField]
        private List<Targeter> targetersToAvoid;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            HashSet<Vector2Int> previouslyChosenTileIndices = new HashSet<Vector2Int>();

            Vector2[] originPositions = this.originTargeter.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta);

            List<Vector2[]> separatedAvoidPositions = new List<Vector2[]>();
            foreach (Targeter targeterToAvoid in this.targetersToAvoid)
            {
                separatedAvoidPositions.Add(targeterToAvoid.Calculate(currentPositions, elapsedDuration, elapsedDurationDelta));
            }

            Vector2[] positions = new Vector2[currentPositions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                if (TiledArea.TryGetTiledAreaAtPosition(originPositions[i], out TiledArea tiledArea))
                {
                    List<Vector2Int> availableTileIndices = new List<Vector2Int>();
                    for (int x = 0; x < tiledArea.MainAreaDimensions.x; x++)
                    {
                        for (int y = 0; y < tiledArea.MainAreaDimensions.y; y++)
                        {
                            Vector2Int tileIndices = new Vector2Int(x, y);
                            Vector2 absoluteTilePosition = TilePositioning.TileAbsolutePositionFromIndices(tileIndices, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions);
                            bool isWithinAvoidRange = false;
                            foreach (Vector2[] avoidPositions in separatedAvoidPositions)
                            {
                                isWithinAvoidRange |= Vector2.Distance(avoidPositions[i], absoluteTilePosition) < this.avoidanceDistance.ImmutableValue;
                            }
                            foreach (Vector2Int chosenTileIndices in previouslyChosenTileIndices)
                            {
                                isWithinAvoidRange |= Vector2.Distance(TilePositioning.TileAbsolutePositionFromIndices(chosenTileIndices, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions), absoluteTilePosition) < this.avoidanceDistance.ImmutableValue;
                            }
                            if (isWithinAvoidRange || !tiledArea.NavigationGrid.IsTraversable(tileIndices, this.rectDimensions, this.traversableTerrain))
                            {
                                continue;
                            }
                            availableTileIndices.Add(tileIndices);
                        }
                    }

                    if (availableTileIndices.Count > 0)
                    {
                        Vector2Int chosenTileIndices = availableTileIndices[Random.Range(0, availableTileIndices.Count)];
                        previouslyChosenTileIndices.Add(chosenTileIndices);
                        positions[i] = TilePositioning.TileAbsolutePositionFromIndices(chosenTileIndices, tiledArea.AbsoluteCenterPosition, tiledArea.MainAreaDimensions);
                    }
                }
            }

            return positions;
        }
    }
}
