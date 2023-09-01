using System.Collections.Generic;
using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class RandomTraversableTileAroundMobTargeter : Targeter
    {
        [SerializeField]
        private MobSerializedHandle mob;
        [SerializeField]
        private Vector2Int rectDimensions;
        [SerializeField]
        private TraversableTerrain traversableTerrain;
        [SerializeField]
        private FloatSerializedReference avoidanceDistance;
        [SerializeField]
        private List<Targeter> targetersToAvoid;
        [SerializeField]
        private List<MobQuery> mobsToAvoidQueries;
        [SerializeField]
        private FloatSerializedReference minimumDistanceApart;

        protected override Vector2[] CustomRetrieve(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            if (!this.mob.TryGetValue(out Mob mob))
            {
                return currentPositions;
            }

            List<Vector2> positionsToAvoid = new List<Vector2>();
            foreach (Targeter targeterToAvoid in this.targetersToAvoid)
            {
                positionsToAvoid.Add(targeterToAvoid.Retrieve(Vector2.zero, elapsedDuration, elapsedDurationDelta));
            }
            foreach (MobQuery mobsToAvoidQuery in this.mobsToAvoidQueries)
            {
                foreach (Mob mobToAvoid in mobsToAvoidQuery.Execute())
                {
                    positionsToAvoid.Add(mobToAvoid.Position);
                }
            }

            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);

            HashSet<Vector2Int> previouslyChosenTileIndexPositions = new HashSet<Vector2Int>();
            for (int i = 0; i < positions.Length; i++)
            {
                List<Vector2Int> availableTileIndexPositions = new List<Vector2Int>();
                for (int x = 0; x < mob.TiledArea.MainAreaDimensions.x; x++)
                {
                    for (int y = 0; y < mob.TiledArea.MainAreaDimensions.y; y++)
                    {
                        Vector2Int tileIndexPosition = new Vector2Int(x, y);
                        if (!mob.TiledArea.NavigationGrid.IsTraversable(tileIndexPosition, this.rectDimensions, this.traversableTerrain, Resistance.None))
                        {
                            continue;
                        }

                        Vector2 tilePosition = AreaTiling.TilePositionFromIndexPosition(tileIndexPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                        bool isWithinUnacceptableRange = false;
                        foreach (Vector2 positionToAvoid in positionsToAvoid)
                        {
                            isWithinUnacceptableRange |= Vector2.Distance(positionToAvoid, tilePosition) < this.avoidanceDistance.ImmutableValue;
                        }
                        foreach (Vector2Int previouslyChosenTileIndexPosition in previouslyChosenTileIndexPositions)
                        {
                            isWithinUnacceptableRange |= Vector2.Distance(AreaTiling.TilePositionFromIndexPosition(previouslyChosenTileIndexPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions), tilePosition) < this.minimumDistanceApart.ImmutableValue;
                        }
                        if (isWithinUnacceptableRange)
                        {
                            continue;
                        }

                        availableTileIndexPositions.Add(tileIndexPosition);
                    }
                }

                if (availableTileIndexPositions.Count > 0)
                {
                    Vector2Int chosenTileIndexPosition = availableTileIndexPositions[Random.Range(0, availableTileIndexPositions.Count)];
                    previouslyChosenTileIndexPositions.Add(chosenTileIndexPosition);
                    positions[i] = AreaTiling.TilePositionFromIndexPosition(chosenTileIndexPosition, mob.TiledArea.CenterPosition, mob.TiledArea.MainAreaDimensions);
                }
            }

            return positions;
        }
    }
}
