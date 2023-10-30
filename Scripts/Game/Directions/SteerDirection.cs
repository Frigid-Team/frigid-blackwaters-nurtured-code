using UnityEngine;

using FrigidBlackwaters.Core;

namespace FrigidBlackwaters.Game
{
    public class SteerDirection : Direction
    {
        [SerializeField]
        private Direction startDirection;
        [SerializeField]
        private Direction targetDirection;
        [SerializeField]
        private float steerSpeedDegrees;

        protected override Vector2[] CustomRetrieve(Vector2[] currDirections, float elapsedDuration, float elapsedDurationDelta)
        {
            Vector2[] startDirections = this.startDirection.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
            Vector2[] targetDirections = this.targetDirection.Retrieve(currDirections, elapsedDuration, elapsedDurationDelta);
            float angleStep = this.steerSpeedDegrees * Mathf.Deg2Rad * elapsedDurationDelta;
            Vector2[] directions = new Vector2[currDirections.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 steeredDirection = currDirections[i];
                if (steeredDirection == Vector2.zero)
                {
                    steeredDirection = startDirections[i];
                }
                directions[i] = steeredDirection;

                float angleDifference = Vector2.SignedAngle(steeredDirection, targetDirections[i]);
                if (angleDifference == 0f)
                {
                    continue;
                }
                float steeredAngleRad = steeredDirection.ComponentAngle0To360() * Mathf.Deg2Rad;
                steeredAngleRad = angleDifference > 0 ? (steeredAngleRad + angleStep) : (steeredAngleRad - angleStep);
                directions[i] = new Vector2(Mathf.Cos(steeredAngleRad), Mathf.Sin(steeredAngleRad));
            }
            return directions;
        }
    }
}
