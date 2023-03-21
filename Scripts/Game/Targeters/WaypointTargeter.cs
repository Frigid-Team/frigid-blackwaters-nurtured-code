using UnityEngine;

namespace FrigidBlackwaters.Game
{
    public class WaypointTargeter : Targeter
    {
        [SerializeField]
        private MoveThroughWaypoints moveThroughWaypoints;

        public override Vector2[] Calculate(Vector2[] currentPositions, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] positions = new Vector2[currentPositions.Length];
            currentPositions.CopyTo(positions, 0);
            if (this.moveThroughWaypoints.TryGetNextWaypoint(out Vector2 waypoint))
            {
                for (int i = 0; i < positions.Length; i++) positions[i] = waypoint;
            }
            return positions;
        }
    }
}
